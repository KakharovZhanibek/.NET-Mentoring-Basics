using System;
using System.IO;
using System.Linq;
using BrainstormSessions.Logging;
using Serilog;
using Serilog.Events;
using Xunit;

namespace BrainstormSessions.Test.UnitTests
{
    /// <summary>
    /// Verifies that <see cref="SmtpPickupDirSink"/> writes .eml files to the
    /// configured pickup directory, mirroring log4net's SmtpPickupDirAppender.
    /// </summary>
    public class SmtpPickupDirSinkTests : IDisposable
    {
        // Use a unique temp directory per test run so parallel runs don't clash.
        private readonly string _pickupDir;

        public SmtpPickupDirSinkTests()
        {
            _pickupDir = Path.Combine(Path.GetTempPath(), $"smtp-pickup-{Guid.NewGuid():N}");
        }

        public void Dispose()
        {
            // Clean up the temporary pickup directory after each test.
            if (Directory.Exists(_pickupDir))
                Directory.Delete(_pickupDir, recursive: true);
        }

        // ------------------------------------------------------------------ //
        //  Helper
        // ------------------------------------------------------------------ //

        private ILogger BuildLogger(LogEventLevel minimumLevel = LogEventLevel.Error)
        {
            var options = new SmtpPickupDirOptions
            {
                PickupDirectory = _pickupDir,
                From = "sender@test.local",
                To = "recipient@test.local",
                Subject = "Test Alert"
            };

            return new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.SmtpPickupDir(options, restrictedToMinimumLevel: minimumLevel)
                .CreateLogger();
        }

        // ------------------------------------------------------------------ //
        //  Tests
        // ------------------------------------------------------------------ //

        [Fact]
        public void Emit_ErrorEvent_CreatesEmlFileInPickupDirectory()
        {
            // Arrange
            var logger = BuildLogger();

            // Act
            logger.Error("An error occurred during {Operation}", "UnitTest");

            // Assert – exactly one .eml file should appear in the pickup dir.
            var emlFiles = Directory.GetFiles(_pickupDir, "*.eml");
            Assert.Single(emlFiles);
        }

        [Fact]
        public void Emit_ErrorEvent_EmlFileContainsExpectedHeaders()
        {
            // Arrange
            var logger = BuildLogger();

            // Act
            logger.Error("Critical failure in {Module}", "PaymentService");

            // Assert – the .eml file must contain From/To/Subject headers.
            var eml = File.ReadAllText(Directory.GetFiles(_pickupDir, "*.eml").Single());
            Assert.Contains("sender@test.local", eml);
            Assert.Contains("recipient@test.local", eml);
            Assert.Contains("Test Alert", eml);
        }

        [Fact]
        public void Emit_ErrorEvent_EmlBodyContainsLogMessage()
        {
            // Arrange
            var logger = BuildLogger();
            // Keep the message short: quoted-printable wraps at 76 chars and the
            // timestamp prefix already takes ~43 chars, so a ?30-char message is
            // guaranteed to stay on a single QP line and appear verbatim in the file.
            const string message = "DB error";

            // Act
            logger.Error(message);

            // Assert
            var eml = File.ReadAllText(Directory.GetFiles(_pickupDir, "*.eml").Single());
            Assert.Contains(message, eml);
        }

        [Fact]
        public void Emit_BelowMinimumLevel_NoEmlFileCreated()
        {
            // Arrange – sink configured for Error+, emit only Warning.
            var logger = BuildLogger(minimumLevel: LogEventLevel.Error);

            // Act
            logger.Warning("This should NOT produce an e-mail");

            // Assert
            var emlFiles = Directory.Exists(_pickupDir)
                ? Directory.GetFiles(_pickupDir, "*.eml")
                : Array.Empty<string>();
            Assert.Empty(emlFiles);
        }

        [Fact]
        public void Emit_MultipleErrorEvents_CreatesOneEmlFilePerEvent()
        {
            // Arrange
            var logger = BuildLogger();

            // Act
            logger.Error("First error");
            logger.Error("Second error");
            logger.Fatal("Fatal problem");

            // Assert
            var emlFiles = Directory.GetFiles(_pickupDir, "*.eml");
            Assert.Equal(3, emlFiles.Length);
        }

        [Fact]
        public void Sink_PickupDirectory_IsCreatedAutomatically()
        {
            // Arrange – directory does NOT exist yet.
            Assert.False(Directory.Exists(_pickupDir));

            var options = new SmtpPickupDirOptions { PickupDirectory = _pickupDir };

            // Act – constructing the sink should create the directory.
            using var sink = new SmtpPickupDirSink(options);

            // Assert
            Assert.True(Directory.Exists(_pickupDir));
        }

        [Fact]
        public void Emit_FatalEvent_EmlSubjectContainsLevelName()
        {
            // Arrange
            var options = new SmtpPickupDirOptions
            {
                PickupDirectory = _pickupDir,
                From = "noreply@test.local",
                To = "ops@test.local",
                Subject = "BrainstormSessions Alert"
            };

            var logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.SmtpPickupDir(options, restrictedToMinimumLevel: LogEventLevel.Verbose)
                .CreateLogger();

            // Act
            logger.Fatal("Server is on fire");

            // Assert – subject line must contain the level token "Fatal"
            var eml = File.ReadAllText(Directory.GetFiles(_pickupDir, "*.eml").Single());
            Assert.Contains("Fatal", eml);
        }
    }
}
