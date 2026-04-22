using HelloUserLibrary.Interfaces;
using HelloUserMAUIApp.Services;

namespace HelloUserMAUIApp.Pages;

public partial class InputPage : ContentPage
{
    private readonly IGreeterService _greeterService;
    private readonly IMauiUserNameSource _userNameSource;

    public InputPage(IGreeterService greeterService, IMauiUserNameSource userNameSource)
    {
        InitializeComponent();
        _greeterService = greeterService;
        _userNameSource = userNameSource;
    }

    private void OnUsernameTextChanged(object? sender, TextChangedEventArgs e)
    {
        _userNameSource.SetUserName(e.NewTextValue ?? string.Empty);
        GreetButton.IsEnabled = !string.IsNullOrWhiteSpace(e.NewTextValue);
    }

    private void OnGreetClicked(object? sender, EventArgs e)
    {
        _greeterService.Greet();
    }
}
