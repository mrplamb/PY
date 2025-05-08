using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayManagerWeb.Models;
using System.ComponentModel.DataAnnotations;

namespace HolidayManagerWeb.Models
{
    public class Holiday
    {
        [Key]
        public int HolidayId { get; set; }
        public string Name { get; set; } = null!;
        public DateOnly Date { get; set; }
    }
}
