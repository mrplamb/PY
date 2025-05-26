// HolidayManagerWeb.Models/Document.cs
using System; // Make sure System is imported for DateTime
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HolidayManagerWeb.Models; // Ensure this is present for Trip model

namespace HolidayManagerWeb.Models
{
    public class Document
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] // Add this attribute for auto-incrementing ID
        public int DocumentId { get; set; } // Changed from ID to DocumentId as per your request

        public string FileName { get; set; } = null!; // Non-nullable string (C# 8.0 feature)
        public string FilePath { get; set; } = null!; // Non-nullable string

        // --- RE-ADDING PREVIOUSLY DISCUSSED PROPERTIES ---
        public string FileType { get; set; } = null!; // e.g., ".pdf", ".docx"
        public DateTime UploadDate { get; set; }

        // --- NEW PROPERTY FOR CATEGORIZATION (as discussed) ---
        public string DocumentType { get; set; } = "General"; // e.g., "Passport", "Visa", "Ticket", "Insurance"

        // Foreign Key to link the document to a Trip
        [ForeignKey("Trip")] // Specifies the foreign key relationship
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!; // Navigation property to the associated Trip

        public Document()
        {
            UploadDate = DateTime.Now; // Default to current time when created
            // FileType and DocumentType will be set during upload, but give a default
            // for string.Empty or "General" for safety if not explicitly set.
        }
    }
}