using System.Diagnostics; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input; 
using HolidayManagerWeb.Models; 
using System; 

namespace Final 
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
                
                if (textBlock.Tag is string url && Uri.IsWellFormedUriString(url, UriKind.Absolute))
                {
                    try
                    {
                        
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
        
    }
}