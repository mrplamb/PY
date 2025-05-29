using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows;
using HolidayManagerWeb;
using HolidayManagerWeb.Models; 
using Final.Services;
using System.Diagnostics;
using System.Windows.Controls;


namespace Final
{
    public partial class BankTransactions : Window
    {
        private readonly NordigenService _nordigenService;
        private readonly AppDbContext _db;
        private readonly string _accessToken = "sandbox_token";

        public ObservableCollection<TransactionItem> Transactions { get; set; } = new();
        public ObservableCollection<string> Categories { get; set; } = new(); 

        public BankTransactions()
        {
            InitializeComponent();

            _db = new AppDbContext();
            _nordigenService = new NordigenService(_accessToken);

            TransactionsListView.ItemsSource = Transactions;
            CategoryComboBox.ItemsSource = Categories; 

            LoadTrips();
            LoadAccounts();
            LoadCategoriesFromTransactions(); 
        }

        
        private void LoadTrips()
        {
            if (AppState.CurrentUser == null) return;

            var trips = _db.Trips
                .Where(t => t.UserId == AppState.CurrentUser.ID)
                .OrderBy(t => t.StartDate)
                .ToList();

            TripComboBox.ItemsSource = trips;
            TripComboBox.DisplayMemberPath = "Destination";
            TripComboBox.SelectedValuePath = "TripId";

            if (trips.Any())
                TripComboBox.SelectedIndex = 0;
        }


        // Fetch transactions for selected account and link to selected trip
        private async void FetchTransactions_Click(object sender, RoutedEventArgs e)
        {
            if (TripComboBox.SelectedItem is not Trip selectedTrip)
            {
                MessageBox.Show("Please select a trip to link transactions.");
                return;
            }

            if (AccountComboBox.SelectedItem is string accountId)
            {
                try
                {
                    var json = await _nordigenService.FetchTransactionsAsync(accountId);
                    var doc = JsonDocument.Parse(json);

                    if (doc.RootElement.TryGetProperty("transactions", out var transactionsElement) &&
                        transactionsElement.TryGetProperty("booked", out var bookedElement) &&
                        bookedElement.ValueKind == JsonValueKind.Array)
                    {
                        var transactions = bookedElement.EnumerateArray()
                            .Select(txn => new TransactionItem
                            {
                                Date = txn.GetProperty("bookingDate").GetString(),
                                Description = txn.GetProperty("remittanceInformationUnstructured").GetString(),
                                Amount = txn.GetProperty("transactionAmount").GetProperty("amount").GetString()
                                
                            }).ToList();

                        await SaveTransactionsToDb(transactions, selectedTrip);

                        Transactions.Clear();
                        foreach (var txn in transactions)
                        {
                            Transactions.Add(txn);
                        }

                        MessageBox.Show("Transactions fetched and saved!");
                        LoadCategoriesFromTransactions(); 
                    }
                    else
                    {
                        MessageBox.Show("No booked transactions found.");
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error fetching transactions: {ex.Message}");
                }
            }
            else
            {
                MessageBox.Show("Please select an account first.");
            }
        }

        
        private async Task SaveTransactionsToDb(IEnumerable<TransactionItem> transactionItems, Trip selectedTrip)
        {
            foreach (var txnItem in transactionItems)
            {
                
                string category = CategoryComboBox.SelectedItem as string ?? "Uncategorized"; 

                var transaction = new Transaction
                {
                    UserId = AppState.CurrentUser.ID,
                    TripId = selectedTrip.TripId,
                    Amount = decimal.TryParse(txnItem.Amount, out var amt) ? amt : 0,
                    Timestamp = DateTime.TryParse(txnItem.Date, out var date) ? date : DateTime.Now,
                    Description = txnItem.Description,
                    Type = txnItem.Amount.StartsWith("-") ? "Debit" : "Credit",
                    Category = category 
                };

                _db.Transactions.Add(transaction);
            }

            await _db.SaveChangesAsync();
        }

       
        private async void AddManualTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (TripComboBox.SelectedItem is not Trip selectedTrip)
            {
                MessageBox.Show("Please select a trip to link this transaction.");
                return;
            }
            if (CategoryComboBox.SelectedItem is not string selectedCategory) 
            {
                MessageBox.Show("Please select a category for the transaction.");
                return;
            }

            if (!decimal.TryParse(ManualAmountTextBox.Text, out var amount))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            var manualTransaction = new Transaction
            {
                UserId = AppState.CurrentUser.ID,
                TripId = selectedTrip.TripId,
                Amount = amount,
                Timestamp = ManualDatePicker.SelectedDate ?? DateTime.Now,
                Description = ManualDescriptionTextBox.Text,
                Type = amount < 0 ? "Debit" : "Credit",
                Category = selectedCategory 
            };

            _db.Transactions.Add(manualTransaction);
            await _db.SaveChangesAsync();

            // Add to UI and refresh categories
            Transactions.Add(new TransactionItem
            {
                Date = manualTransaction.Timestamp.ToShortDateString(),
                Description = manualTransaction.Description,
                Amount = manualTransaction.Amount.ToString(),
                Category = manualTransaction.Category 
            });

            
            LoadCategoriesFromTransactions(); 
            ClearManualInputFields(); 
        }

        private async void CreateRequisition_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var nordigenService = new NordigenService("secret ID", "secret key");
                var redirectUrl = "https://localhost";
                var institutionId = "SANDBOXFINANCE_SFIN0000"; 
                var reference = "HolidayManager"; 

                var (requisitionId, link) = await nordigenService.CreateRequisitionAsync(redirectUrl, institutionId, reference);

                MessageBox.Show($"Requisition Created!\nID: {requisitionId}\nLink: {link}", "Success");
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = link,
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error creating requisition: {ex.Message}");
            }
        }

        private async void LinkBankAccount_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var redirectUri = "http://localhost"; 
                var institutionId = "BELFIUS BELGIUM"; 

                var authorizationUrl = await _nordigenService.CreateRequisitionAsync(redirectUri, institutionId);

                Process.Start(new ProcessStartInfo
                {
                    FileName = authorizationUrl,
                    UseShellExecute = true
                });

                MessageBox.Show("Please complete the bank authorization process in your browser.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error linking bank account: {ex.Message}");
            }
        }

        private async void LoadAccounts()
        {
            try
            {
                var requisitionId = "3fa85f64-5717-4562-b3fc-2c963f66afa6"; 
                var accounts = await _nordigenService.GetLinkedAccountsAsync(requisitionId);

                AccountComboBox.ItemsSource = accounts;
                MessageBox.Show("Accounts loaded successfully.");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading accounts: {ex.Message}");
            }
        }

        
        private void LoadTransactionsForTrip(Trip selectedTrip)
        {
            var tripTransactions = _db.Transactions
                .Where(t => t.TripId == selectedTrip.TripId)
                .OrderBy(t => t.Timestamp)
                .ToList();

            Transactions.Clear();
            foreach (var txn in tripTransactions)
            {
                Transactions.Add(new TransactionItem
                {
                    Date = txn.Timestamp.ToShortDateString(),
                    Description = txn.Description,
                    Amount = txn.Amount.ToString(),
                    Category = txn.Category 
                });
            }
        }

        private void TripComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripComboBox.SelectedItem is Trip selectedTrip)
            {
                LoadTransactionsForTrip(selectedTrip);
            }
        }

        private async void DeleteTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button deleteBtn && deleteBtn.Tag is string description)
            {
                if (TripComboBox.SelectedItem is not Trip selectedTrip)
                {
                    MessageBox.Show("Please select a trip first.");
                    return;
                }

               
                var transactionToDelete = _db.Transactions
                    .FirstOrDefault(t => t.TripId == selectedTrip.TripId && t.Description == description && t.UserId == AppState.CurrentUser.ID);

                if (transactionToDelete == null)
                {
                    MessageBox.Show("Transaction not found in DB.");
                    return;
                }

                var result = MessageBox.Show("Are you sure you want to delete this transaction?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);

                if (result == MessageBoxResult.Yes)
                {
                    _db.Transactions.Remove(transactionToDelete);
                    await _db.SaveChangesAsync();

                    // Remove from UI
                    var itemToRemove = Transactions.FirstOrDefault(txn => txn.Description == description);
                    if (itemToRemove != null)
                        Transactions.Remove(itemToRemove);

                    MessageBox.Show("Transaction deleted.");
                    LoadCategoriesFromTransactions(); 
                }
            }
        }

        private Transaction _selectedTransactionForEdit;

        
        private void TransactionsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TransactionsListView.SelectedItem is TransactionItem item)
            {
                ManualDatePicker.SelectedDate = DateTime.Parse(item.Date);
                ManualDescriptionTextBox.Text = item.Description;
                ManualAmountTextBox.Text = item.Amount;
                CategoryComboBox.SelectedItem = item.Category; // Set the category dropdown

               
                _selectedTransactionForEdit = _db.Transactions
                    .FirstOrDefault(t => t.Description == item.Description && t.UserId == AppState.CurrentUser.ID);
            }
        }

       
        private async void EditTransaction_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedTransactionForEdit == null)
            {
                MessageBox.Show("Please select a transaction to edit.");
                return;
            }
            if (CategoryComboBox.SelectedItem is not string selectedCategory) 
            {
                MessageBox.Show("Please select a category for the transaction.");
                return;
            }

            if (!decimal.TryParse(ManualAmountTextBox.Text, out var amount))
            {
                MessageBox.Show("Invalid amount.");
                return;
            }

            _selectedTransactionForEdit.Description = ManualDescriptionTextBox.Text;
            _selectedTransactionForEdit.Timestamp = ManualDatePicker.SelectedDate ?? DateTime.Now;
            _selectedTransactionForEdit.Amount = amount;
            _selectedTransactionForEdit.Category = selectedCategory; 

            await _db.SaveChangesAsync();

            
            LoadTransactionsForTrip(TripComboBox.SelectedItem as Trip);
            LoadCategoriesFromTransactions(); 
            MessageBox.Show("Transaction updated!");
            ClearManualInputFields(); 
        }

        
        private void LoadCategoriesFromTransactions()
        {
            try
            {
                Categories.Clear(); 

                
                var distinctCategories = _db.Transactions
                                            .Where(t => t.UserId == AppState.CurrentUser.ID && t.Category != null && t.Category != "")
                                            .Select(t => t.Category)
                                            .Distinct()
                                            .OrderBy(c => c)
                                            .ToList();

                foreach (var category in distinctCategories)
                {
                    Categories.Add(category);
                }

                
                if (!Categories.Contains("Food")) Categories.Add("Food");
                if (!Categories.Contains("Transportation")) Categories.Add("Transportation");
                if (!Categories.Contains("Activities")) Categories.Add("Activities");
                if (!Categories.Contains("Shopping")) Categories.Add("Shopping");
                if (!Categories.Contains("Souvenirs")) Categories.Add("Souvenirs");
                if (!Categories.Contains("Visits")) Categories.Add("Visits");
                if (!Categories.Contains("Extra Expenses")) Categories.Add("Extra Expenses");

                
                var sortedCategories = Categories.OrderBy(c => c).ToList();
                Categories.Clear();
                foreach (var category in sortedCategories)
                {
                    Categories.Add(category);
                }

               
                if (Categories.Any())
                {
                    CategoryComboBox.SelectedIndex = 0;
                }
                else
                {
                    CategoryComboBox.SelectedIndex = -1; 
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading categories from transactions: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private void ClearManualInputFields()
        {
            ManualDatePicker.SelectedDate = null;
            ManualDescriptionTextBox.Clear();
            ManualAmountTextBox.Clear();
            CategoryComboBox.SelectedIndex = -1;
            _selectedTransactionForEdit = null;
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
            Logout_Click(sender, e); 
        }

        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settingsWindow = new Settings();
            settingsWindow.Show();
            this.Close();
        }

        private void Logout_Click(object sender, RoutedEventArgs e)
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

        private void BackToHome_Click(object sender, RoutedEventArgs e)
        {
            HomePage homepage = new HomePage();
            homepage.Show();
            this.Close();
        }
    }

    
    public class TransactionItem
    {
        public string Date { get; set; }
        public string Description { get; set; }
        public string Amount { get; set; }
        public string Category { get; set; } 
    }
}