using System.Diagnostics; // This is needed for Process.Start
using System.Windows;
using System.Windows.Controls; // This is for TextBlock
using System.Windows.Input; // This is for MouseButtonEventArgs
using HolidayManagerWeb.Models; // Assuming your models (CitySuggestion, PlaceDetail) are here
using System; // Needed for Uri, UriKind

namespace Final // Make sure this namespace matches your project
{
    public partial class CityActivitiesWindow : Window
    {
        private CitySuggestion _citySuggestion;

        public CityActivitiesWindow(CitySuggestion citySuggestion)
        {
            InitializeComponent();
            _citySuggestion = citySuggestion;
            CityNameTextBlock.Text = $"Activities for {_citySuggestion.CityName}";
            PlacesToVisitList.ItemsSource = _citySuggestion.PlacesToVisit;
            PlacesToEatList.ItemsSource = _citySuggestion.PlacesToEat;
        }
        private void LinkTextBlock_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (sender is TextBlock textBlock)
            {
                // Ensure that 'url' is extracted as a string from the Tag property
                if (textBlock.Tag is string url && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    try
                    {
                        // Use Process.Start to open the URL in the default browser
                        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Could not open link: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show("Invalid or missing URL for this item.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
        }
        // ---------------------------------------------------------------
        // END OF THE METHOD
        // ---------------------------------------------------------------
    }
}