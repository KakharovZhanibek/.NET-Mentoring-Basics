# BrainstormSessions — Logging Guide

ASP.NET Core 3.0 MVC application using **Serilog** for structured logging.

---

## Quick Start

```bash
# Run the application
dotnet run --project BrainstormSessions

# Run all tests
dotnet test BrainstormSessions.Test
```

---

## Where Logs Are Saved

Two sinks are active simultaneously:

### 1. Rolling File — all levels ? Debug

| Property | Value |
|---|---|
| **Location** | `logs/brainstormsessions-YYYYMMDD.txt` |
| **Relative to** | Application working directory (e.g. `BrainstormSessions/bin/Debug/netcoreapp3.0/`) |
| **Rolling** | New file created every day |
| **Levels captured** | Debug, Information, Warning, Error, Fatal |

Example path:
```
BrainstormSessions/bin/Debug/netcoreapp3.0/logs/brainstormsessions-20241215.txt
```

Example entry:
```
2024-12-15 10:23:41.123 +05:00 [INF] HomeController Fetching all brainstorm sessions.
2024-12-15 10:23:41.130 [ERR] IdeasController CreateActionResult called with invalid model state.
```

### 2. SMTP Email — Error and Fatal only

Every **Error** or **Fatal** event triggers an email.  
Two delivery modes are available, configured in `appsettings.json`.

---

## Configuring Email (`appsettings.json`)

```json
"SmtpPickupDir": {
  "From":    "sender@example.com",
  "To":      "recipient@example.com",
  "Subject": "BrainstormSessions Log Alert",

  "PickupDirectory": "smtp-pickup",

  "SmtpHost": "smtp.gmail.com",
  "SmtpPort": 587,
  "EnableSsl": true,
  "Username": "sender@example.com",
  "Password": ""
}
```

### Mode 1 — Pickup Directory (development / local testing)

Leave `SmtpHost` **empty**. Each Error/Fatal event is written as a `.eml` file to `smtp-pickup/` relative to the working directory. A local SMTP relay (e.g. IIS SMTP) picks up and delivers the files. No credentials required.

```json
"SmtpHost": ""
```

### Mode 2 — Network SMTP (staging / production)

Set `SmtpHost` to your mail server. The sink connects directly and authenticates with `Username` / `Password`.

```json
"SmtpHost": "smtp.gmail.com",
"SmtpPort": 587,
"EnableSsl": true,
"Username": "sender@gmail.com"
```

> ?? **Never commit a real password to source control.**  
> Supply it via an environment variable or .NET user-secrets instead:

```bash
# Environment variable (staging / production)
$env:SmtpPickupDir__Password = "your-app-password"

# .NET user-secrets (local development only)
dotnet user-secrets set "SmtpPickupDir:Password" "your-app-password" --project BrainstormSessions
```

#### Gmail setup
1. Enable 2-Step Verification on the Gmail account.
2. Go to [myaccount.google.com/apppasswords](https://myaccount.google.com/apppasswords) and generate a 16-character App Password.
3. Use that App Password — **not** your regular Gmail password.

---

## Log Levels

| Level | File sink | Email sink | Where emitted |
|---|---|---|---|
| **Debug** | ? | ? | `SessionController.Index` (entry + result) |
| **Information** | ? | ? | `HomeController.Index`, `IdeasController` success paths |
| **Warning** | ? | ? | `HomeController.Index` invalid POST, session not found |
| **Error** | ? | ? | `IdeasController.CreateActionResult` invalid model state |
| **Fatal** | ? | ? | Unhandled host startup exception |

---

## Testing Email Delivery

To send a test Error and Fatal email on startup, uncomment the two blocks in `Program.cs`:

```csharp
// FOR TESTING ONLY – remove before deploying to production
Log.Error(
    "TEST ERROR – startup probe to verify SMTP email delivery. Timestamp: {Timestamp}",
    DateTimeOffset.Now);

Log.Fatal(
    new InvalidOperationException("Simulated fatal condition for SMTP delivery test."),
    "TEST FATAL – startup probe to verify SMTP email delivery. Timestamp: {Timestamp}",
    DateTimeOffset.Now);
```

Start the app — two emails should arrive at the `To` address within seconds. Remove the blocks when delivery is confirmed.

---

## Running Tests

```bash
dotnet test BrainstormSessions.Test
```

| Test class | What it covers |
|---|---|
| `LoggingTests` | Each controller emits the correct log level via `ILogger<T>` (uses Serilog `InMemorySink`) |
| `SmtpPickupDirSinkTests` | `SmtpPickupDirSink` writes `.eml` files, respects minimum level, creates pickup dir automatically |

---

## Project Structure

```
BrainstormSessions/
??? Controllers/
?   ??? HomeController.cs       # Info + Warning logs
?   ??? SessionController.cs    # Debug logs
??? Api/
?   ??? IdeasController.cs      # Debug + Info + Warning + Error logs
??? Logging/
?   ??? SmtpPickupDirOptions.cs     # Configuration POCO
?   ??? SmtpPickupDirSink.cs        # Custom Serilog sink
?   ??? SmtpPickupDirSinkExtensions.cs  # Fluent API extension
??? appsettings.json            # Serilog + SMTP settings
??? Program.cs                  # Logger bootstrap
```
