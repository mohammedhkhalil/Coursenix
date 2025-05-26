using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Coursenix.Models
{
    public class Context : IdentityDbContext<AppUser>
    {
        public Context() { }

        // Constructor to accept DbContextOptions for dependency injection
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<ResetCode> ResetCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Booking>()
              .HasOne(b => b.Student)
              .WithMany()
              .HasForeignKey(b => b.StudentId)
              .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Booking>()
                 .HasOne(b => b.Group)
                 .WithMany()
                 .HasForeignKey(b => b.GroupId)
                 .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Attendance>()
               .HasOne(a => a.Student)
               .WithMany()
               .HasForeignKey(a => a.StudentId)
               .OnDelete(DeleteBehavior.Restrict);
            // One-to-One: Student ↔ AppUser
            modelBuilder.Entity<Student>()
                .HasOne(s => s.AppUser)
                .WithOne()
                .HasForeignKey<Student>(s => s.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-One: Teacher ↔ AppUser
            modelBuilder.Entity<Teacher>()
                .HasOne(t => t.AppUser)
                .WithOne()
                .HasForeignKey<Teacher>(t => t.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Subject → Teacher
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Subjects)
                .HasForeignKey(s => s.TeacherId);

            // One-to-Many: Subject → Groups
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Groups)
                .WithOne(g => g.Subject)
                .HasForeignKey(g => g.SubjectId);

            // Decimal Precision: Subject.Price
            modelBuilder.Entity<Subject>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18, 2)");

            // Define the one-to-many relationship: One Group → Many GroupDays
            modelBuilder.Entity<GroupDay>()
                .HasOne(gd => gd.Group)
                .WithMany(g => g.GroupDays)
                .HasForeignKey(gd => gd.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Store the enum (WeekDay) as an integer in the database – explicitly stated for clarity
            modelBuilder.Entity<GroupDay>()
                .Property(gd => gd.Day)
                .HasConversion<int>();

            // SeedData(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {

            optionsBuilder.UseSqlServer(@"Data Source=LAPTOP-0RH6BA2F\SQLEXPRESS;Initial Catalog=TestCoursenix;Integrated Security=True; TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }


        /*
        private void SeedData(ModelBuilder modelBuilder)
        {
            // Add initial data here
        }
        */
    }
}
