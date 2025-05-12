using HolidayManagerWeb.Models;
using System.Collections.Generic;

namespace HolidayManagerWeb.Models
{
    public class DashboardModel
    {
        public int UserId { get; set; }
        public decimal WalletBalance { get; set; }
        public List<Transaction> Transactions { get; set; } = new();

    }
}
