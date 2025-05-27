using System;
using System.Windows;
using System.Windows.Controls;
using HolidayManagerWeb.Models; // For User model
using HolidayManagerWeb; // For AppDbContext
using System.Linq; // For LINQ queries (FirstOrDefault)
using Microsoft.EntityFrameworkCore; // For tracking changes
using Microsoft.Win32; // For OpenFileDialog
using System.IO; // For file operations
using System.Windows.Media.Imaging; // For BitmapImage

namespace Final
{
    public partial class Personal_info : Window
    {
        private readonly AppDbContext _db;
        private User _currentUser; // Keep a reference to the loaded user

        public Personal_info()
        {
            InitializeComponent();
            _db = new AppDbContext();
            LoadUserData(); // Load user data when the page initializes
        }

        private void LoadUserData()
        {
            if (AppState.CurrentUser == null)
            {
                MessageBox.Show("User session not found. Please log in again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optionally redirect to login or disable UI elements
                this.Close(); // Close this window if no user session
                return;
            }

            // Retrieve the current user from the database to ensure we have the latest data
            // Use .Find() for primary key lookup, or .FirstOrDefault() if you don't have the ID
            _currentUser = _db.Users.Find(AppState.CurrentUser.ID);

            if (_currentUser == null)
            {
                MessageBox.Show("User data could not be loaded from the database.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                this.Close();
                return;
            }

            // Populate UI fields with current user data
            HeaderUserNameTextBlock.Text = _currentUser.Name;
            FullNameTextBox.Text = _currentUser.Name;
            EmailTextBox.Text = _currentUser.Email; // Email is ReadOnly in XAML

            if (_currentUser.Birth != DateOnly.MinValue) // Check if DOB is set
            {
                DobDatePicker.SelectedDate = _currentUser.Birth.ToDateTime(TimeOnly.MinValue);
            }

            // Select gender in ComboBox
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

            // Load profile picture
            LoadProfilePicture(_currentUser.ProfilePicturePath);
        }

        private void LoadProfilePicture(string imagePath)
        {
            if (!string.IsNullOrEmpty(imagePath) && File.Exists(imagePath))
            {
                try
                {
                    // Use BitmapImage to load the image. Setting CacheOption.OnLoad is important
                    // to release the file handle immediately, allowing subsequent deletes/overwrites.
                    BitmapImage bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(imagePath);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad; // Release file handle
                    bitmap.EndInit();

                    ProfilePictureImage.Source = bitmap;
                    // Update header image as well
                    HeaderProfileImage.ImageSource = bitmap;
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error loading profile picture: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Fallback to default if load fails
                    ProfilePictureImage.Source = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
                    HeaderProfileImage.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Final;component/Views/Profile_picture.jpeg"));
                }
            }
            else
            {
                // Set default image if no path or file not found
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

            // Update _currentUser object with values from UI fields
            _currentUser.Name = FullNameTextBox.Text.Trim();
            // Email is ReadOnly, so no need to update it from TextBox
            _currentUser.Birth = DobDatePicker.SelectedDate.HasValue
                ? DateOnly.FromDateTime(DobDatePicker.SelectedDate.Value)
                : DateOnly.MinValue; // Or handle as validation error if DOB is mandatory

            _currentUser.Gender = (GenderComboBox.SelectedItem as ComboBoxItem)?.Content.ToString();
            _currentUser.PhoneNumber = PhoneTextBox.Text.Trim();
            _currentUser.Address = AddressTextBox.Text.Trim();

            // Validate data if necessary before saving
            if (string.IsNullOrWhiteSpace(_currentUser.Name) || string.IsNullOrWhiteSpace(_currentUser.Email))
            {
                MessageBox.Show("Name and Email are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                // Mark the entity as modified so EF Core knows to update it
                _db.Entry(_currentUser).State = EntityState.Modified;
                await _db.SaveChangesAsync(); // Use await for async save

                // Update AppState.CurrentUser with the saved changes
                // This is important so other parts of the app use the latest data
                AppState.CurrentUser = _currentUser;

                MessageBox.Show("Personal information saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving changes: {ex.Message}\nInner Exception: {ex.InnerException?.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Reload data in case of error to revert unsaved changes in UI
                _db.Entry(_currentUser).Reload(); // Reloads the entity from the database
                LoadUserData(); // Reload UI to reflect database state
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

                    // Generate a unique filename to prevent conflicts
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

        // Sidebar Navigation methods (as per previous discussion, consider using a Frame for better navigation)
        private void MyAccount_Click(object sender, RoutedEventArgs e)
        {
            // If Personal_info is already the current window, no need to open a new one.
            // If you use a Frame, you'd navigate it: (this.Parent as Window)?.MainFrame.Navigate(new Personal_infoPage());
        }
        private void Trips_Click(object sender, RoutedEventArgs e)
        {
            // You should pass the user data to TripsPage or ensure AppState.CurrentUser is set
            // new TripsPage().Show();
        }
        private void Finance_Click(object sender, RoutedEventArgs e)
        {
            // new BankInfo().Show();
        }
        private void Documents_Click(object sender, RoutedEventArgs e)
        {
            // new UploadDocumentPage().Show();
        }
        private void Dashboarding_Click(object sender, RoutedEventArgs e) { }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

    }
}