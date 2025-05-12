using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayManagerWeb.Models;

namespace HolidayManagerWeb.Models
{
    internal class AccountService
    {
        private readonly AppDbContext _db;

        public AccountService(AppDbContext db)
        {
            _db = db;
        }

        public int RegisterUser(string name, string email, string password, DateOnly birth)
        {
            var user = new User(name, email, password, birth);
            _db.Users.Add(user);
            _db.SaveChanges();
            return user.ID;
        }

        public int RegisterAdmin(string name, string email, string password, DateOnly birth)
        {
            var admin = new Admin(name, email, password, birth);
            _db.Admins.Add(admin);
            _db.SaveChanges();
            return admin.ID;
        }

        public Person? Login(string email, string password)
        {
            var user = _db.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && user.VerifyPassword(password))
                return user;

            var admin = _db.Admins.FirstOrDefault(a => a.Email == email);
            if (admin != null && admin.VerifyPassword(password))
                return admin;

            return null;
        }

    }
}
