using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HolidayManagerWeb.Models;
using Microsoft.EntityFrameworkCore;

namespace HolidayManagerWeb
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Holiday> Holidays { get; set; }
        public DbSet<LeaveRequest> LeaveRequests { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Transaction> Transactions { get; set; }


        public AppDbContext() { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connectionString = "server=localhost;port=3308;database=holidaymanager;user=root;password=;";
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }



        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasKey(u => u.ID);
            modelBuilder.Entity<Admin>().HasKey(a => a.ID);
            modelBuilder.Entity<Holiday>().HasKey(h => h.HolidayId);
            modelBuilder.Entity<LeaveRequest>().HasKey(l => l.LeaveRequestId);
            modelBuilder.Entity<Trip>().HasKey(t => t.TripId);
            modelBuilder.Entity<Document>().HasKey(d => d.DocumentId);

        }
    }
}

