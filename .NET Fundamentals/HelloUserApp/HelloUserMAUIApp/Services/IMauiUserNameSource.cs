namespace HelloUserMAUIApp.Services;

/// <summary>
/// MAUI-specific abstraction for the username entered in the input window.
/// Keeps <see cref="HelloUserLibrary.Interfaces.INameProvider"/> free of UI types.
/// </summary>
public interface IMauiUserNameSource
{
    string GetUserName();

    void SetUserName(string userName);
}
