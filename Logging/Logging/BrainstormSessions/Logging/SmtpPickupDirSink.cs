using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting;
using Serilog.Formatting.Display;

namespace BrainstormSessions.Logging
{
    /// <summary>
    /// A Serilog sink that emails each log event via SMTP.
    /// <para>
    /// Two delivery modes are selected automatically based on
    /// <see cref="SmtpPickupDirOptions.SmtpHost"/>:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <b>Pickup-directory mode</b> – <see cref="SmtpPickupDirOptions.SmtpHost"/>
    ///     is null/empty.  Each event is written as a .eml file to
    ///     <see cref="SmtpPickupDirOptions.PickupDirectory"/> (mirrors
    ///     log4net's SmtpPickupDirAppender).  No credentials needed.
    ///   </item>
    ///   <item>
    ///     <b>Network mode</b> – <see cref="SmtpPickupDirOptions.SmtpHost"/> is set.
    ///     Each event is sent directly over the network using the configured
    ///     <see cref="SmtpPickupDirOptions.Username"/> and
    ///     <see cref="SmtpPickupDirOptions.Password"/> credentials.
    ///   </item>
    /// </list>
    /// </summary>
    public sealed class SmtpPickupDirSink : ILogEventSink, IDisposable
    {
        private const string DefaultOutputTemplate =
            "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}";

        private readonly SmtpPickupDirOptions _options;
        private readonly ITextFormatter _formatter;

        public SmtpPickupDirSink(SmtpPickupDirOptions options, ITextFormatter formatter = null)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _formatter = formatter
                ?? new MessageTemplateTextFormatter(DefaultOutputTemplate);

            // Pre-create the pickup directory when running in pickup-dir mode.
            if (string.IsNullOrWhiteSpace(_options.SmtpHost))
                Directory.CreateDirectory(_options.PickupDirectory);
        }

        /// <inheritdoc />
        public void Emit(LogEvent logEvent)
        {
            if (logEvent is null) throw new ArgumentNullException(nameof(logEvent));

            using var bodyWriter = new StringWriter();
            _formatter.Format(logEvent, bodyWriter);
            var body = bodyWriter.ToString();

            var subject = $"[{logEvent.Level}] {_options.Subject}";

            using var message = new MailMessage(
                from: _options.From,
                to: _options.To,
                subject: subject,
                body: body);

            using var client = BuildSmtpClient();
            client.Send(message);
        }

        /// <inheritdoc />
        public void Dispose() { /* SmtpClient is disposed per-send */ }

        // ------------------------------------------------------------------ //
        //  Private helpers
        // ------------------------------------------------------------------ //

        private SmtpClient BuildSmtpClient()
        {
            // Network mode: SmtpHost is configured – authenticate and send directly.
            if (!string.IsNullOrWhiteSpace(_options.SmtpHost))
            {
                var client = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
                {
                    DeliveryMethod = SmtpDeliveryMethod.Network,
                    EnableSsl = _options.EnableSsl
                };

                // Only attach credentials when a username is provided.
                // Some internal relays allow unauthenticated relay from trusted IPs.
                if (!string.IsNullOrWhiteSpace(_options.Username))
                {
                    client.Credentials = new NetworkCredential(
                        _options.Username,
                        _options.Password);
                }

                return client;
            }

            // Pickup-directory mode: no network connection, no credentials needed.
            return new SmtpClient
            {
                DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory,
                PickupDirectoryLocation = Path.GetFullPath(_options.PickupDirectory)
            };
        }
    }
}
