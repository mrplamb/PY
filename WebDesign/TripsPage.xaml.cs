using System.Windows;

namespace Final
{
    public partial class TripsPage : Window
    {
        public TripsPage()
        {
            InitializeComponent();
        }

        private void AddTripButton_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Add New Trip clicked!");
            // Logic for adding a new trip can be added here.
        }
    }
}
