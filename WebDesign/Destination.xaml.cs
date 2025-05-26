using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Newtonsoft.Json.Linq;

namespace Final
{
    public partial class DestinationPage : Page
    {
        private readonly HttpClient _httpClient = new HttpClient();
        private DispatcherTimer _typingTimer;

        public DestinationPage()
        {
            InitializeComponent();

            _typingTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromMilliseconds(500)
            };
            _typingTimer.Tick += TypingTimer_Tick;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            string destination = (DestinationComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? DestinationComboBox.Text;
            DateTime? fromDate = FromDatePicker.SelectedDate;
            DateTime? toDate = ToDatePicker.SelectedDate;
            string budget = BudgetTextBox.Text;
            string planning = PlanningTextBox.Text;
            string items = ItemsTextBox.Text;

            MessageBox.Show($"Destination: {destination}\nFrom: {fromDate:d}\nTo: {toDate:d}\nBudget: {budget}\n\nPlanning:\n{planning}\n\nItems:\n{items}",
                            "Trip Details Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void DestinationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Optional: logic when user selects from dropdown
        }

        private void BudgetTextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void PlanningTextBox_TextChanged(object sender, TextChangedEventArgs e) { }
        private void ItemsTextBox_TextChanged(object sender, TextChangedEventArgs e) { }

        private void DestinationComboBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _typingTimer.Stop();
            _typingTimer.Tag = DestinationComboBox.Text;
            _typingTimer.Start();
        }

        private void DestinationComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            if (comboBox?.Template.FindName("PART_EditableTextBox", comboBox) is TextBox textBox)
            {
                textBox.TextChanged += DestinationComboBox_TextChanged;
            }
        }


        private async void TypingTimer_Tick(object sender, EventArgs e)
        {
            _typingTimer.Stop();
            string query = _typingTimer.Tag.ToString();

            if (!string.IsNullOrWhiteSpace(query) && query.Length >= 3)
            {
                await FetchDestinationSuggestions(query);
            }
        }

        private async Task FetchDestinationSuggestions(string query)
        {
            try
            {
                string url = $"https://wft-geo-db.p.rapidapi.com/v1/geo/cities?namePrefix={Uri.EscapeDataString(query)}&limit=5";

                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(url),
                    Headers =
                    {
                        { "x-rapidapi-key", "e5f65929a5mshe3264eb6a810caep120bb5jsndf623e4e3242" },
                        { "x-rapidapi-host", "wft-geo-db.p.rapidapi.com" },
                    },
                };

                using (var response = await _httpClient.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    var body = await response.Content.ReadAsStringAsync();

                    JObject json = JObject.Parse(body);
                    var cities = json["data"];

                    DestinationComboBox.Items.Clear();

                    foreach (var city in cities)
                    {
                        string cityName = $"{city["city"]}, {city["country"]}";
                        DestinationComboBox.Items.Add(new ComboBoxItem { Content = cityName });
                    }

                    DestinationComboBox.IsDropDownOpen = true;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error fetching destinations: {ex.Message}", "API Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
