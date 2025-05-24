using System.Windows;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Windows.Controls;
using Newtonsoft.Json.Linq;

namespace Final
{
    public partial class Activities : Window
    {
        public Activities()
        {
            InitializeComponent();
        }

        private async void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string destination = DestinationTextBox.Text.Trim();
            if (string.IsNullOrWhiteSpace(destination))
            {
                MessageBox.Show("Please enter a destination.");
                return;
            }

            ActivitiesListBox.Items.Clear();
            ActivitiesListBox.Items.Add("Loading...");

            try
            {
                var activities = await GetActivitiesAsync(destination);
                ActivitiesListBox.Items.Clear();

                foreach (var activity in activities)
                {
                    ActivitiesListBox.Items.Add(activity);
                }

                if (activities.Count == 0)
                {
                    ActivitiesListBox.Items.Add("No activities found.");
                }
            }
            catch
            {
                ActivitiesListBox.Items.Clear();
                ActivitiesListBox.Items.Add("Failed to load activities.");
            }
        }

        private async Task<List<string>> GetActivitiesAsync(string destination)
        {
            List<string> results = new List<string>();

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("X-RapidAPI-Key", "e5f65929a5mshe3264eb6a810caep120bb5jsndf623e4e3242");
                client.DefaultRequestHeaders.Add("X-RapidAPI-Host", "booking-com.p.rapidapi.com");

                var url = $"https://booking-com.p.rapidapi.com/v1/activities/search?location={destination}&language=en-us";

                HttpResponseMessage response = await client.GetAsync(url);
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    var data = JObject.Parse(json);

                    foreach (var item in data["results"])
                    {
                        string title = item["title"]?.ToString();
                        if (!string.IsNullOrEmpty(title))
                            results.Add(title);
                    }
                }
            }

            return results;
        }
    }
}
