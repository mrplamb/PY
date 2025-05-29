using System.Runtime.CompilerServices;
using System.Windows;
using HolidayManagerWeb;

namespace Final
{
    public partial class Settings : Window
    {
        private readonly AppDbContext _db;
        public Settings()
        {
            InitializeComponent();
            _db = new AppDbContext();
        }

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            HomePage homePage = new HomePage(); // Replace with your actual home page class
            homePage.Show();
            this.Close();
        }

        private void ChangePassword_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Show a custom dialog to get current & new password
            ChangePasswordDialog dialog = new ChangePasswordDialog();
            if (dialog.ShowDialog() == true)
            {
                string currentPassword = dialog.CurrentPassword;
                string newPassword = dialog.NewPassword;

                // Verify current password
                var user = _db.Users.FirstOrDefault(u => u.ID == AppState.CurrentUser.ID);
                if (user == null)
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
                {
                    MessageBox.Show("Current password is incorrect.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Update password
                user.SetPassword(newPassword);
                _db.SaveChanges();

                MessageBox.Show("Password updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void DeleteAccount_Click(object sender, RoutedEventArgs e)
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var result = MessageBox.Show("Are you sure you want to delete your account? This action is irreversible.",
                "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                var user = _db.Users.FirstOrDefault(u => u.ID == AppState.CurrentUser.ID);
                if (user != null)
                {
                    _db.Users.Remove(user);
                    _db.SaveChanges();

                    MessageBox.Show("Account deleted. Goodbye!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);

                    // Log out the user
                    AppState.CurrentUser = null;

                    // Optionally: Close app or show login window
                    Application.Current.Shutdown();
                }
                else
                {
                    MessageBox.Show("User not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void PaymentMethods_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Payment methods logic here!");
        }

        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            Personal_info personalInfoPage = new Personal_info();
            personalInfoPage.Show(); // Show the new window

        }

        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            TripsPage tripsPage = new TripsPage();
            tripsPage.Show(); // Show the new window

        }

        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            BankInfo bankInfo = new BankInfo();
            bankInfo.Show();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            UploadDocumentPage documents = new UploadDocumentPage();
            documents.Show();
        }

        private void Dashboarding_Click(object sender, RoutedEventArgs e)
        {
            DashboardPage dashboard = new DashboardPage();
            dashboard.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppState.CurrentUser = null;

                MainWindow loginPage = new MainWindow();
                loginPage.Show();

                this.Close();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppState.CurrentUser = null;

                MainWindow loginPage = new MainWindow();
                loginPage.Show();

                this.Close();
            }
        }
    }
}
