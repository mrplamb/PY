using System.Windows;
using System.Windows.Controls;

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

       
    }
}
