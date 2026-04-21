namespace HelloUserMAUIApp.Pages;

public partial class OutputPage : ContentPage
{
    public OutputPage(string greetingText)
    {
        InitializeComponent();
        GreetingLabel.Text = greetingText;
    }
}
