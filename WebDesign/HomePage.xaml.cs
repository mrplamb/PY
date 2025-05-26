using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Reflection.Metadata;

namespace Final
{
    public partial class HomePage : Window
    {
        private List<string> _imagePaths = new List<string>
        {
            "Views/SlideShow2.jpeg"
        };
        private int _currentImageIndex = 0;
        private DispatcherTimer _timer;

        public HomePage()
        {
            InitializeComponent();
            StartSlideshow();
        }

        private void StartSlideshow()
        {
            if (_imagePaths.Count == 0) return;

            _timer = new DispatcherTimer();
            _timer.Interval = System.TimeSpan.FromSeconds(3);
            _timer.Tick += Timer_Tick;
            _timer.Start();

            UpdateImage();
        }

        private void Timer_Tick(object sender, System.EventArgs e)
        {
            _currentImageIndex = (_currentImageIndex + 1) % _imagePaths.Count;
            UpdateImage();
        }

        private void UpdateImage()
        {
            var imagePath = _imagePaths[_currentImageIndex];
            SlideshowImage.Source = new BitmapImage(new System.Uri(imagePath, System.UriKind.Relative));
        }

        // NEW: Event handler for the "My Account" button
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
    }
}