using HelloUserLibrary;
using HelloUserLibrary.Interfaces;

namespace HelloUserMAUIApp.Services;

public sealed class MauiUserGreeterService : IGreeterService
{
    private readonly INameProvider _nameProvider;

    public MauiUserGreeterService(INameProvider nameProvider)
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
