using System;
using System.Collections.Generic;
using System.Text;
using HelloUserConsoleApp.Interfaces;
using HelloUserLibrary;

namespace HelloUserConsoleApp.Services
{
    internal class GreeterService : IGreeterService
    {
        public void Greet(string username)
        {
            string userGreeting = $"Hello, {username}!";
            Console.Clear();
            if (!string.IsNullOrEmpty(username))
            {
                Console.WriteLine(DatetimeUserGreetingConcatenator.Concatenate(userGreeting));
                Console.ReadKey();
                return;
            }

            Console.Write("Enter your Username: ");
            username = Console.ReadLine();
            username = string.IsNullOrWhiteSpace(username) ? "User" : username.Trim();
            userGreeting = $"Hello, {username}!";

            Console.Clear();
            Console.WriteLine(DatetimeUserGreetingConcatenator.Concatenate(userGreeting));
            Console.ReadKey();
        }
    }
}
