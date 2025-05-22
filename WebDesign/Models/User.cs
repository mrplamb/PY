using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayManagerWeb.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HolidayManagerWeb.Models
{
    public class User : Person
    {
        //private Data data = new Data();

        public User() { }

        public ICollection<Trip> Trips { get; set; } = new List<Trip>();


        public User(string name, string email, string password, DateOnly birth)
        {
            Name = name;
            Email = email;
            SetPassword(password);
            Birth = birth;
            //ID = data.InsertUser(this);
        }

        public User(int id, string name, string email, string passwordHash, DateOnly birth) : base(id, name, email, passwordHash, birth)
        {
        }

        public decimal WalletBalance { get; set; } = 0;

        public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();


        public override string ToString()
        {
            return $"USER: {ID} - {Name} - {Birth}";
        }
    }
}
