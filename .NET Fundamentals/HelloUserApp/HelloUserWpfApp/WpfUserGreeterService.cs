using HelloUserLibrary;
using HelloUserLibrary.Interfaces;

namespace HelloUserWpfApp
{
    public class WpfUserGreeterService : IGreeterService
    {
        private readonly INameProvider _nameProvider;

        public WpfUserGreeterService(INameProvider nameProvider)
        {
            _nameProvider = nameProvider;
        }

        public void Greet(string username = "")
        {
            string name = _nameProvider.ReadUserName();
            string userGreeting = DatetimeUserGreetingConcatenator.Concatenate($"Hello, {name}!");
            _nameProvider.WriteUserName(userGreeting);
        }
    }
}
