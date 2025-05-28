using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HolidayManagerWeb.Models;
/*using HolidayManager;*/

namespace HolidayManagerWeb.Models
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; } = null!;

        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;

        public decimal Amount { get; set; }
        public string Type { get; set; } = "Credit"; // Credit or Debit
        public string Description { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        public string Category { get; set; } 
    }
}

