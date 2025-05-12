using Microsoft.AspNetCore.Mvc;
/*using HolidayManager;*/
using HolidayManagerWeb.Models;

namespace HolidayManagerWeb.Controllers
{
public class TripController : Controller
    {
        private readonly AppDbContext _db;

        public TripController(AppDbContext db)
        {
            _db = db;
        }

        public IActionResult Index(int userId)
        {
            var trips = _db.Trips.Where(t => t.UserId == userId).ToList();
            ViewBag.UserId = userId;
            return View(trips);
        }

        [HttpGet]
        public IActionResult Create(int userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        [HttpPost]
        public IActionResult Create(int userId, string destination, DateTime start, DateTime end, decimal budget)
        {
            var trip = new Trip
            {
                UserId = userId,
                Destination = destination,
                StartDate = DateOnly.FromDateTime(start),
                EndDate = DateOnly.FromDateTime(end),
                Budget = budget,
                Status = "Planned"
            };

            _db.Trips.Add(trip);
            _db.SaveChanges();

            return RedirectToAction("Index", new { userId });
        }

        public IActionResult Details(int id)
        {
            var trip = _db.Trips.FirstOrDefault(t => t.TripId == id);
            if (trip == null) return NotFound();

            return View(trip);
        }

        [HttpPost]
        public IActionResult PayForTrip(int tripId, int userId)
        {
            var trip = _db.Trips.FirstOrDefault(t => t.TripId == tripId && t.UserId == userId);
            var user = _db.Users.FirstOrDefault(u => u.ID == userId);

            if (trip != null && user != null && user.WalletBalance >= trip.Budget)
            {
                user.WalletBalance -= trip.Budget;
                trip.IsPaid = true;
                trip.ActualCost = trip.Budget;

                _db.Transactions.Add(new Transaction
                {
                    UserId = userId,
                    Amount = trip.Budget,
                    Type = "Debit",
                    Description = $"Payment for trip to {trip.Destination}"
                });

                _db.SaveChanges();
            }

            return RedirectToAction("Details", new { id = tripId });
        }
    }

}

