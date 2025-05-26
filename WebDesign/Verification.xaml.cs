using System.Windows;

namespace Final
{
    public partial class Verification : Window
    {
        private string expectedCode;

        public Verification()
        {
            InitializeComponent();
            expectedCode = "";
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            // Combine the 4 code input fields into one string
            string enteredCode = $"{Code1.Text}{Code2.Text}{Code3.Text}{Code4.Text}".Trim();

            // Check if all boxes were filled
            if (enteredCode.Length != 4)
            {
                MessageBox.Show("Please enter the complete 4-digit verification code.");
                return;
            }

            // Check if the entered code matches the expected one
            if (enteredCode == expectedCode)
            {
                MessageBox.Show("Verification successful!");

                // Proceed to main app window
                var tripsPage = new TripsPage();
                tripsPage.Show();

                this.Close(); // Close the verification window
            }
            else
            {
                MessageBox.Show("Incorrect code. Please try again.");
            }
        }


        private void ResendCode_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("A new code has been sent to your email.");
            // You could regenerate and resend here
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // or navigate to previous window
        }
    }
}
