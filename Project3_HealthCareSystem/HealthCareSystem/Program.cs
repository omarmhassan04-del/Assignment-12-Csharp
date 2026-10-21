using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HealthCareSystem
{
    public class Patient
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime DateOfBirth { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }

    public class Doctor
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Specialization { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
    }

    public class Appointment
    {
        public int PatientId { get; set; }
        public Patient Patient { get; set; }

        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }

        public DateTime AppointmentDate { get; set; }
    }

    public class HealthCareDbContext : DbContext
    {
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Appointment> Appointments { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=HealthCareDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Appointment>()
                .HasKey(a => new { a.PatientId, a.DoctorId, a.AppointmentDate });

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Doctor)
                .WithMany(d => d.Appointments)
                .HasForeignKey(a => a.DoctorId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Patient>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Doctor>()
                .Property(d => d.Specialization)
                .IsRequired()
                .HasMaxLength(100);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new HealthCareDbContext();

            Console.WriteLine("Health Care System Initialized Successfully.");
            Console.WriteLine("Configured Entities:");
            Console.WriteLine($"- {nameof(Patient)} (Id, Name, DateOfBirth)");
            Console.WriteLine($"- {nameof(Doctor)} (Id, Name, Specialization)");
            Console.WriteLine($"- {nameof(Appointment)} (PatientId, DoctorId, AppointmentDate)");
            Console.WriteLine("Relationship:");
            Console.WriteLine("- Many-to-Many between Patient and Doctor mapped through Appointment entity with Navigation Properties.");

            var script = context.Database.GenerateCreateScript();
            Console.WriteLine("\nGenerated Database DDL Script:");
            Console.WriteLine(script);
        }
    }
}
