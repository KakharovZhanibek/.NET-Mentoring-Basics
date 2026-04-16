using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using HelloUserConsoleApp.Services;
using HelloUserLibrary;

namespace HelloUserConsoleApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var userName = args.Length > 0 ? string.Join(" ", args) : string.Empty;
            var greeterService = new GreeterService();
            greeterService.Greet(userName);
        }
    }
}
