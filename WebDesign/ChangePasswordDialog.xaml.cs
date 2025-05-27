using System.Windows;

namespace Final
{
    public partial class ChangePasswordDialog : Window
    {
        public string CurrentPassword { get; private set; }
        public string NewPassword { get; private set; }

        public ChangePasswordDialog()
        {
            InitializeComponent();
        }

        private void Change_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CurrentPasswordBox.Password) ||
                string.IsNullOrWhiteSpace(NewPasswordBox.Password) ||
                string.IsNullOrWhiteSpace(ConfirmPasswordBox.Password))
            {
                MessageBox.Show("All fields are required.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (NewPasswordBox.Password != ConfirmPasswordBox.Password)
            {
                MessageBox.Show("New passwords do not match.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            CurrentPassword = CurrentPasswordBox.Password;
            NewPassword = NewPasswordBox.Password;

            DialogResult = true; // Close dialog with OK result
            Close();
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false; // Close dialog with Cancel result
            Close();
        }
    }
}
