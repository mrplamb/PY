using System.Windows;

namespace Final
{
    public partial class Verification : Window
    {
        private string expectedCode;

        public Verification(string verificationCode)
        {
            InitializeComponent();
            expectedCode = verificationCode;
        }

        private void Verify_Click(object sender, RoutedEventArgs e)
        {
            string enteredCode = $"{Code1.Text}{Code2.Text}{Code3.Text}{Code4.Text}";

            if (enteredCode == expectedCode)
            {
                MessageBox.Show("Verification successful!");
                // Proceed to the main app window
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
