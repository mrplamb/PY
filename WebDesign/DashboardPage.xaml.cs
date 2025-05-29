using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using LiveCharts;
using LiveCharts.Wpf;
using HolidayManagerWeb; 
using HolidayManagerWeb.Models; 

namespace Final
{
    public partial class DashboardPage : Window
    {
        private readonly AppDbContext _db;

        // --- LiveCharts Series Collections ---
        public SeriesCollection CategorySeries { get; set; } //Pie Chart
        public SeriesCollection ExpenseOverTimeSeries { get; set; } //Line Chart
        public SeriesCollection BudgetSeries { get; set; } // (Budget vs Actual)

        // Axis Properties for Line Chart
        public string[] Labels { get; set; } // dates for Line Chart
        public Func<double, string> Formatter { get; set; } // Y-axis label (currency)

        // Observable Collections for ListViews
        public ObservableCollection<TripSummaryItem> TripsSummary { get; set; } // For the TripsListView
        public ObservableCollection<Transaction> TopExpenses { get; set; } // For the TopExpensesListView

        // Properties for Overall Financial Summary
        public string OverallTotalTripsDisplay { get; set; }
        public string OverallTotalExpensesDisplay { get; set; }
        public string OverallHighestSpentTripDisplay { get; set; }


        public DashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();

            // Initialize Series Collections
            CategorySeries = new SeriesCollection();
            ExpenseOverTimeSeries = new SeriesCollection();
            BudgetSeries = new SeriesCollection();

            // Initialize Observable Collections
            TripsSummary = new ObservableCollection<TripSummaryItem>();
            TopExpenses = new ObservableCollection<Transaction>();

            // Line Chart
            Formatter = value => value.ToString("C2"); // Formats Y-axis values as currency

            this.DataContext = this;

            LoadOverallSummary();
            LoadTripsSummary();
        }

        // Overall Financial Summary Methods 
        private void LoadOverallSummary()
        {
            if (AppState.CurrentUser == null) return;

            try
            {
                var userTrips = _db.Trips.Where(t => t.UserId == AppState.CurrentUser.ID).ToList();
                var userTransactions = _db.Transactions.Where(t => t.UserId == AppState.CurrentUser.ID).ToList();

                // Calculate total expenses across all trips (assuming expenses are positive amounts)
                decimal totalExpensesAcrossTrips = userTransactions
                    .Where(t => t.Amount > 0) // Adjust filter if expenses are negative or use 'Type'
                    .Sum(t => Math.Abs(t.Amount));

                // Find highest spent trip
                var highestSpentTrip = userTrips
                    .Select(trip => new
                    {
                        Trip = trip,
                        TotalSpent = userTransactions
                                        .Where(t => t.TripId == trip.TripId && t.Amount > 0) // Adjust filter
                                        .Sum(t => Math.Abs(t.Amount))
                    })
                    .OrderByDescending(x => x.TotalSpent)
                    .FirstOrDefault();

                // Update UI TextBlocks
                OverallTotalTripsDisplay = userTrips.Count.ToString();
                OverallTotalExpensesDisplay = totalExpensesAcrossTrips.ToString("C2");
                OverallHighestSpentTripDisplay = highestSpentTrip != null
                    ? $"{highestSpentTrip.Trip.Destination} ({highestSpentTrip.TotalSpent:C2})"
                    : "N/A";

                // Notify UI of property changes (important if not using MVVM framework)
                OverallTotalTripsTextBlock.Text = OverallTotalTripsDisplay;
                OverallTotalExpensesTextBlock.Text = OverallTotalExpensesDisplay;
                OverallHighestSpentTripTextBlock.Text = OverallHighestSpentTripDisplay;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading overall summary: {ex.Message}", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
                OverallTotalTripsTextBlock.Text = "Error";
                OverallTotalExpensesTextBlock.Text = "Error";
                OverallHighestSpentTripTextBlock.Text = "Error";
            }
        }

        // Trip List / Summary Methods 
        private void LoadTripsSummary()
        {
            if (AppState.CurrentUser == null) return;

            try
            {
                TripsSummary.Clear();
                var trips = _db.Trips
                    .Where(t => t.UserId == AppState.CurrentUser.ID)
                    .OrderBy(t => t.StartDate)
                    .ToList();

                foreach (var trip in trips)
                {
                    decimal totalSpent = _db.Transactions
                        .Where(t => t.TripId == trip.TripId && t.Amount > 0) 
                        .Sum(t => Math.Abs(t.Amount));

                    TripsSummary.Add(new TripSummaryItem
                    {
                        TripId = trip.TripId,
                        Destination = trip.Destination,
                        StartDate = trip.StartDate,
                        EndDate = trip.EndDate,
                        Budget = trip.Budget,
                        Status = trip.Status,
                        TotalSpent = totalSpent,
                        TotalSpentFormatted = totalSpent.ToString("C2"),
                        BudgetFormatted = trip.Budget.ToString("C2"),
                        BudgetVsActualDisplay = $"{totalSpent:C2} / {trip.Budget:C2}",
                        RemainingBudget = trip.Budget - totalSpent
                    });
                }

                // Automatically select the first trip if available
                if (TripsSummary.Any())
                {
                    TripsListView.SelectedIndex = 0;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading trip summaries: {ex.Message}", "Data Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Handles selection in the Trips ListView to load detailed data
        private void TripsListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripsListView.SelectedItem is TripSummaryItem selectedTripSummary)
            {
                // Find the actual Trip object from the database using its ID
                var selectedTrip = _db.Trips.FirstOrDefault(t => t.TripId == selectedTripSummary.TripId);
                if (selectedTrip != null)
                {
                    SelectedTripNameTextBlock.Text = $"Details for {selectedTrip.Destination}";
                    LoadChartsAndDetailsForTrip(selectedTrip);
                }
            }
            else
            {
                SelectedTripNameTextBlock.Text = "Select a Trip for Details";
                // Clear all charts and lists if no trip is selected
                CategorySeries.Clear();
                ExpenseOverTimeSeries.Clear();
                BudgetSeries.Clear();
                Labels = new string[0]; // Clear labels for line chart
                TopExpenses.Clear();
            }
        }

        // Main method to load all charts and lists for a selected trip
        private void LoadChartsAndDetailsForTrip(Trip selectedTrip)
        {
            LoadPieChartData(selectedTrip);
            LoadLineChartData(selectedTrip);
            LoadDonutChartData(selectedTrip, selectedTrip.Budget); // Pass trip budget
            LoadTopExpensesData(selectedTrip);
        }

        // --- Chart Data Loading Methods ---

        // Renamed from LoadExpenseData to be more specific
        // Replace the LoadPieChartData method in your DashboardPage.xaml.cs with this:

        private void LoadPieChartData(Trip selectedTrip)
        {
            try
            {
                var expenseData = _db.Transactions
                    .Where(t => t.TripId == selectedTrip.TripId &&
                                t.UserId == AppState.CurrentUser.ID &&
                                t.Category != null &&
                                t.Category != "" &&
                                t.Amount > 0) // Assuming positive expenses
                    .GroupBy(t => t.Category)
                    .Select(g => new
                    {
                        Category = g.Key,
                        TotalExpenses = Math.Abs(g.Sum(t => t.Amount))
                    })
                    .OrderByDescending(x => x.TotalExpenses)
                    .ToList();

                CategorySeries.Clear();
                decimal grandTotalExpenses = 0;

                foreach (var data in expenseData)
                {
                    if (data.TotalExpenses > 0)
                    {
                        CategorySeries.Add(new PieSeries
                        {
                            Title = data.Category,
                            Values = new ChartValues<decimal> { data.TotalExpenses },
                            LabelPoint = chartPoint =>
                                $"{chartPoint.SeriesView.Title}: {chartPoint.Y.ToString("C2")} ({chartPoint.Participation:P1})"
                        });
                        grandTotalExpenses += data.TotalExpenses;
                    }
                }

                // Use the correct TextBlock name from your XAML
                SelectedTripTotalExpensesTextBlock.Text = $"Total for Trip: {grandTotalExpenses:C2}";
                if (!CategorySeries.Any())
                {
                    SelectedTripTotalExpensesTextBlock.Text = "No expenses recorded for this trip.";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading pie chart data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CategorySeries.Clear();
                SelectedTripTotalExpensesTextBlock.Text = "Error loading data.";
            }
        }

        private void LoadLineChartData(Trip selectedTrip)
        {
            try
            {
                // Group transactions by date and sum expenses for each day
                var dailyExpenses = _db.Transactions
                    .Where(t => t.TripId == selectedTrip.TripId &&
                                t.UserId == AppState.CurrentUser.ID &&
                                t.Amount > 0) // Assuming positive expenses
                    .GroupBy(t => t.Timestamp.Date)
                    .Select(g => new { Date = g.Key, DailyTotal = Math.Abs(g.Sum(t => t.Amount)) })
                    .OrderBy(x => x.Date)
                    .ToList();

                // Generate labels (dates) for the X-axis
                Labels = dailyExpenses.Select(x => x.Date.ToShortDateString()).ToArray();

                // Create a LineSeries for the daily expenses
                ExpenseOverTimeSeries.Clear();
                ExpenseOverTimeSeries.Add(new LineSeries
                {
                    Title = "Daily Expenses",
                    Values = new ChartValues<decimal>(dailyExpenses.Select(x => x.DailyTotal)),
                    PointGeometrySize = 10,
                    DataLabels = true, // Show labels on data points
                    LabelPoint = chartPoint => chartPoint.Y.ToString("C2") // Format point labels as currency
                });

                // Notify UI of property changes
                OnPropertyChanged(nameof(Labels));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading line chart data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ExpenseOverTimeSeries.Clear();
                Labels = new string[0]; // Clear labels on error
                OnPropertyChanged(nameof(Labels));
            }
        }

        private void LoadDonutChartData(Trip selectedTrip, decimal tripBudget)
        {
            try
            {
                decimal totalSpent = _db.Transactions
                    .Where(t => t.TripId == selectedTrip.TripId &&
                                t.UserId == AppState.CurrentUser.ID &&
                                t.Amount > 0) // Assuming positive expenses
                    .Sum(t => Math.Abs(t.Amount));

                decimal remainingBudget = tripBudget - totalSpent;

                BudgetSeries.Clear();

                // Add spent portion
                BudgetSeries.Add(new PieSeries
                {
                    Title = "Spent",
                    Values = new ChartValues<decimal> { totalSpent },
                    LabelPoint = chartPoint => $"{chartPoint.Y.ToString("C2")}",
                    Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightCoral) // Customize color
                });

                // Add remaining portion (only if positive)
                if (remainingBudget > 0)
                {
                    BudgetSeries.Add(new PieSeries
                    {
                        Title = "Remaining",
                        Values = new ChartValues<decimal> { remainingBudget },
                        LabelPoint = chartPoint => $"{chartPoint.Y.ToString("C2")}",
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.LightGreen) // Customize color
                    });
                }
                else if (remainingBudget < 0)
                {
                    // If over budget, show the "over budget" amount as a negative remaining or a dedicated slice
                    BudgetSeries.Add(new PieSeries
                    {
                        Title = "Over Budget",
                        Values = new ChartValues<decimal> { Math.Abs(remainingBudget) },
                        LabelPoint = chartPoint => $"{chartPoint.Y.ToString("C2")}",
                        Fill = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Red) // Customize color
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading donut chart data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                BudgetSeries.Clear();
            }
        }

        // --- Top Expenses / Categories Methods ---
        private void LoadTopExpensesData(Trip selectedTrip)
        {
            try
            {
                TopExpenses.Clear();
                // Get top 5 (or desired number) of highest individual expenses for the trip
                var topTxns = _db.Transactions
                    .Where(t => t.TripId == selectedTrip.TripId &&
                                t.UserId == AppState.CurrentUser.ID &&
                                t.Amount > 0) // Assuming positive expenses
                    .OrderByDescending(t => Math.Abs(t.Amount))
                    .Take(5) // Get top 5
                    .ToList();

                foreach (var txn in topTxns)
                {
                    TopExpenses.Add(txn);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading top expenses: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                TopExpenses.Clear();
            }
        }

        // --- Filter Methods ---
        private void FilterDates_Changed(object sender, SelectionChangedEventArgs e)
        {
            // This method would be used to filter the TripsListView based on the selected date range.
            // For now, it's a placeholder. If you want to filter all trips by date range,
            // you'd re-call LoadTripsSummary() with the selected date range.
            // Example:
            // DateTime? startDate = StartDatePicker.SelectedDate;
            // DateTime? endDate = EndDatePicker.SelectedDate;
            // LoadTripsSummary(startDate, endDate); // Adapt LoadTripsSummary to take dates
            MessageBox.Show("Date range filtering not yet fully implemented for the trip list.", "Feature", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // --- Interactive Elements Methods ---
        private void ExpensePieChart_DataClick(object sender, ChartPoint chartPoint)
        {
            // This event fires when a slice of the pie chart is clicked.
            // chartPoint.SeriesView.Title gives you the category name (e.g., "Food & Dining")
            string clickedCategory = chartPoint.SeriesView.Title;

            MessageBox.Show($"You clicked on '{clickedCategory}' expenses. " +
                            $"This feature will soon filter the transactions list!", "Chart Clicked", MessageBoxButton.OK, MessageBoxImage.Information);

            // Future: You could open a new window, or filter the TopExpensesListView
            // to show only transactions for this category.
            // Example: (Requires a Transactions ListView outside of TopExpenses or a new modal)
            // var transactionsForCategory = _db.Transactions
            //     .Where(t => t.TripId == ((TripSummaryItem)TripsListView.SelectedItem).TripId &&
            //                 t.Category == clickedCategory &&
            //                 t.UserId == AppState.CurrentUser.ID)
            //     .ToList();
            // Show this list to the user.
        }

        // --- Helper Class for Trip Summary (for ListView) ---
        // You should define this class either directly in DashboardPage.xaml.cs
        // or in a separate Models folder if it's reused.
        public class TripSummaryItem : Trip // Inheriting from Trip is useful for common properties
        {
            // These properties are calculated for display purposes in the Dashboard
            public decimal TotalSpent { get; set; }
            public string TotalSpentFormatted { get; set; }
            public string BudgetFormatted { get; set; }
            public string BudgetVsActualDisplay { get; set; }
            public decimal RemainingBudget { get; set; }

            // Helper for ListView column
            public string DatesRange => $"{StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";
        }

        // --- INotifyPropertyChanged Implementation (Recommended for UI Updates) ---
        // Although this example uses direct TextBlock.Text updates and ObservableCollections,
        // for properties like Labels and Formatter, and if you expand properties
        // (like OverallTotalTripsDisplay), INotifyPropertyChanged is best practice.
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
        }
    }
}