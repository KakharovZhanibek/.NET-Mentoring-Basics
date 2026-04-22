using System;
using System.Threading.Tasks;
using HelloUserLibrary.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace HelloUserConsoleApp
{
    public static class Program
    {
        static async Task Main(string[] args)
        {
            var servicesProvider = new ServiceCollection()
                .AddSingleton<INameProvider, ConsoleNameProvider>()
                .AddTransient<IGreeterService, ConsoleUserGreeterService>()
                .AddTransient<UsernameFromCommandLineRetriever>()
                .BuildServiceProvider();
            
            Console.Clear();
            var usernameRetriever = servicesProvider.GetRequiredService<UsernameFromCommandLineRetriever>();
            string username = await usernameRetriever.GetUsernameFromCommandLineArgs(args);

            var greeterService = servicesProvider.GetRequiredService<IGreeterService>();
            greeterService.Greet(username);

            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
