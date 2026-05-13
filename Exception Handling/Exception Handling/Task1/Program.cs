using System;

namespace Task1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Console.WriteLine("Enter input lines (type 'exit' to quit):");
            
            while (true)
            {
                string input = Console.ReadLine();
                
                if (string.Equals(input, "exit", StringComparison.OrdinalIgnoreCase))
                {
                    break;
                }
                
                PrintFirstCharacter(input);
            }
        }
        
        private static void PrintFirstCharacter(string input)
        {
            try
            {
                if (string.IsNullOrEmpty(input))
                {
                    throw new ArgumentException("Input string cannot be null or empty.");
                }
                
                char firstChar = input[0];
                Console.WriteLine($"First character: {firstChar}");
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}