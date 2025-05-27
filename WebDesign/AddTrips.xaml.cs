using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using HolidayManagerWeb;
using static Final.Models.GeoDBCity;


namespace Final
{
    public partial class AddTrips : Window
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private DispatcherTimer _typingTimer;

        public AddTrips()
        {
            InitializeComponent();
            InitializeTypingTimer();
        }

        private void InitializeTypingTimer()
        {
            _typingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(1000)
            };
            _typingTimer.Tick += TypingTimer_Tick;
        }

        private void DestinationComboBox_PreviewKeyUp(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            _typingTimer.Start();
        }

        private async void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            string query = DestinationComboBox.Text;
            if (!string.IsNullOrWhiteSpace(query))
            {
                await LoadDestinationSuggestionsAsync(query);
            }
        }

        private async Task LoadDestinationSuggestionsAsync(string query)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri("https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefix={query}&limit=10"),
                    Headers =
                    {
                        { "x-rapidapi-key", "a189e63b7dmshc52a3a69f1614bep1984e5jsnd4fb2434ca85" },
                        { "x-rapidapi-host", "wft-geo-db.p.rapidapi.com" },
                    },
                };

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var cityResponse = JsonSerializer.Deserialize<GeoDBCityResponse>(json, options);
                var suggestions = cityResponse.Data
                    .Select(c => $"{c.City}, {c.Country}")
                    .ToList();

                foreach (var city in cityResponse.Data)
                {
                    if (!string.IsNullOrWhiteSpace(city.City) && !string.IsNullOrWhiteSpace(city.Country))
                    {
                        suggestions.Add($"{city.City}, {city.Country}");
                    }
                }

                DestinationComboBox.ItemsSource = suggestions;
                DestinationComboBox.IsDropDownOpen = true;
            }

            catch (HttpRequestException ex)
            {
                if (ex.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    MessageBox.Show("API limit reached. Please try again later.");
                }
                else
                {
                    MessageBox.Show($"Error: {ex.Message}");
                }
            }

            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching destinations: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public class Destination
        {
            public string Name { get; set; }
            public string Country { get; set; }
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

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Input validation
                if (DestinationComboBox.Text.Trim() == "" ||
                    StartDatePicker.SelectedDate == null ||
                    EndDatePicker.SelectedDate == null ||
                    string.IsNullOrWhiteSpace(BudgetTextBox.Text))
                {
                    MessageBox.Show("Please fill in all fields (destination, dates, budget).");
                    return;
                }

                // Parse budget
                if (!decimal.TryParse(BudgetTextBox.Text.Trim(), out decimal budget))
                {
                    MessageBox.Show("Invalid budget format. Please enter a valid number.");
                    return;
                }

                // Create new trip
                var trip = new HolidayManagerWeb.Models.Trip
                {
                    UserId = AppState.CurrentUser.ID, // Current logged-in user
                    Destination = DestinationComboBox.Text.Trim(),
                    StartDate = DateOnly.FromDateTime(StartDatePicker.SelectedDate.Value),
                    EndDate = DateOnly.FromDateTime(EndDatePicker.SelectedDate.Value),
                    Budget = budget,
                    ItemsToTake = ItemsTextBox.Text.Trim(),
                    PlannedActivities = ActivitiesTextBox.Text.Trim(),
                    Status = "Planned",
                    IsPaid = false
                };

                // Save to DB
                using (var db = new AppDbContext())
                {
                    db.Trips.Add(trip);
                    db.SaveChanges();
                }

                MessageBox.Show("Trip saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving trip: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
