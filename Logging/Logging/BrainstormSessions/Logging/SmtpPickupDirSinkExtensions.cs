using System;
using BrainstormSessions.Logging;
using Serilog;
using Serilog.Configuration;
using Serilog.Events;
using Serilog.Formatting;

namespace BrainstormSessions.Logging
{
    /// <summary>
    /// Fluent-API extensions that wire <see cref="SmtpPickupDirSink"/> into a
    /// Serilog <see cref="LoggerConfiguration"/>.
    /// </summary>
    public static class SmtpPickupDirSinkExtensions
    {
        /// <summary>
        /// Write log events to an SMTP pickup directory as .eml files.
        /// </summary>
        /// <param name="sinkConfiguration">Logger sink configuration.</param>
        /// <param name="options">Sink options (pickup directory, from/to addresses, etc.).</param>
        /// <param name="restrictedToMinimumLevel">
        /// Minimum log level for this sink. Defaults to <see cref="LogEventLevel.Error"/>
        /// so that only Error and Fatal events trigger an e-mail.
        /// </param>
        /// <param name="formatter">
        /// Optional custom <see cref="ITextFormatter"/> for the e-mail body.
        /// When <c>null</c> the sink's built-in template is used.
        /// </param>
        public static LoggerConfiguration SmtpPickupDir(
            this LoggerSinkConfiguration sinkConfiguration,
            SmtpPickupDirOptions options,
            LogEventLevel restrictedToMinimumLevel = LogEventLevel.Error,
            ITextFormatter formatter = null)
        {
            if (sinkConfiguration is null) throw new ArgumentNullException(nameof(sinkConfiguration));
            if (options is null) throw new ArgumentNullException(nameof(options));

            return sinkConfiguration.Sink(
                new SmtpPickupDirSink(options, formatter),
                restrictedToMinimumLevel);
        }
    }
}
