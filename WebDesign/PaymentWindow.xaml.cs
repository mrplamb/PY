using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using Final.Services;
using HolidayManagerWeb;
using HolidayManagerWeb.Models;

namespace Final
{
    public partial class PaymentWindow : Window
    {
        private readonly PayPalService _payPalService;
        private readonly AppDbContext _db = new AppDbContext();

        public int TripId { get; set; }

        public PaymentWindow()
        {
            InitializeComponent();
            _payPalService = new PayPalService();
            TripId = TripId;
            _db = new AppDbContext();
            LoadTrips();
        }


        private async void PayWithPayPal_Click(object sender, RoutedEventArgs e)
        {
            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount) || amount <= 0)
            {
                MessageBox.Show("Please enter a valid payment amount.", "Invalid Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                string approvalUrl = await _payPalService.CreateOrderAsync(amount, "EUR");

                // Open in default browser
                Process.Start(new ProcessStartInfo(approvalUrl) { UseShellExecute = true });

                MessageBox.Show("Redirecting to PayPal for payment...", "Info", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close(); // Optionally close the PaymentWindow
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating PayPal order: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close(); // Close PaymentWindow to return to HomePage
        }

        private async void CapturePayment_Click(object sender, RoutedEventArgs e)
        {
            // Prompt the user for the order ID
            string orderId = Microsoft.VisualBasic.Interaction.InputBox("Please enter the PayPal Order ID:", "Capture Payment", "");

            if (string.IsNullOrWhiteSpace(orderId))
            {
                MessageBox.Show("Order ID is required.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                PayPalService payPalService = new PayPalService();
                string response = await payPalService.CaptureOrderAsync(orderId);

                CaptureResultTextBlock.Text = "Payment captured successfully!";
                CaptureResultTextBlock.Foreground = Brushes.Green;

                MessageBox.Show("Payment captured!\n\n" + response, "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                CaptureResultTextBlock.Text = "Failed to capture payment.";
                CaptureResultTextBlock.Foreground = Brushes.Red;
                MessageBox.Show("Error capturing payment:\n\n" + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void LoadTrips()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User not logged in.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var trips = _db.Trips
                .Where(t => t.UserId == AppState.CurrentUser.ID)
                .OrderBy(t => t.StartDate)
                .ToList();

            TripComboBox.ItemsSource = trips;
        }

        private void PayNow_Click(object sender, RoutedEventArgs e)
        {
            if (TripComboBox.SelectedItem is Trip selectedTrip)
            {
                MessageBox.Show($"Proceeding to pay for trip to {selectedTrip.Destination}", "Payment Info", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a trip before proceeding to payment.", "No Trip Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}
