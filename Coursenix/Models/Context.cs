// Models/Context.cs - Corrected with OnDelete(DeleteBehavior.Restrict) for Subject-Group

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic; // Required for ICollection
using System.Globalization; // Required for CultureInfo

namespace Coursenix.Models
{
    public class Context : DbContext // Ensure you inherit from DbContext
    {
        // Constructor to accept DbContextOptions for dependency injection
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        // DbSet properties for your entities. These represent the tables in your database.
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Student> Students { get; set; } // Assuming you have a Student model
        public DbSet<Group> Groups { get; set; }

        // You would add other DbSet properties here for any other models (e.g., Enrollments, etc.)

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships

            // Subject to Teacher (One-to-Many)
            modelBuilder.Entity<Subject>()
                .HasOne(s => s.Teacher) // Each Subject has one Teacher
                .WithMany(t => t.Subjects) // Each Teacher can have many Subjects
                .HasForeignKey(s => s.TeacherId);
            // Keep this as default cascade or set to Restrict based on preference
            // .OnDelete(DeleteBehavior.Restrict);


            // Subject to Group (One-to-Many) - Keep this configuration
            modelBuilder.Entity<Subject>()
                .HasMany(s => s.Groups)
                .WithOne(g => g.Subject)
                .HasForeignKey(g => g.SubjectId);
            // The DeleteBehavior.Restrict you added here was part of the fix attempt
            // for the cascade issue. You might want to keep it or let it default.
            // Leaving it as Restrict is generally safer if you don't want Groups deleted
            // automatically when a Subject is deleted.
            // .OnDelete(DeleteBehavior.Restrict); // <-- Keep this line if you want to restrict Subject deletion


            // --- Removed: Group to Teacher relationship configuration ---
            // Because TeacherId is removed from Group.cs, this relationship is removed.
            /*
             modelBuilder.Entity<Group>()
                 .HasOne(g => g.Teacher)
                 .WithMany(t => t.Groups)
                 .HasForeignKey(g => g.TeacherId);
                 // .OnDelete(DeleteBehavior.Restrict);
            */
            // --- End Removed ---


            // Configure precision for decimal properties if needed
            modelBuilder.Entity<Subject>()
                .Property(s => s.Price)
                .HasColumnType("decimal(18, 2)");

            // Seed Data
            SeedData(modelBuilder);
        }

        private static void SeedData(ModelBuilder modelBuilder)
        {
            // Seed Teachers
            modelBuilder.Entity<Teacher>().HasData(
                new Teacher
                {
                    TeacherId = 1, // Explicitly set IDs
                    Name = "Jane",
                    Email = "jane.doe@example.com",
                    PhoneNumber = "555-1234",
                    Password= "Password",
                    ProfilePicture="asdfasdf"
                },
                new Teacher
                {
                    TeacherId = 2,
                    Name = "John",
                    Email = "john.smith@example.com",
                    PhoneNumber = "555-5678",
                    Password = "asdf",
                    ProfilePicture="asdfasdf"
                }
            );

            // Seed Subjects
            modelBuilder.Entity<Subject>().HasData(
                new Subject
                {
                    SubjectId = 1, // Explicitly set IDs
                    SubjectName = "Mathematics",
                    Price = 150,
                    Description = "Comprehensive math course for grade 8.",
                    GradeLevel = 8,
                    Location = "Room 101",
                    StudentsPerGroup = 25,
                    TeacherId = 1,
                    ThumbnailFileName = "asdfasdfasd"
                },
                 new Subject
                 {
                     SubjectId = 2, // Explicitly set IDs
                     SubjectName = "Science",
                     Price = 180,
                     Description = "Interactive science lessons for grade 7.",
                     GradeLevel = 7,
                     Location = "Lab B",
                     StudentsPerGroup = 20,
                     TeacherId = 2,
                     ThumbnailFileName="Asdfasdf"
                 }
            );

            // Seed Groups (assuming TeacherId is added to Group)
            modelBuilder.Entity<Group>().HasData(
                new Group
                {
                    GroupId = 1, // Explicitly set IDs
                    SubjectId = 1,
                    //acherId = 1, // Assign TeacherId if you added it to Group model
                    Grade = 8,
                    GroupName = "Math Group A",
                    DayOfWeek = "Monday & Wednesday",
                    StartTime = DateTime.Today.AddHours(9).AddMinutes(0),
                    EndTime = DateTime.Today.AddHours(10).AddMinutes(30),
                    TotalSeats = 25,
                    EnrolledStudentsCount = 0,
                    Location = "Room 101"
                },
                 new Group
                 {
                     GroupId = 2, // Explicitly set IDs
                     SubjectId = 1,
           //        TeacherId = 1, // Assign TeacherId if you added it to Group model
                     Grade = 8,
                     GroupName = "Math Group B",
                     DayOfWeek = "Tuesday & Thursday",
                     StartTime = DateTime.Today.AddHours(14).AddMinutes(0),
                     EndTime = DateTime.Today.AddHours(15).AddMinutes(30),
                     TotalSeats = 25,
                     EnrolledStudentsCount = 0,
                     Location = "Room 101"
                 }
            );



            // Seed Students - Assuming ParentNumber is required
            modelBuilder.Entity<Student>().HasData(
                new Student
                {
                    StudentId = 1, // Explicitly set IDs
                    Name = "Alice",
                    Email = "alice.smith@example.com",
                    PhoneNumber = "555-9876",
                    GradeLevel = 8,
                    ParentNumber = "555-1111",// Provided ParentNumber
                    Password = "asdfsadfasdf"

                },
                new Student
                {
                    StudentId = 2,
                    Name = "Bob",
                    Email = "bob.j@example.com",
                    PhoneNumber = "555-4321",
                    GradeLevel = 7,
                    ParentNumber = "555-2222",// Provided ParentNumber
                    Password = "sdfasdf"
                }
            );
        }
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=.;Initial Catalog=TestCoursenix;Integrated Security=True; TrustServerCertificate=True;");
            base.OnConfiguring(optionsBuilder);
        }
    }
}