using HelloUserLibrary.Interfaces;

namespace HelloUserMAUIApp.Services;

public sealed class MauiNameProvider : INameProvider
{
    private readonly IMauiUserNameSource _userNameSource;
    private readonly IMauiGreetingWindowPresenter _greetingWindowPresenter;

    public MauiNameProvider(
        IMauiUserNameSource userNameSource,
        IMauiGreetingWindowPresenter greetingWindowPresenter)
    {
        _userNameSource = userNameSource;
        _greetingWindowPresenter = greetingWindowPresenter;
    }

    public string ReadUserName() => _userNameSource.GetUserName();

    public void WriteUserName(string userName) => _greetingWindowPresenter.Present(userName);
}
