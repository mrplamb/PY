using System.Windows;
using System.Windows.Controls;

namespace Final
{
    public partial class UploadDocumentPage : Window
    {
        public UploadDocumentPage()
        {
            InitializeComponent();
        }

        private void AddDocument_Click(object sender, RoutedEventArgs e)
        {
            string docName = NewDocumentTextBox.Text.Trim();

            if (!string.IsNullOrEmpty(docName))
            {
                Button newDocButton = new Button
                {
                    Content = docName,
                    Margin = new Thickness(0, 5, 0, 5),
                    Height = 40,
                    FontSize = 16,
                    BorderBrush = System.Windows.Media.Brushes.Black,
                    BorderThickness = new Thickness(1),
                    Background = System.Windows.Media.Brushes.White
                };

                // Optional: add click behavior for each document
                newDocButton.Click += (s, args) =>
                {
                    MessageBox.Show($"Clicked on {docName}", "Document", MessageBoxButton.OK);
                };

                DocumentsPanel.Children.Add(newDocButton);
                NewDocumentTextBox.Text = string.Empty;
            }
            else
            {
                MessageBox.Show("Please enter a document name.", "Input Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }
}
