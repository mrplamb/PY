using System;
using System.Windows;
using HolidayManagerWeb.Models;

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
            string name = FullNameTextBox.Text;
            string email = EmailTextBox.Text;
            string password = "temporaryPassword123"; // Replace with real password flow
            DateOnly birth = DobDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DobDatePicker.SelectedDate.Value)
                : DateOnly.MinValue;

            var user = new User(name, email, password, birth)
            {
                WalletBalance = 0
            };

            AppState.CurrentUser = user;

            MessageBox.Show($"Saved Personal Info:\n\n" +
                            $"Name: {user.Name}\n" +
                            $"DOB: {user.Birth}\n" +
                            $"Email: {user.Email}",
                            "Success", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void GenderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            // Optional: handle gender selection if needed
        }

        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: live validation
        }

        private void AddressTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // Optional: live validation
        }
    }
}
