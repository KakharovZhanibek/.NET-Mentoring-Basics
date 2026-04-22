using System;
using HelloUserLibrary.Interfaces;

namespace HelloUserConsoleApp
{
    internal class ConsoleNameProvider : INameProvider
    {
        public string ReadUserName()
        {
            string username = string.Empty;
            while (string.IsNullOrEmpty(username))
            {
                Console.WriteLine("Please enter your name:");
                username = Console.ReadLine();
                if (string.IsNullOrEmpty(username))
                {
                    Console.WriteLine("Name cannot be empty. Please try again.");
                }
            }
            return username;
        }

        public void WriteUserName(string userName)
        {
            Console.Clear();
            Console.WriteLine(userName);
        }
    }
}
