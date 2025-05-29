using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using HolidayManagerWeb;
using HolidayManagerWeb.Models; // Ensure your Transaction and Trip models are here

// Add LiveCharts using directives
using LiveCharts;
using LiveCharts.Wpf;

namespace Final
{
    public partial class DashboardPage : Window
    {
        private readonly AppDbContext _db;

        // This property holds the data series that LiveCharts uses for the pie chart.
        public SeriesCollection CategorySeries { get; set; }

        public DashboardPage()
        {
            InitializeComponent();
            _db = new AppDbContext();

            // Initialize the SeriesCollection. This is crucial for LiveCharts.
            CategorySeries = new SeriesCollection();
            // Set the DataContext of the window to itself so XAML bindings can find CategorySeries.
            this.DataContext = this;

            LoadTrips();
        }

        // Loads the user's trips into the ComboBox.
        private void LoadTrips()
        {
            // Only load trips if a user is logged in.
            if (AppState.CurrentUser == null) return;

            var trips = _db.Trips
                .Where(t => t.UserId == AppState.CurrentUser.ID)
                .OrderBy(t => t.StartDate)
                .ToList();

            TripComboBox.ItemsSource = trips;
            // The DisplayMemberPath="Destination" is set in XAML, so no need here.
            TripComboBox.SelectedValuePath = "TripId";

            // Select the first trip if there are any, to automatically load data.
            if (trips.Any())
                TripComboBox.SelectedIndex = 0;
        }

        // Event handler for when a different trip is selected in the ComboBox.
        private void TripComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TripComboBox.SelectedItem is Trip selectedTrip)
            {
                LoadExpenseData(selectedTrip);
            }
            else
            {
                // If no trip is selected (e.g., if the list becomes empty), clear the chart and text.
                CategorySeries.Clear();
                TotalExpensesTextBlock.Text = "";
            }
        }

        // Loads and processes expense data for the selected trip to display on the pie chart.
        private void LoadExpenseData(Trip selectedTrip)
        {
            try
            {
                // Query the database to get transactions for the selected trip and current user.
                // It then groups them by category and calculates the total expenses for each category.
                var expenseData = _db.Transactions
                    .Where(t => t.TripId == selectedTrip.TripId &&
                                t.UserId == AppState.CurrentUser.ID &&
                                t.Category != null && // Exclude transactions where the category is NULL in the database
                                t.Category != "" &&   // Exclude transactions where the category is an empty string
                                                      // === IMPORTANT: FILTER FOR EXPENSES ===
                                                      // Uncomment the line below that matches how expenses are recorded in your database:

                                // Option 1: If expenses are stored as POSITIVE numbers (e.g., 50.00 for food)
                                t.Amount > 0) // <--- Use this line if your expenses are positive amounts

                    // Option 2: If expenses are stored as NEGATIVE numbers (e.g., -50.00 for food)
                    // t.Amount < 0) // <--- Use this line if your expenses are negative amounts

                    // Option 3: If your Transaction model has a 'Type' property (e.g., "Debit", "Credit")
                    // t.Type == "Debit") // <--- Use this line if you classify expenses with a "Debit" type

                    .GroupBy(t => t.Category) // Group transactions by their category
                    .Select(g => new // Project the grouped data into an anonymous type
                    {
                        Category = g.Key, // The category name
                        TotalExpenses = Math.Abs(g.Sum(t => t.Amount)) // Sum the absolute amounts for each category
                    })
                    .OrderByDescending(x => x.TotalExpenses) // Order categories by total expenses (largest first for visual appeal)
                    .ToList(); // Execute the query and get the results as a list

                // Clear any existing series from the chart before adding new data.
                CategorySeries.Clear();
                decimal grandTotalExpenses = 0; // Initialize a variable to sum up all expenses for the trip.

                // Iterate through the aggregated expense data to create pie slices.
                foreach (var data in expenseData)
                {
                    if (data.TotalExpenses > 0) // Only add slices for categories with actual expenses (greater than zero)
                    {
                        CategorySeries.Add(new PieSeries
                        {
                            Title = data.Category, // The name of the category, shown in the legend and tooltip
                            Values = new ChartValues<decimal> { data.TotalExpenses }, // The numerical value for this slice
                            // Define how the label on the pie slice itself and in the tooltip will be formatted.
                            // {chartPoint.Y.ToString("C2")} formats the amount as currency.
                            // {chartPoint.Participation:P1} formats the percentage of the total.
                            LabelPoint = chartPoint =>
                                $"{chartPoint.SeriesView.Title}: {chartPoint.Y.ToString("C2")} ({chartPoint.Participation:P1})"
                        });
                        grandTotalExpenses += data.TotalExpenses; // Add to the running total.
                    }
                }

                // Check if any expense data was successfully added to the chart.
                if (!CategorySeries.Any())
                {
                    // If no data, inform the user with a message.
                    MessageBox.Show("No expenses found for this trip to display on the chart. " +
                                    "Please ensure transactions for this trip have a valid category " +
                                    "and that their amounts are recorded correctly (e.g., as positive expenses).",
                                    "No Data", MessageBoxButton.OK, MessageBoxImage.Information);
                    TotalExpensesTextBlock.Text = "No expenses recorded for this trip."; // Update the total expenses display.
                }
                else
                {
                    // If data exists, display the grand total expenses.
                    TotalExpensesTextBlock.Text = $"Total Expenses: {grandTotalExpenses:C2}";
                }
            }
            catch (Exception ex)
            {
                // Catch and display any errors that occur during the data loading process.
                MessageBox.Show($"Error loading expense data: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                CategorySeries.Clear(); // Clear the chart to indicate an error.
                TotalExpensesTextBlock.Text = "Error loading data."; // Update the total expenses display.
            }
        }
    }
}