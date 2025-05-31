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
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }
        public DbSet<ResetCode> ResetCodes { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<GradeLevel> GradeLevels { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Booking to Student: One-to-Many
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Student)
                .WithMany(s => s.Bookings)
                .HasForeignKey(b => b.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Booking to Group: One-to-Many
            modelBuilder.Entity<Booking>()
                .HasOne(b => b.Group)
                .WithMany(g => g.Bookings)
                .HasForeignKey(b => b.GroupId)
                .OnDelete(DeleteBehavior.Cascade);

            // Attendance to Student: One-to-Many
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Student)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Attendance to Session: One-to-Many
            modelBuilder.Entity<Attendance>()
                .HasOne(a => a.Session)
                .WithMany(s => s.Attendances)
                .HasForeignKey(a => a.SessionId)
                .OnDelete(DeleteBehavior.Cascade);

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

            // One-to-One: Admin ↔ AppUser (assumed, adjust if not needed)
            modelBuilder.Entity<Admin>()
                .HasOne(a => a.AppUser)
                .WithOne()
                .HasForeignKey<Admin>(a => a.AppUserId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Course → Teacher
            modelBuilder.Entity<Course>()
                .HasOne(s => s.Teacher)
                .WithMany(t => t.Courses)
                .HasForeignKey(s => s.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // One-to-Many: Course → GradeLevel
            modelBuilder.Entity<GradeLevel>()
                .HasOne(gl => gl.Course)
                .WithMany(c => c.GradeLevels)
                .HasForeignKey(gl => gl.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: GradeLevel → Group
            modelBuilder.Entity<Group>()
                .HasOne(g => g.GradeLevel)
                .WithMany(gl => gl.Groups)
                .HasForeignKey(g => g.GradeLevelId)
                .OnDelete(DeleteBehavior.Cascade);

            // One-to-Many: Group → Session
            modelBuilder.Entity<Session>()
                .HasOne(s => s.Group)
                .WithMany(g => g.Sessions)
                .HasForeignKey(s => s.GroupId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Group>()
       .Property(g => g.StartTime)
       .HasColumnType("datetime2"); // Use datetime2 for DateTime properties

            modelBuilder.Entity<Group>()
                .Property(g => g.EndTime)
                .HasColumnType("datetime2"); // Use datetime2 for DateTime properties
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=TestCoursenix;Integrated Security=True;TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}