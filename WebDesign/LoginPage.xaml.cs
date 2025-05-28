using System;
using System.Linq;
using System.Windows;
using HolidayManagerWeb; // Assuming AppDbContext and AppState are here
using HolidayManagerWeb.Models; // Assuming your User model is here

namespace Final
{
    public partial class LoginPage : Window
    {
        private readonly AppDbContext _db;

        public LoginPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both email and password.");
                return;
            }

            var user = _db.Users.FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                MessageBox.Show("Invalid email.");
                return;
            }

            // Verify the password using BCrypt.Net.BCrypt.Verify
            // Make sure you have the BCrypt.Net-Next NuGet package installed
            // You should have already installed it for your SignUp_Click
            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                MessageBox.Show("Incorrect password.");
                return;
            }

            // Successful login
            AppState.CurrentUser = user; // Set the current logged-in user
            MessageBox.Show("Login successful!");

            // Navigate to the main application page (e.g., HomePage, Dashboard)
            HomePage homepage = new HomePage(); // Replace HomePage with your actual main app window
            homepage.Show();
            this.Close(); // Close the login window
        }

        private void CreateAccount_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to the main SignUp window
            MainWindow signupPage = new MainWindow();
            signupPage.Show();
            this.Close(); // Close the current login window
        }
    }
}