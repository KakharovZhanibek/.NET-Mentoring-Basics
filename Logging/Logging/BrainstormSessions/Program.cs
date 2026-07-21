using System;
using System.IO;
using BrainstormSessions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;

namespace BrainstormSessions
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Read appsettings.json so that SMTP pickup-dir settings can be
            // consumed before the full DI host is built.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var smtpOptions = new SmtpPickupDirOptions();
            configuration.GetSection("SmtpPickupDir").Bind(smtpOptions);

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                // Sink 1 – rolling file: all levels ≥ Debug
                .WriteTo.File(
                    "logs/brainstormsessions-.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}")
                // Sink 2 – SMTP: only Error and above become e-mails
                .WriteTo.SmtpPickupDir(
                    smtpOptions,
                    restrictedToMinimumLevel: LogEventLevel.Error)
                .CreateLogger();

            // ----------------------------------------------------------------
            // FOR TESTING ONLY – remove before deploying to production.
            // Fires one Error and one Fatal event immediately after the logger
            // is configured so the SMTP sink can be verified end-to-end
            // (two emails should arrive at the address in appsettings.json).
            // ----------------------------------------------------------------
            //Log.Error(
            //    "TEST ERROR – startup probe to verify SMTP email delivery. " +
            //    "Timestamp: {Timestamp}",
            //    DateTimeOffset.Now);

            //Log.Fatal(
            //    new InvalidOperationException("Simulated fatal condition for SMTP delivery test."),
            //    "TEST FATAL – startup probe to verify SMTP email delivery. " +
            //    "Timestamp: {Timestamp}",
            //    DateTimeOffset.Now);
            // ----------------------------------------------------------------

            try
            {
                Log.Information("Starting BrainstormSessions web host");
                CreateHostBuilder(args).Build().Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Host terminated unexpectedly");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseSerilog()
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
