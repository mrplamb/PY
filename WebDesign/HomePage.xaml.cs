using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Collections.Generic;

namespace Final
{
    public partial class HomePage : Window
    {
        private List<string> _imagePaths = new List<string>
        {
            "Images/slide1.jpg",
            "Images/slide2.jpg",
            "Images/slide3.jpg"
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
    }
}
