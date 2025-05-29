using System;
using System.Collections.ObjectModel; 
using System.IO;                    
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging; 
using HolidayManagerWeb;            
using HolidayManagerWeb.Models;     
using Microsoft.EntityFrameworkCore; 
using Microsoft.Win32;              
using System.Diagnostics;          

namespace Final
{
    public partial class UploadDocumentPage : Window
    {
        private readonly AppDbContext _db;
        private User _currentUser;
        private Trip _selectedTrip; 

        
        public ObservableCollection<Document> CurrentTripDocuments { get; set; }
        public ObservableCollection<Trip> UserTrips { get; set; } 

        public UploadDocumentPage()
        {
            InitializeComponent();
            _db = new AppDbContext();
            CurrentTripDocuments = new ObservableCollection<Document>();
            UserTrips = new ObservableCollection<Trip>();

            DocumentsList.ItemsSource = CurrentTripDocuments; 
            TripComboBox.ItemsSource = UserTrips; 

            LoadUserDataAndTrips(); 
        }

        private async void LoadUserDataAndTrips()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User session not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            
            _currentUser = await _db.Users
                                    .Include(u => u.Trips) 
                                    .FirstOrDefaultAsync(u => u.ID == AppState.CurrentUser.ID);

            if (_currentUser == null)
            {
                MessageBox.Show("User data could not be loaded from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            
            HeaderUserNameTextBlock.Text = _currentUser.Name;
            LoadProfilePicture(_currentUser.ProfilePicturePath); 

            
            UserTrips.Clear();
            foreach (var trip in _currentUser.Trips.OrderBy(t => t.StartDate)) 
            {
                UserTrips.Add(trip);
            }

           
            if (UserTrips.Any())
            {
                TripComboBox.SelectedIndex = 0;
            }
            else
            {
                MessageBox.Show("You have no trips. Please create a trip first to upload documents.", "No Trips Found", MessageBoxButton.OK, MessageBoxImage.Information);
                
            }
        }

        
        private async void TripComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripComboBox.SelectedItem is Trip selectedTrip)
            {
                _selectedTrip = selectedTrip;
                
                await LoadDocumentsForSelectedTrip();
            }
            else
            {
                _selectedTrip = null;
                CurrentTripDocuments.Clear(); 
            }
        }

        
        private async System.Threading.Tasks.Task LoadDocumentsForSelectedTrip()
        {
            if (_selectedTrip == null)
            {
                CurrentTripDocuments.Clear();
                return;
            }

            
            var tripWithDocuments = await _db.Trips
                                             .Include(t => t.Documents)
                                             
                                             .FirstOrDefaultAsync(t => t.TripId == _selectedTrip.TripId);

            CurrentTripDocuments.Clear(); 

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


       
        private void LoadProfilePicture(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; 
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


        private string _selectedFilePath; 
        
        private void BrowseDocument_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Document files (PDF, DOCX, XLSX, JPG, PNG)|*.pdf;*.docx;*.xlsx;*.jpg;*.jpeg;*.png|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                _selectedFilePath = openFileDialog.FileName;
                FileNameTextBlock.Text = Path.GetFileName(_selectedFilePath); 
            }
        }

       
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

            
            string documentType = "General"; 
            if (DocumentTypeComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                documentType = selectedItem.Content.ToString();
            }

            try
            {
                string originalFileName = Path.GetFileName(_selectedFilePath);
                string fileExtension = Path.GetExtension(_selectedFilePath).ToLower();
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                
                string tripDocsFolder = Path.Combine(appDataPath, "HolidayManager", "TripDocuments", _selectedTrip.TripId.ToString());

                
                if (!Directory.Exists(tripDocsFolder))
                {
                    Directory.CreateDirectory(tripDocsFolder);
                }

                
                string uniqueFileName = $"{Guid.NewGuid()}_{originalFileName}";
                string destinationFilePath = Path.Combine(tripDocsFolder, uniqueFileName);

                
                File.Copy(_selectedFilePath, destinationFilePath, true);

                
                var newDocument = new Document
                {
                    FileName = originalFileName,
                    FilePath = destinationFilePath,
                    FileType = fileExtension,
                    UploadDate = DateTime.Now,
                    DocumentType = documentType, 
                    TripId = _selectedTrip.TripId, 
                    UserId = AppState.CurrentUser.ID
                };

                _db.Documents.Add(newDocument); 
                await _db.SaveChangesAsync();    

                CurrentTripDocuments.Add(newDocument); 
                FileNameTextBlock.Text = "No file chosen"; 
                _selectedFilePath = null; 

                MessageBox.Show($"Document '{originalFileName}' uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to upload document: {ex.Message}\nInner: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

       
        private void ViewDocument_Click(object sender, RoutedEventArgs e)
        {
            
            string filePath = (sender as Button)?.Tag?.ToString();

            if (!string.IsNullOrEmpty(filePath) && File.Exists(filePath))
            {
                try
                {
                   
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

       
        private async void DeleteDocument_Click(object sender, RoutedEventArgs e)
        {
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
                  
                    var documentToDelete = await _db.Documents.FindAsync(documentId);

                    if (documentToDelete != null)
                    {
                        
                        if (File.Exists(documentToDelete.FilePath))
                        {
                            try
                            {
                                File.Delete(documentToDelete.FilePath);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"Warning: Could not delete document file from disk: {ex.Message}");
                                
                            }
                        }

                        
                        _db.Documents.Remove(documentToDelete);
                        await _db.SaveChangesAsync();

                        
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