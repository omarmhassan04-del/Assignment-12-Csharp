using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace EcommerceSystem
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public ICollection<Product> Products { get; set; } = new HashSet<Product>();
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }

        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }

        public ICollection<Order> Orders { get; set; } = new HashSet<Order>();
    }

    public class Order
    {
        public int Id { get; set; }
        public DateTime OrderDate { get; set; }

        public int CustomerId { get; set; }
        public Customer Customer { get; set; }

        public ICollection<OrderDetail> OrderDetails { get; set; } = new HashSet<OrderDetail>();
    }

    public class OrderDetail
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }

        public int Quantity { get; set; }
    }

    public class EcommerceDbContext : DbContext
    {
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=EcommerceDB;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Product>()
                .Property(p => p.Price)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Customer>()
                .Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Customer>()
                .Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(150);

            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasKey(od => new { od.OrderId, od.ProductId });

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany(p => p.OrderDetails)
                .HasForeignKey(od => od.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }

    internal class Program
    {
        static void Main(string[] args)
        {
            using var context = new EcommerceDbContext();

            Console.WriteLine("E-commerce System Initialized Successfully.");
            Console.WriteLine("Configured Entities:");
            Console.WriteLine($"- {nameof(Category)} (Id, Name)");
            Console.WriteLine($"- {nameof(Product)} (Id, Name, Price, CategoryId)");
            Console.WriteLine($"- {nameof(Customer)} (Id, Name, Email)");
            Console.WriteLine($"- {nameof(Order)} (Id, OrderDate, CustomerId)");
            Console.WriteLine($"- {nameof(OrderDetail)} (OrderId, ProductId, Quantity)");
            Console.WriteLine("Mapped Relationships:");
            Console.WriteLine("- One-to-Many: Category to Product");
            Console.WriteLine("- Many-to-Many: Order to Product through OrderDetail");
            Console.WriteLine("- One-to-Many: Customer to Order");

            var script = context.Database.GenerateCreateScript();
            Console.WriteLine("\nGenerated Database DDL Script:");
            Console.WriteLine(script);
        }
    }
}
