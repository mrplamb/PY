using System;
using System.ComponentModel.DataAnnotations; // Add this for [Key]
using System.ComponentModel.DataAnnotations.Schema; // Already present, good!
using BCrypt.Net; // Already present, good!

namespace HolidayManagerWeb.Models
{
    public class Person
    {
        // Primary Key - Essential for EF Core
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Auto-incrementing ID
        public int ID { get; set; }

        public string Name { get; set; }
        public string Email { get; set; }

        [Column("passwordhash")] // Good, explicitly maps to 'passwordhash' column
        public string PasswordHash { get; private set; } // private set is good for SetPassword method
        public DateOnly Birth { get; set; }

        // --- NEW PROPERTIES FOR PERSONAL INFO ---
        public string Gender { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        // --- NEW PROPERTY FOR PROFILE PICTURE PATH ---
        public string ProfilePicturePath { get; set; }


        // Parameterless constructor (required by EF Core)
        public Person()
        {
            // Initialize new properties with default values
            // This ensures they are not null when a new Person object is created
            Gender = "Not specified"; // Or provide a sensible default
            PhoneNumber = "";
            Address = "";
            ProfilePicturePath = "";
        }

        // Constructor for creating a NEW Person (without an existing ID)
        // This is the constructor your User class's 4-parameter constructor should call via :base(...)
        public Person(string name, string email, string plainTextPassword, DateOnly birth)
        {
            Name = name;
            Email = email;
            SetPassword(plainTextPassword); // Hash the password here
            Birth = birth;

            // Initialize new properties
            Gender = "Not specified";
            PhoneNumber = "";
            Address = "";
            ProfilePicturePath = "";
        }

        // Constructor for loading an EXISTING Person from the database (with an ID and already hashed password)
        // This is the constructor your User class's 5-parameter constructor should call via :base(...)
        public Person(int id, string name, string email, string passwordHash, DateOnly birth)
        {
            ID = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash; // Use the already hashed password
            Birth = birth;

            // Initialize new properties (assuming they are loaded from DB if they exist)
            // Or provide defaults if not loaded or if this constructor is used for partial data
            Gender = "Not specified";
            PhoneNumber = "";
            Address = "";
            ProfilePicturePath = "";
        }

        // Method to set password (hashes the input password)
        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Method to verify password
        public bool VerifyPassword(string input)
        {
            return BCrypt.Net.BCrypt.Verify(input, PasswordHash);
        }

        public override string ToString()
        {
            return $"{Name} - {Email} - {Birth}";
        }
    }
}