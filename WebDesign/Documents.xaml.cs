using System;
using System.Collections.ObjectModel; // For ObservableCollection
using System.IO;                    // For file operations
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging; // For profile picture
using HolidayManagerWeb;            // For AppDbContext
using HolidayManagerWeb.Models;     // For User, Trip, and Document models
using Microsoft.EntityFrameworkCore; // For EF Core operations
using Microsoft.Win32;              // For OpenFileDialog
using System.Diagnostics;           // For Process.Start

namespace Final
{
    public partial class UploadDocumentPage : Window
    {
        private readonly AppDbContext _db;
        private User _currentUser;
        private Trip _selectedTrip; // To hold the currently selected trip

        // Use ObservableCollection to automatically update the UI when items are added/removed
        public ObservableCollection<Document> CurrentTripDocuments { get; set; }
        public ObservableCollection<Trip> UserTrips { get; set; } // To populate the Trip ComboBox

        public UploadDocumentPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            CurrentTripDocuments = new ObservableCollection<Document>();
            UserTrips = new ObservableCollection<Trip>();

            DocumentsList.ItemsSource = CurrentTripDocuments; // Bind the ItemsControl to this collection
            TripComboBox.ItemsSource = UserTrips; // Bind the Trip ComboBox

            LoadUserDataAndTrips(); // Load data when page initializes
        }

        private async void LoadUserDataAndTrips()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User session not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Retrieve the current user and their trips from the database
            _currentUser = await _db.Users
                                    .Include(u => u.Trips) // Include trips to populate ComboBox
                                    .FirstOrDefaultAsync(u => u.ID == AppState.CurrentUser.ID);

            if (_currentUser == null)
            {
                MessageBox.Show("User data could not be loaded from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Populate header user info
            HeaderUserNameTextBlock.Text = _currentUser.Name;
            LoadProfilePicture(_currentUser.ProfilePicturePath); // Load user's profile picture

            // Populate the Trip ComboBox
            UserTrips.Clear();
            foreach (var trip in _currentUser.Trips.OrderBy(t => t.StartDate)) // Order trips for better display
            {
                UserTrips.Add(trip);
            }

            // Select the first trip by default if available
            if (UserTrips.Any())
            {
                TripComboBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("You have no trips. Please create a trip first to upload documents.", "No Trips Found", MessageBoxButton.OK, MessageBoxImage.Information);
                // Optionally disable upload section if no trips
            }
        }

        // Handles selection change in the Trip ComboBox
        private async void TripComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripComboBox.SelectedItem is Trip selectedTrip)
            {
                _selectedTrip = selectedTrip;
                // Load documents for the newly selected trip
                await LoadDocumentsForSelectedTrip();
            }
            else
            {
                _selectedTrip = null;
                CurrentTripDocuments.Clear(); // Clear documents if no trip is selected
            }
        }

        // Loads documents for the currently selected trip
        private async System.Threading.Tasks.Task LoadDocumentsForSelectedTrip()
        {
            if (_selectedTrip == null)
            {
                CurrentTripDocuments.Clear();
                return;
            }

            // Load documents for the selected trip from the database
            // Ensure the trip itself is loaded with its documents
            var tripWithDocuments = await _db.Trips
                                             .Include(t => t.Documents)
                                             // CORRECTED: Filtering by TripId, not ID
                                             .FirstOrDefaultAsync(t => t.TripId == _selectedTrip.TripId);

            CurrentTripDocuments.Clear(); // Clear existing documents before adding new ones

            if (tripWithDocuments != null && tripWithDocuments.Documents != null)
            {
                foreach (var doc in tripWithDocuments.Documents.OrderBy(d => d.UploadDate))
                {
                    CurrentTripDocuments.Add(doc);
                }
            }
            else
            {
                CurrentTripDocuments.Clear();
            }
        }


        // Helper method to load the profile picture for the header
        private void LoadProfilePicture(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Release file handle
                    bitmap.EndInit();
                    HeaderProfileImage.ImageSource = bitmap;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading header profile picture: {ex.Message}");
                    HeaderProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Final;component/Images/default_profile.png"));
                }
            }
            else
            {
                HeaderProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Final;component/Images/default_profile.png"));
            }
        }


        private string _selectedFilePath; // To hold the path of the file chosen by the user

        // Handles the "Browse..." button click to open file dialog
        private void BrowseDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Document files (PDF, DOCX, XLSX, JPG, PNG)|*.pdf;*.docx;*.xlsx;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                FileNameTextBlock.Text = Path.GetFileName(_selectedFilePath); // Display only the file name
            }
        }

        // Handles the "Upload Document" button click
        private async void UploadDocument_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTrip == null)
            {
                MessageBox.Show("Please select a trip first.", "No Trip Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_selectedFilePath))
            {
                MessageBox.Show("Please select a document to upload first.", "No Document Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            // Get selected document type
            string documentType = "General"; // Default value
            if (DocumentTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                documentType = selectedItem.Content.ToString();
            }

            try
            {
                string originalFileName = Path.GetFileName(_selectedFilePath);
                string fileExtension = Path.GetExtension(_selectedFilePath).ToLower();
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                // Store documents in a folder specific to the Trip ID
                string tripDocsFolder = Path.Combine(appDataPath, "HolidayManager", "TripDocuments", _selectedTrip.TripId.ToString());

                // Create trip-specific documents folder if it doesn't exist
                if (!Directory.Exists(tripDocsFolder))
                {
                    Directory.CreateDirectory(tripDocsFolder);
                }

                // Generate a unique filename to prevent conflicts
                string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
                string destinationFilePath = Path.Combine(tripDocsFolder, uniqueFileName);

                // Copy the file
                File.Copy(_selectedFilePath, destinationFilePath, true);

                // Create new Document object
                var newDocument = new Document
                {
                    FileName = originalFileName,
                    FilePath = destinationFilePath,
                    FileType = fileExtension,
                    UploadDate = DateTime.Now,
                    DocumentType = documentType, // Set the selected document type
                    TripId = _selectedTrip.TripId, // Link to the currently selected Trip
                    UserId = AppState.CurrentUser.ID
                };

                _db.Documents.Add(newDocument); // Add to DbContext
                await _db.SaveChangesAsync();    // Save to database

                CurrentTripDocuments.Add(newDocument); // Add to ObservableCollection to update UI
                FileNameTextBlock.Text = "No file chosen"; // Reset display
                _selectedFilePath = null; // Clear selected file path

                MessageBox.Show($"Document '{originalFileName}' uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to upload document: {ex.Message}\nInner: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handles the "View" button click for a document
        private void ViewDocument_Click(object sender, RoutedEventArgs e)
        {
            // The Tag property of the button holds the FilePath
            string filePath = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                    // Open the document using the default application
                    Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true });
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Could not open document: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            else
            {
                MessageBox.Show("Document file not found or path is invalid.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        // Handles the "Delete" button click for a document
        private async void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
            // The Tag property of the button holds the DocumentId
            if (!int.TryParse((sender as Button)?.Tag?.ToString(), out int documentId))
            {
                MessageBox.Show("Could not get document ID for deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this document?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Find the document in the database
                    var documentToDelete = await _db.Documents.FindAsync(documentId);

                    if (documentToDelete != null)
                    {
                        // Delete the file from the file system
                        if (File.Exists(documentToDelete.FilePath))
                        {
                            try
                            {
                                File.Delete(documentToDelete.FilePath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Could not delete document file from disk: {ex.Message}");
                                // Don't block DB deletion if file delete fails, but log it
                            }
                        }

                        // Remove from database and save changes
                        _db.Documents.Remove(documentToDelete);
                        await _db.SaveChangesAsync();

                        // Remove from ObservableCollection to update UI
                        CurrentTripDocuments.Remove(documentToDelete);

                        MessageBox.Show("Document deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Document not found in database.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete document: {ex.Message}\nInner: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }


        // Sidebar Navigation Methods (ensure these open the correct windows)
        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            Personal_info personalInfoPage = new Personal_info();
            personalInfoPage.Show();
            this.Close();
        }

        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            // TripsPage tripsPage = new TripsPage(); // Replace with your actual Trips page
            // tripsPage.Show();
            // this.Close();
        }

        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            // BankInfo bankInfo = new BankInfo(); // Replace with your actual Finance page
            // bankInfo.Show();
            // this.Close();
        }

        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            // Already on Documents page, do nothing or reload if needed.
            // UploadDocumentPage documentsPage = new UploadDocumentPage();
            // documentsPage.Show();
            // this.Close();
        }

        private void Dashboarding_Click(object sender, RoutedEventArgs e)
        {
            // DashboardingPage dashboardingPage = new DashboardingPage(); // Replace with your actual Dashboarding page
            // dashboardingPage.Show();
            // this.Close();
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }


    }
}