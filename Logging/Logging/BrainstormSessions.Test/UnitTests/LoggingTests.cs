using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BrainstormSessions.Api;
using BrainstormSessions.Controllers;
using BrainstormSessions.Core.Interfaces;
using BrainstormSessions.Core.Model;
using Microsoft.Extensions.Logging;
using Moq;
using Serilog;
using Serilog.Events;
using Serilog.Extensions.Logging;
using Serilog.Sinks.InMemory;
using Xunit;

namespace BrainstormSessions.Test.UnitTests
{
    /// <summary>
    /// Verifies that each controller emits the expected Serilog log levels.
    /// Serilog's InMemorySink captures all events so assertions are made against
    /// <see cref="InMemorySink.Instance.LogEvents"/> — no log4net dependency needed.
    /// </summary>
    public class LoggingTests : IDisposable
    {
        private readonly SerilogLoggerFactory _loggerFactory;

        public LoggingTests()
        {
            // Configure a Serilog logger that writes everything (Verbose+) to the
            // shared InMemorySink.  The factory bridges Serilog ↔ ILogger<T>.
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.InMemory()
                .CreateLogger();

            _loggerFactory = new SerilogLoggerFactory(Log.Logger, dispose: false);
        }

        public void Dispose()
        {
            // Clear captured events between tests.
            InMemorySink.Instance.Dispose();
            _loggerFactory.Dispose();
        }

        // ------------------------------------------------------------------ //
        //  Helpers
        // ------------------------------------------------------------------ //

        private ILogger<T> CreateLogger<T>() => _loggerFactory.CreateLogger<T>();

        private static List<BrainstormSession> GetTestSessions() =>
            new List<BrainstormSession>
            {
                new BrainstormSession { DateCreated = new DateTime(2016, 7, 2), Id = 1, Name = "Test One" },
                new BrainstormSession { DateCreated = new DateTime(2016, 7, 1), Id = 2, Name = "Test Two" }
            };

        // ------------------------------------------------------------------ //
        //  Tests
        // ------------------------------------------------------------------ //

        [Fact]
        public async Task HomeController_Index_LogInfoMessages()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync()).ReturnsAsync(GetTestSessions());
            var controller = new HomeController(mockRepo.Object, CreateLogger<HomeController>());

            // Act
            await controller.Index();

            // Assert
            Assert.True(
                InMemorySink.Instance.LogEvents.Any(e => e.Level == LogEventLevel.Information),
                "Expected Information messages in the logs");
        }

        [Fact]
        public async Task HomeController_IndexPost_LogWarningMessage_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.ListAsync()).ReturnsAsync(GetTestSessions());
            var controller = new HomeController(mockRepo.Object, CreateLogger<HomeController>());
            controller.ModelState.AddModelError("SessionName", "Required");

            // Act
            await controller.Index(new HomeController.NewSessionModel());

            // Assert
            Assert.True(
                InMemorySink.Instance.LogEvents.Any(e => e.Level == LogEventLevel.Warning),
                "Expected Warning messages in the logs");
        }

        [Fact]
        public async Task IdeasController_CreateActionResult_LogErrorMessage_WhenModelStateIsInvalid()
        {
            // Arrange
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            var controller = new IdeasController(mockRepo.Object, CreateLogger<IdeasController>());
            controller.ModelState.AddModelError("error", "some error");

            // Act
            await controller.CreateActionResult(model: null);

            // Assert
            Assert.True(
                InMemorySink.Instance.LogEvents.Any(e => e.Level == LogEventLevel.Error),
                "Expected Error messages in the logs");
        }

        [Fact]
        public async Task SessionController_Index_LogDebugMessages()
        {
            // Arrange
            int testSessionId = 1;
            var mockRepo = new Mock<IBrainstormSessionRepository>();
            mockRepo.Setup(repo => repo.GetByIdAsync(testSessionId))
                .ReturnsAsync(GetTestSessions().FirstOrDefault(s => s.Id == testSessionId));
            var controller = new SessionController(mockRepo.Object, CreateLogger<SessionController>());

            // Act
            await controller.Index(testSessionId);

            // Assert – SessionController.Index emits exactly 2 Debug events:
            // one at entry and one after the session is successfully fetched.
            Assert.True(
                InMemorySink.Instance.LogEvents.Count(e => e.Level == LogEventLevel.Debug) == 2,
                "Expected 2 Debug messages in the logs");
        }
    }
}
