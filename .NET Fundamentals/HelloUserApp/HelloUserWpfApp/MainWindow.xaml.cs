using System.Windows;
using System.Windows.Controls;
using HelloUserLibrary.Interfaces;

namespace HelloUserWpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IGreeterService _greeter;

        public MainWindow()
        {
            InitializeComponent();
            var nameProvider = new WpfNameProvider(this);
            _greeter = new WpfUserGreeterService(nameProvider);
        }

        private void SayHelloButton_Click(object sender, RoutedEventArgs e)
        {
            _greeter.Greet();
        }

        private void UsernameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            SayHelloButton.IsEnabled = !string.IsNullOrWhiteSpace(UsernameTextBox.Text);
        }
    }
}
