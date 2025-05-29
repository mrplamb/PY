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

        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            Personal_info personalInfoPage = new Personal_info();
            personalInfoPage.Show(); 
            this.Close();

        }

        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            TripsPage tripsPage = new TripsPage();
            tripsPage.Show(); 
            this.Close();

        }

        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            BankInfo bankInfo = new BankInfo();
            bankInfo.Show();
            this.Close();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            UploadDocumentPage documents = new UploadDocumentPage();
            documents.Show();
            this.Close();
        }

        private void Dashboarding_Click(object sender, RoutedEventArgs e)
        {
            DashboardPage dashboard = new DashboardPage();
            dashboard.Show();
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppState.CurrentUser = null;

                MainWindow loginPage = new MainWindow();
                loginPage.Show();

                this.Close();
            }
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to log out?", "Confirm Logout", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                AppState.CurrentUser = null;

                MainWindow loginPage = new MainWindow();
                loginPage.Show();

                this.Close();
            }
        }



    }
}