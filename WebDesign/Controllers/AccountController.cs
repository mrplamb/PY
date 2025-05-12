using Microsoft.AspNetCore.Mvc;
/*using HolidayManager;*/
using HolidayManagerWeb.Models;


namespace HolidayManagerWeb.Controllers
{
    
    public class AccountController : Controller
    {
        private readonly AppDbContext _db;
        private readonly AccountService _service;

        public AccountController(AppDbContext db)
        {
            _db = db;
            _service = new AccountService(db);
        }

        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            var user = _service.Login(email, password);
            if (user == null) return View("LoginFailed");

            // TODO: Set session or cookie
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public IActionResult Register(string name, string email, string password, DateTime birth)
        {
            _service.RegisterUser(name, email, password, DateOnly.FromDateTime(birth));
            return RedirectToAction("Login");
        }
    }

}
