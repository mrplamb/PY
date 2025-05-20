/*using HolidayManager;*/
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using HolidayManagerWeb.Models;

namespace HolidayManagerWeb.Controllers
{
    public class WalletController : Controller
    {
        private readonly AppDbContext _db;
        private readonly IConfiguration _config;

        public WalletController(AppDbContext db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        public IActionResult Dashboard(int userId)
        {
            var user = _db.Users
                .Where(u => u.ID == userId)
                .Select(u => new DashboardModel
                {
                    UserId = userId,
                    WalletBalance = u.WalletBalance,
                    Transactions = u.Transactions
                        .OrderByDescending(t => t.Timestamp)
                        .ToList()
                })
                .FirstOrDefault();

            if (user == null)
                return NotFound();

            return View(user);
        }


        public IActionResult AddFunds(int userId)
        {
            ViewBag.ClientId = _config["PayPal:ClientId"];
            ViewBag.UserId = userId;
            return View();
        }

        public IActionResult Success(int userId, decimal amount)
        {
            var user = _db.Users.FirstOrDefault(u => u.ID == userId);
            if (user != null)
            {
                user.WalletBalance += amount;
                _db.Transactions.Add(new Transaction
                {
                    UserId = userId,
                    Amount = amount,
                    Type = "Credit",
                    Description = "Wallet top-up via PayPal"
                });

                _db.SaveChanges();
            }

            return RedirectToAction("Dashboard", new { userId });
        }
    }
}

