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
        public string Status { get; set; } = "Planned"; // Planned, Completed, Cancelled

        public ICollection<Document> Documents { get; set; } = new List<Document>();

        public bool IsPaid { get; set; } = false;
        public decimal ActualCost { get; set; }

    }
}
