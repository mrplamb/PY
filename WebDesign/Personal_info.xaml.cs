using System;
using System.Windows;
using System.Windows.Controls;
using HolidayManagerWeb.Models; 
using HolidayManagerWeb; 
using System.Linq; 
using Microsoft.EntityFrameworkCore; 
using Microsoft.Win32; 
using System.IO;
using System.Windows.Media.Imaging; 

namespace Final
{
    public partial class Personal_info : Window
    {
        private readonly AppDbContext _db;
        private User _currentUser; 

        public Personal_info()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadUserData(); 
        }

        private void LoadUserData()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User session not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                this.Close(); 
                return;
            }

            
            _currentUser = _db.Users.Find(AppState.CurrentUser.ID);

            if (_currentUser == null)
            {
                MessageBox.Show("User data could not be loaded from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            
            HeaderUserNameTextBlock.Text = _currentUser.Name;
            FullNameTextBox.Text = _currentUser.Name;
            EmailTextBox.Text = _currentUser.Email; 

            if (_currentUser.Birth != DateOnly.MinValue) 
            {
                DobDatePicker.SelectedDate = _currentUser.Birth.ToDateTime(TimeOnly.MinValue);
            }

            
            if (!string.IsNullOrEmpty(_currentUser.Gender))
            {
                foreach (ComboBoxItem item in GenderComboBox.Items)
                {
                    if (item.Content.ToString() == _currentUser.Gender)
                    {
                        GenderComboBox.SelectedItem = item;
                        break;
                    }
                }
            }

            PhoneTextBox.Text = _currentUser.PhoneNumber;
            AddressTextBox.Text = _currentUser.Address;

            
            LoadProfilePicture(_currentUser.ProfilePicturePath);
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

                    ProfilePictureImage.Source = bitmap;
                    
                    HeaderProfileImage.ImageSource = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading profile picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                   
                    ProfilePictureImage.Source = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
                    HeaderProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
                }
            }
            else
            {
                
                ProfilePictureImage.Source = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
                HeaderProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
            }
        }


        private async void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_currentUser == null)
            {
                MessageBox.Show("No user loaded to save.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            
            _currentUser.Name = FullNameTextBox.Text.Trim();
            
            _currentUser.Birth = DobDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DobDatePicker.SelectedDate.Value)
                : DateOnly.MinValue; 

            _currentUser.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentUser.PhoneNumber = PhoneTextBox.Text.Trim();
            _currentUser.Address = AddressTextBox.Text.Trim();

            
            if (string.IsNullOrWhiteSpace(_currentUser.Name) || string.IsNullOrWhiteSpace(_currentUser.Email))
            {
                MessageBox.Show("Name and Email are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                
                _db.Entry(_currentUser).State = EntityState.Modified;
                await _db.SaveChangesAsync();

                
                AppState.CurrentUser = _currentUser;

                MessageBox.Show("Personal information saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                
                _db.Entry(_currentUser).Reload(); 
                LoadUserData(); 
            }
        }

        private async void UploadPhotoButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Image files (*.png;*.jpeg;*.jpg;*.gif;*.bmp)|*.png;*.jpeg;*.jpg;*.gif;*.bmp|All files (*.*)|*.*";
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            if (openFileDialog.ShowDialog() == true)
            {
                try
                {
                    string sourceFilePath = openFileDialog.FileName;
                    string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string appImagesFolder = Path.Combine(appDataPath, "HolidayManager", "ProfilePictures");

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(appImagesFolder))
                    {
                        Directory.CreateDirectory(appImagesFolder);
                    }

                    
                    string fileName = $"{_currentUser.ID}_{Guid.NewGuid()}{Path.GetExtension(sourceFilePath)}";
                    string destinationFilePath = Path.Combine(appImagesFolder, fileName);

                    // Copy the file
                    File.Copy(sourceFilePath, destinationFilePath, true); // Overwrite if exists (though Guid should prevent this)

                    // Delete old profile picture file if it exists
                    if (!string.IsNullOrEmpty(_currentUser.ProfilePicturePath) && File.Exists(_currentUser.ProfilePicturePath))
                    {
                        try
                        {
                            File.Delete(_currentUser.ProfilePicturePath);
                        }
                        catch (Exception ex)
                        {
                            // Log or display warning if old file can't be deleted
                            Console.WriteLine($"Warning: Could not delete old profile picture file: {ex.Message}");
                        }
                    }

                    // Update user's profile picture path in the model and database
                    _currentUser.ProfilePicturePath = destinationFilePath;
                    _db.Entry(_currentUser).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    // Update UI
                    LoadProfilePicture(destinationFilePath);
                    MessageBox.Show("Profile picture uploaded successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to upload picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DeletePhotoButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_currentUser.ProfilePicturePath))
            {
                MessageBox.Show("No profile picture to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete your profile picture?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                try
                {
                    // Delete the file from the file system
                    if (File.Exists(_currentUser.ProfilePicturePath))
                    {
                        File.Delete(_currentUser.ProfilePicturePath);
                    }

                    // Clear the path in the model and database
                    _currentUser.ProfilePicturePath = null; // Set to null or empty string
                    _db.Entry(_currentUser).State = EntityState.Modified;
                    await _db.SaveChangesAsync();

                    // Update UI to show default picture
                    LoadProfilePicture(null);
                    MessageBox.Show("Profile picture deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Failed to delete picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        // Existing methods
        private void GenderComboBox_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) { }
        private void PhoneTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void AddressTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e) { }
        private void BackToHome_Click(object sender, RoutedEventArgs e) { this.Close(); }

        
        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            Personal_info personalInfoPage = new Personal_info();
            personalInfoPage.Show(); // Show the new window
            this.Close();

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