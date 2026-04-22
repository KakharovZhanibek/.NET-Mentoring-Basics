using HelloUserMAUIApp.Pages;

namespace HelloUserMAUIApp
{
    public partial class App : Application
    {
        public App(IServiceProvider services)
        {
            Services = services;
            InitializeComponent();
        }

        public IServiceProvider Services { get; }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var inputPage = Services.GetRequiredService<InputPage>();
            var window = new Window(inputPage) 
            { 
                Title = "Input",
                Width = 650,
                Height = 500
            };
            return window;
        }
    }
}