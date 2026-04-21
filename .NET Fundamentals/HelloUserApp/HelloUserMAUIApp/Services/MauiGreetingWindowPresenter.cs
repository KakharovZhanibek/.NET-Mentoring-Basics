using HelloUserMAUIApp.Pages;

namespace HelloUserMAUIApp.Services;

public sealed class MauiGreetingWindowPresenter : IMauiGreetingWindowPresenter
{
    public void Present(string greetingText)
    {
        var app = Application.Current;
        if (app is null)
            return;

        var outputPage = new OutputPage(greetingText);
        var window = new Window(outputPage)
        {
            Title = "Output",
            Width = 550,
            Height = 450
        };

        app.OpenWindow(window);
    }
}
