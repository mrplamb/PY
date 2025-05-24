using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;


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
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _typingTimer.Tick += TypingTimer_Tick;
        }

        private void DestinationComboBox_TextChanged(object sender, TextChangedEventArgs e)
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
                    RequestUri = new Uri("https://booking-com15.p.rapidapi.com/api/v1/cars/searchCarRentals?pick_up_latitude=40.6397018432617&pick_up_longitude=-73.7791976928711&drop_off_latitude=40.6397018432617&drop_off_longitude=-73.7791976928711&pick_up_time=10%3A00&drop_off_time=10%3A00&driver_age=30&currency_code=USD&location=US"),
                    Headers =
                    {
                        { "x-rapidapi-key", "e5f65929a5mshe3264eb6a810caep120bb5jsndf623e4e3242" },
                        { "x-rapidapi-host", "booking-com15.p.rapidapi.com" },
                    },
                };

                using var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var json = await response.Content.ReadAsStringAsync();
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var destinations = JsonSerializer.Deserialize<List<Destination>>(json, options);

                var suggestions = new List<string>();
                foreach (var dest in destinations)
                {
                    if (!string.IsNullOrWhiteSpace(dest.Name) && !string.IsNullOrWhiteSpace(dest.Country))
                    {
                        suggestions.Add($"{dest.Name}, {dest.Country}");
                    }
                }

                DestinationComboBox.ItemsSource = suggestions;
                DestinationComboBox.IsDropDownOpen = true;
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
    }
}
