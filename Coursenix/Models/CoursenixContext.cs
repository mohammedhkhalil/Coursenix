using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Coursenix.Models
{
    public class CoursenixContext : DbContext
    {

        // DbSets for all entities
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseSqlServer(@"Data Source=HISHAMSAYED;Initial Catalog=Coursenix1;Integrated Security=True");
            //base.OnConfiguring(optionsBuilder);
        }
    }
}
