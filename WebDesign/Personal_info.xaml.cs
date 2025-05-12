using System.Windows;
using System.Windows.Controls;

namespace Final
{
    public partial class Personal_info : Window
    {
        public Personal_info()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            var user = new User
            {
                FullName = FullNameTextBox.Text,
                DateOfBirth = DobDatePicker.SelectedDate,
                Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString(),
                Email = EmailTextBox.Text,
                PhoneNumber = PhoneTextBox.Text,
                Address = AddressTextBox.Text
            };

            AppState.CurrentUser = user;

            MessageBox.Show($"Saved Personal Info:\n\n" +
                            $"Name: {user.FullName}\n" +
                            $"DOB: {user.DateOfBirth?.ToShortDateString() ?? "Not Selected"}\n" +
                            $"Gender: {user.Gender}\n" +
                            $"Email: {user.Email}\n" +
                            $"Phone: {user.PhoneNumber}\n" +
                            $"Address: {user.Address}",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }


        private void GenderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void PhoneTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void AddressTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        
    }
}
