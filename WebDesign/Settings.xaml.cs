using System.Windows;

namespace Final

{

    public partial class Settings : Window

    {

        public Settings()

        {

            InitializeComponent();

        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)

        {

            // Show dialog to change password

            MessageBox.Show("Change Password logic here!");

        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)

        {

            var result = MessageBox.Show("Are you sure you want to delete your account? This action is irreversible.",

                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)

            {

                // Call delete account logic here

                MessageBox.Show("Account deleted.");

                // Optionally: log out the user and close app or redirect to login

            }

        }

        private void UpdateProfilePicture_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Update profile picture logic here!");

        }

        private void UpdatePersonalInfo_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Update personal info logic here!");

        }

        private void NotificationPreferences_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Notification preferences logic here!");

        }

        private void Appearance_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Appearance / theme logic here!");

        }

        private void PaymentMethods_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Payment methods logic here!");

        }

        private void LanguageCurrency_Click(object sender, RoutedEventArgs e)

        {

            MessageBox.Show("Language and currency logic here!");

        }

    }

}

