namespace HelloUserMAUIApp.Services;

/// <summary>
/// Opens a secondary window to show the composed greeting.
/// </summary>
public interface IMauiGreetingWindowPresenter
{
    void Present(string greetingText);
}
