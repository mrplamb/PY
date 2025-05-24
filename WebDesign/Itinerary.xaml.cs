using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Final
{
    public partial class Itinerary : Window
    {
        private readonly HttpClient httpClient = new HttpClient();

        public Itinerary()
        {
            InitializeComponent();
        }

        private async void GetItinerary_Click(object sender, RoutedEventArgs e)
        {
            string destination = DestinationTextBox.Text.Trim();

            if (string.IsNullOrEmpty(destination))
            {
                MessageBox.Show("Please enter a destination.", "Input Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Step 1: Get destination ID
                string destId = await GetDestinationId(destination);
                if (string.IsNullOrEmpty(destId))
                {
                    MessageBox.Show("Destination not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Step 2: Get hotel suggestions
                string hotelInfo = await GetHotelSuggestions(destId);
                ItineraryTextBlock.Text = hotelInfo;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async Task<string> GetDestinationId(string destination)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com.p.rapidapi.com/v1/hotels/locations?name={Uri.EscapeDataString(destination)}&locale=en-us"),
                Headers =
                {
                    { "X-RapidAPI-Key", "e5f65929a5mshe3264eb6a810caep120bb5jsndf623e4e3242" },
                    { "X-RapidAPI-Host", "booking-com.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(body);
                var root = jsonDoc.RootElement;

                if (root.GetArrayLength() > 0)
                {
                    var firstResult = root[0];
                    if (firstResult.TryGetProperty("dest_id", out JsonElement destIdElement))
                    {
                        return destIdElement.GetString();
                    }
                }
            }

            return null;
        }

        private async Task<string> GetHotelSuggestions(string destId)
        {
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://booking-com.p.rapidapi.com/v1/hotels/search?dest_id={Uri.EscapeDataString(destId)}&dest_type=city&order_by=popularity&locale=en-us&checkin_date=2025-06-01&checkout_date=2025-06-02&adults_number=1&units=metric&room_number=1"),
                Headers =
                {
                    { "X-RapidAPI-Key", "YOUR_RAPIDAPI_KEY" },
                    { "X-RapidAPI-Host", "booking-com.p.rapidapi.com" },
                },
            };

            using (var response = await httpClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var body = await response.Content.ReadAsStringAsync();
                var jsonDoc = JsonDocument.Parse(body);
                var root = jsonDoc.RootElement;

                if (root.TryGetProperty("result", out JsonElement results) && results.GetArrayLength() > 0)
                {
                    var firstHotel = results[0];
                    string hotelName = firstHotel.GetProperty("hotel_name").GetString();
                    string address = firstHotel.GetProperty("address").GetString();
                    double price = firstHotel.GetProperty("min_total_price").GetDouble();
                    string currency = firstHotel.GetProperty("currency_code").GetString();

                    return $"Hotel Name: {hotelName}\nAddress: {address}\nPrice: {price} {currency}";
                }
                else
                {
                    return "No hotels found for the selected destination.";
                }
            }
        }
    }
}
