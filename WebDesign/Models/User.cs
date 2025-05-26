using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations.Schema; // Add this if you use [DatabaseGenerated] in Person

namespace HolidayManagerWeb.Models
{
    public class User : Person // User inherits from Person
    {
        public ICollection<Document> Documents { get; set; } = new List<Document>();
        public User() : base() // Call base constructor to initialize inherited properties
        {
            // Specific User initializations if any
            WalletBalance = 0; // Ensure WalletBalance is initialized
        }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();

        public User(string name, string email, string password, DateOnly birth)
            : base(name, email, BCrypt.Net.BCrypt.HashPassword(password), birth) // Hash password here
        {
            // Assuming Person has a constructor that takes name, email, passwordHash, birth
            // Initialize new User-specific properties if any
            WalletBalance = 0; // Default wallet balance
            // Inherited properties (Gender, PhoneNumber, etc.) are set by the base constructor
        }

        // Your existing constructor that takes ID, name, email, passwordHash, birth
        // Ensure this also correctly initializes the inherited properties from Person
        public User(int id, string name, string email, string passwordHash, DateOnly birth)
            : base(id, name, email, passwordHash, birth)
        {
            // Inherited properties (Gender, PhoneNumber, etc.) are set by the base constructor
        }

        public decimal WalletBalance { get; set; } = 0;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();

        public override string ToString()
        {
            return $"USER: {ID} - {Name} - {Birth}";
        }
    }
}