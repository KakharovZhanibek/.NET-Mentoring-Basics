using HelloUserLibrary.Interfaces;
using HelloUserMAUIApp.Pages;
using HelloUserMAUIApp.Services;
using Microsoft.Extensions.Logging;

namespace HelloUserMAUIApp
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            builder.Services.AddSingleton<IMauiUserNameSource, MauiUserNameSource>();
            builder.Services.AddSingleton<IMauiGreetingWindowPresenter, MauiGreetingWindowPresenter>();
            builder.Services.AddSingleton<INameProvider, MauiNameProvider>();
            builder.Services.AddSingleton<IGreeterService, MauiUserGreeterService>();
            builder.Services.AddTransient<InputPage>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
