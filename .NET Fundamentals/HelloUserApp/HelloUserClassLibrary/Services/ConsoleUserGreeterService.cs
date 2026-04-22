using HelloUserLibrary;
using HelloUserLibrary.Interfaces;

namespace HelloUserConsoleApp
{
    public class ConsoleUserGreeterService : IGreeterService
    {
        private readonly INameProvider _nameProvider;

        public ConsoleUserGreeterService(INameProvider nameProvider)
        {
            _nameProvider = nameProvider;
        }

        public void Greet(string username = "")
        {
            if (string.IsNullOrEmpty(username))
            {
                username = _nameProvider.ReadUserName();
            }
            var userGreeting = DatetimeUserGreetingConcatenator.Concatenate($"Hello, {username}!");
            _nameProvider.WriteUserName(userGreeting);
        }
    }
}
