using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HolidayManagerWeb;
using HolidayManagerWeb.Models;

namespace Final
{
    public partial class TripsPage : Window
    {
        private readonly AppDbContext _db;

        public TripsPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadUserTrips();
        }

        private void LoadUserTrips()
        {
            if (AppState.CurrentUser is null)
            {
                MessageBox.Show("User session not found.");
                this.Close();
                return;
            }

            List<Trip> trips = _db.Trips
                .Where(t => t.UserId == AppState.CurrentUser!.ID)
                .ToList();

            foreach (var trip in trips)
            {
                Button tripButton = new Button
                {
                    Content = trip.Destination + " (" + trip.StartDate.ToShortDateString() + ")",
                    Margin = new Thickness(0, 5, 0, 5),
                    Style = (Style)this.FindResource("RoundedButtonStyle")
                };
                TripButtonPanel.Children.Add(tripButton);
            }
        }

        private void AddTripButton_Click(object sender, RoutedEventArgs e)
        {
            var addTripWindow = new AddTrips();
            addTripWindow.ShowDialog();
            TripButtonPanel.Children.Clear();
            LoadUserTrips(); // reload after adding a trip
        }
    }
}
