// Models/CoursenixContext.cs
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Coursenix.Models
{
    public class CoursenixContext : DbContext
    {
        public CoursenixContext(DbContextOptions<CoursenixContext> options)
            : base(options)
        {
        }

        // DbSets for all entities
        public DbSet<Student> Students { get; set; }
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Attendance> Attendances { get; set; }


        // أضف أو قم بتعديل هذه الدالة
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // من المهم استدعاء الدالة الأساسية أولاً

            // تحديد الدقة والمقياس للخاصية Price في الكيان Subject
            modelBuilder.Entity<Subject>()
                .Property(s => s.Price)
                .HasPrecision(18, 2);

            // مثال: تحديد العلاقات بين الكيانات بشكل صريح إذا لم يتم اكتشافها تلقائياً أو لتخصيصها
            modelBuilder.Entity<Subject>()
               .HasOne(s => s.Teacher)
               .WithMany(t => t.Subjects)
               .HasForeignKey(s => s.TeacherId);

            modelBuilder.Entity<Group>()
               .HasOne(g => g.Subject)
               .WithMany(s => s.Groups)
               .HasForeignKey(g => g.SubjectId);

            // ... إلخ لعلاقات Booking, Session, Attendance

        }
    }
}