using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection.Metadata;
using HolidayManagerWeb.Models;
/*using HolidayManager;*/

namespace HolidayManagerWeb.Models
{
    public class Trip
    {

        [Key]
        public int TripId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public string Destination { get; set; } = null!;
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }

        public decimal Budget { get; set; }
        public string Status { get; set; } = "Planned";

        public ICollection<Document> Documents { get; set; } = new List<Document>();

        public bool IsPaid { get; set; } = false;
        public decimal ActualCost { get; set; }

        public string? PlannedActivities { get; set; }
        public string? ItemsToTake { get; set; }

        public string Activities { get; set; }

        public class Transaction
        {
            public string Destination { get; set; }
        }

        public string Description { get; set; }

        public decimal Amount { get; set; }

        public string Category { get; set; }

        public string DatesRange => $"{StartDate.ToShortDateString()} - {EndDate.ToShortDateString()}";

        public decimal TotalSpent { get; set; } // Will be calculated dynamically
        public string TotalSpentFormatted => TotalSpent.ToString("C2");
        public string BudgetFormatted => Budget.ToString("C2");
        public string BudgetVsActualDisplay { get; set; }
    }
}

