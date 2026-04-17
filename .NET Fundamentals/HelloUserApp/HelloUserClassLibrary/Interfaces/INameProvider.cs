namespace HelloUserLibrary.Interfaces
{
    public interface INameProvider
    {
        void WriteUserName(string userName);

        string ReadUserName();
    }
}
