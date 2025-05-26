using System.Linq;
using System.Windows;
using HolidayManagerWeb;
using HolidayManagerWeb.Models;

namespace Final
{
    public partial class TripDetails : Window
    {
        private readonly Trip _trip;

        public TripDetails(Trip trip)
        {
            InitializeComponent();

            _trip = trip; // Save the trip as a private field!

            DestinationTextBlock.Text = $"Destination: {trip.Destination}";
            DatesTextBlock.Text = $"Dates: {trip.StartDate} to {trip.EndDate}";
            BudgetTextBlock.Text = $"Budget: €{trip.Budget}";
            StatusTextBlock.Text = $"Status: {(trip.IsPaid ? "Paid" : "Unpaid")}";
            ActivitiesTextBlock.Text = $"Planned Activities: {trip.PlannedActivities ?? "N/A"}";
            ItemsTextBlock.Text = $"Items to Take: {trip.ItemsToTake ?? "N/A"}";
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var tripToDelete = db.Trips.FirstOrDefault(t => t.TripId == _trip.TripId);
                if (tripToDelete != null)
                {
                    db.Trips.Remove(tripToDelete);
                    db.SaveChanges();
                    MessageBox.Show("Trip deleted.");
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Trip not found in database.");
                }
            }
        }
    }
}
