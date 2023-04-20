using Labb_2_LINQ.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Labb_2_LINQ.Data
{
    internal class SchoolDbContext : DbContext
    {
        public DbSet<Teacher> Teachers { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Course> Courses { get; set; } 
        public DbSet<Student> Students { get; set; }
        public DbSet<TeacherSubject> TeachersSubjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            //dbContextOptionsBuilder.UseSqlServer("DESKTOP-CVJK2UD");
            dbContextOptionsBuilder.UseSqlServer("Server=DESKTOP-CVJK2UD;Database=SchoolDb;Trusted_Connection=True;TrustServerCertificate=True;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TeacherSubject>().HasKey(ts => new { ts.TeacherID, ts.SubjectID });
        }
    }
}
