using System.Windows;
using System.Windows.Controls;

namespace Final
{
    public partial class Main : Window
    {
        public Main()
        {
            InitializeComponent();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"Signing up with: {EmailTextBox.Text}");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Logging in...");
        }
        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Example: Show the current text in the debug output
            var textbox = sender as TextBox;
            Console.WriteLine($"Email changed: {textbox.Text}");
        }

    }
}
