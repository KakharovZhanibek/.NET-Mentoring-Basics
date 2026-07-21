namespace BrainstormSessions.Logging
{
    /// <summary>
    /// Configuration options for the Serilog SMTP sink.
    /// <para>
    /// Two delivery modes are supported, selected automatically based on whether
    /// <see cref="SmtpHost"/> is set:
    /// </para>
    /// <list type="bullet">
    ///   <item>
    ///     <b>Pickup-directory mode</b> (default, <see cref="SmtpHost"/> is null/empty) –
    ///     each log event is written as a .eml file to <see cref="PickupDirectory"/>.
    ///     No SMTP connection or credentials are required.  An SMTP relay monitors
    ///     the directory and delivers the files.  Ideal for local development and
    ///     testing (mirrors log4net's SmtpPickupDirAppender).
    ///   </item>
    ///   <item>
    ///     <b>Network mode</b> (<see cref="SmtpHost"/> is provided) –
    ///     each log event is sent directly to the configured SMTP server using
    ///     <see cref="Username"/> / <see cref="Password"/> credentials.
    ///     Use this in staging and production environments.
    ///     Store the password in an environment variable or .NET user-secrets,
    ///     NOT in appsettings.json committed to source control.
    ///   </item>
    /// </list>
    /// </summary>
    public class SmtpPickupDirOptions
    {
        // ------------------------------------------------------------------ //
        //  Shared fields (both modes)
        // ------------------------------------------------------------------ //

        /// <summary>Sender e-mail address shown in the From: header.</summary>
        public string From { get; set; } = "logs@brainstormsessions.local";

        /// <summary>Recipient e-mail address(es) shown in the To: header.</summary>
        public string To { get; set; } = "admin@brainstormsessions.local";

        /// <summary>
        /// Base subject line. The log level is prepended automatically, e.g.
        /// "[Error] BrainstormSessions Log Alert".
        /// </summary>
        public string Subject { get; set; } = "BrainstormSessions Log Alert";

        // ------------------------------------------------------------------ //
        //  Pickup-directory mode fields
        // ------------------------------------------------------------------ //

        /// <summary>
        /// Local directory path where .eml files are dropped when
        /// <see cref="SmtpHost"/> is not set.
        /// Relative paths are resolved from the working directory at runtime.
        /// Default: "smtp-pickup"
        /// </summary>
        public string PickupDirectory { get; set; } = "smtp-pickup";

        // ------------------------------------------------------------------ //
        //  Network mode fields  (only used when SmtpHost is set)
        // ------------------------------------------------------------------ //

        /// <summary>
        /// SMTP server hostname or IP address (e.g. "smtp.gmail.com",
        /// "smtp.office365.com").  When set, network delivery is used instead
        /// of the pickup directory.
        /// </summary>
        public string SmtpHost { get; set; }

        /// <summary>SMTP server port. Default: 587 (STARTTLS).</summary>
        public int SmtpPort { get; set; } = 587;

        /// <summary>
        /// Whether to use SSL/TLS.  Set to <c>true</c> for port 465 (implicit
        /// TLS); leave <c>false</c> for port 587 (STARTTLS is negotiated
        /// automatically by <see cref="System.Net.Mail.SmtpClient"/>).
        /// </summary>
        public bool EnableSsl { get; set; } = true;

        /// <summary>
        /// SMTP authentication username (typically the From address).
        /// Override at runtime via the environment variable
        /// <c>SmtpPickupDir__Username</c> or .NET user-secrets.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        /// SMTP authentication password.
        /// <b>Never commit a real password to source control.</b>
        /// Override at runtime via the environment variable
        /// <c>SmtpPickupDir__Password</c> or .NET user-secrets.
        /// </summary>
        public string Password { get; set; }
    }
}
