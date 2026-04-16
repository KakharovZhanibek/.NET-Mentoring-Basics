using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HelloUserLibrary;

namespace HelloUserWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnSayHello(object sender, RoutedEventArgs e)
        {
            var username = UsernameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(username))
            {
                username = "User";
            }

            UserGreetings userGreetingsWindow = new UserGreetings();
            var greetingMessage = $"Hello, {username}!";

            userGreetingsWindow.UserGreetingLabel.Text = DatetimeUserGreetingConcatenator.Concatenate(greetingMessage);
            UsernameTextBox.Text = string.Empty;
            userGreetingsWindow.ShowDialog();
        }
    }
}
