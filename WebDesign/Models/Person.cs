using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using HolidayManagerWeb.Models;

namespace HolidayManagerWeb.Models
{
    public class Person
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; private set; }
        public DateOnly Birth { get; set; }
        public int ID { get; set; }

        public Person()
        {
            /*Name = "John Doe";
            Email = "johndoe@email.com";
            Password = "johnny123";
            Birth = new DateOnly(2000, 1, 1);*/
        }

        /*public Person(string name, string email, string passwordHash, DateOnly birth)
        {
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Birth = birth;
        }*/

        public Person(int id, string name, string email, string passwordHash, DateOnly birth)
        {
            ID = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Birth = birth;
        }

        public void SetPassword(string password)
        {
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password);
        }

        public bool VerifyPassword(string input)
        {
            return BCrypt.Net.BCrypt.Verify(input, PasswordHash);
        }

        public override string ToString()
        {
            return $"{Name} - {Email} - {Birth}";
        }
    }
}
