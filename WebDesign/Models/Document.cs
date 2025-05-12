using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using HolidayManagerWeb.Models;

namespace HolidayManagerWeb.Models
{
    public class Document
    {
        [Key]
        public int DocumentId { get; set; }

        public string FileName { get; set; } = null!;
        public string FilePath { get; set; } = null!;

        [ForeignKey("Trip")]
        public int TripId { get; set; }
        public Trip Trip { get; set; } = null!;
    }
}
