using System.Windows;
using HelloUserLibrary.Interfaces;

namespace HelloUserWpfApp
{
    internal class WpfNameProvider : INameProvider
    {

        private readonly Window _owner;

        public string LastGreeting { get; private set; }

        public WpfNameProvider(Window owner)
        {
            _owner = owner;
        }

        public void WriteUserName(string name)
        {
            // Открываем GreetingWindow как диалоговое окно
            var greetingWindow = new GreetingWindow(name)
            {
                Owner = _owner
            };
            greetingWindow.ShowDialog();
        }

        public string ReadUserName()
        {
            // Получаем имя из текстбокса MainWindow
            if (_owner is MainWindow mw)
                return mw.UsernameTextBox.Text;
            return string.Empty;
        }

        //public void WriteUserName0(string name)
        //{
        //    MessageBox.Show(name, "Сообщение");
        //}

        //public string ReadUserName0()
        //{
        //    // Реализуйте окно для ввода имени, например, InputDialog
        //    var dialog = new MainWindow("Введите имя:");
        //    return dialog.ShowDialog() == true ? dialog.ResponseText : string.Empty;
        //}
    }
}
