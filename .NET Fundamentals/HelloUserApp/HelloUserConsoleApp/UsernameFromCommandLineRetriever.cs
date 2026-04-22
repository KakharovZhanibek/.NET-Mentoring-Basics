using System;
using System.CommandLine;
using System.Threading.Tasks;

namespace HelloUserConsoleApp
{
    internal class UsernameFromCommandLineRetriever
    {
        public async Task<string> GetUsernameFromCommandLineArgs(string[] args)
        {
            var rootCommand = new RootCommand("Sample command-line app");
            var username = string.Empty;

            var nameOption = new Option<string[]>("--name", "-n")
            {
                Description = "Username",
                AllowMultipleArgumentsPerToken = true,
            };

            rootCommand.Options.Add(nameOption);

            rootCommand.SetAction(parseResult =>
            {
                var usernameArgs = parseResult.GetValue(nameOption);
                if (usernameArgs != null && usernameArgs.Length > 0)
                {
                    username = string.Join(" ", usernameArgs);
                }
            });

            await rootCommand.Parse(args).InvokeAsync();
            Console.Clear();
            return username;
        }
    }
}
