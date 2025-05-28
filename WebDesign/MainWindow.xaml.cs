using System;
using System.Linq;
using System.Windows;
using HolidayManagerWeb;
using System.Windows.Controls;
using HolidayManagerWeb.Models;

namespace Final
{
    public partial class MainWindow : Window
    {
        private readonly AppDbContext _db;

        public MainWindow()
        {
            InitializeComponent();
            _db = new AppDbContext();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            string email = EmailTextBox.Text.Trim();
            string password = PasswordBox.Password;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please fill in both email and password.");
                return;
            }

            if (_db.Users.Any(u => u.Email == email))
            {
                MessageBox.Show("An account with this email already exists.");
                return;
            }

            var user = new User("", email, password, DateOnly.FromDateTime(DateTime.Today));
            _db.Users.Add(user);
            _db.SaveChanges();

            MessageBox.Show("Account created. You can now log in.");
        }

        private void Login_Click(object sender, RoutedEventArgs e)
        {
            LoginPage loginPage = new LoginPage();
            loginPage.Show();
            this.Close(); // Close the current sign-up window
        }

        private void EmailTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            // Optional real-time validation or feedback for sign-up email
        }


    }


}