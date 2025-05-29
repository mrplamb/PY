using System.Windows;
using System.Windows.Controls;
using Final.Services;
using HolidayManagerWeb.Models;

namespace Final
{
    public partial class BankInfo : Window
    {
        public BankInfo()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string fullName = FullNameTextBox.Text.Trim();
            string iban = IbanTextBox.Text.Trim();
            string bic = BicTextBox.Text.Trim();
            string bankName = BankNameTextBox.Text.Trim();

            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(iban) ||
                string.IsNullOrEmpty(bic) || string.IsNullOrEmpty(bankName))
            {
                MessageBox.Show("Please fill in all fields.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // You can optionally store this in AppState or database
            MessageBox.Show($"Saved Finance Info:\n\n" +
                            $"Full Name: {fullName}\n" +
                            $"IBAN: {iban}\n" +
                            $"BIC: {bic}\n" +
                            $"Bank Name: {bankName}",
                            "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void FullNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void IbanTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BicTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void BankNameTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

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

            private void PayNow_Click(object sender, RoutedEventArgs e)
        {
                PaymentWindow paymentWindow = new PaymentWindow();
                paymentWindow.ShowDialog();
        }

        private void Transactions_Click(object sender, RoutedEventArgs e)
        {
            BankTransactions bankTransactionsPage = new BankTransactions();
            bankTransactionsPage.Show();
            this.Close(); // Close BankInfo if you want to keep only one window open
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
