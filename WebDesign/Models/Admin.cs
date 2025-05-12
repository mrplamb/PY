using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HolidayManagerWeb.Models;


namespace HolidayManagerWeb.Models
{
    public class Admin : Person
    {
        //private Data data = new Data();

        public Admin(string name, string email, string password, DateOnly birth)
        {
            Name = name;
            Email = email;
            SetPassword(password);
            Birth = birth;
            //ID = data.InsertAdmin(this);
        }

        public Admin(int id, string name, string email, string passwordHash, DateOnly birth) : base(id, name, email, passwordHash, birth)
        {
        }

        public override string ToString()
        {
            return $"ADMIN: {ID} - {Name} - {Email} - {Birth}";
        }
    }
}
