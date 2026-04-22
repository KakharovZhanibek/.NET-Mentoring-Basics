namespace HelloUserMAUIApp.Services;

public sealed class MauiUserNameSource : IMauiUserNameSource
{
    private string _userName = string.Empty;

    public string GetUserName() => _userName;

    public void SetUserName(string userName) => _userName = userName ?? string.Empty;
}
