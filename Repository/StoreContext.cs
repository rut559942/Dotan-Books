using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using Entities;

namespace Repository
{

    public class StoreContext : DbContext
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options) { }

        // 1. רשימת הטבלאות
        public DbSet<Book> Books { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Promotion> Promotions { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Customer> Customers { get; set; }

        // 2. עיצוב המודל (Fluent API)
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Order>()
                .Property(o => o.OrderNumber)
                .HasComputedColumnSql("'DS' + CAST([Id] + 465500 AS VARCHAR(20))", stored: true);

            // הגדרת דיוק למחירים - קריטי בחנות!
            modelBuilder.Entity<Book>()
                .Property(b => b.Price)
                .HasColumnType("decimal(18,2)");

            // הגדרת קשר "רבים לאחד" בין ספר למבצע
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Promotion)
                .WithMany(p => p.Books)
                .HasForeignKey(b => b.PromotionId)
                .OnDelete(DeleteBehavior.SetNull); // אם מוחקים מבצע, הספר נשאר - רק המבצע מתבטל

            // יצירת אינדקס על שם הספר לחיפוש מהיר יותר
            modelBuilder.Entity<Book>()
                .HasIndex(b => b.Title);

            // הגדרת אימייל ייחודי ללקוחות
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();

            // הגדרת קשר "אחד לרבים" בין לקוח להזמנות
            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // הגדרת דיוק למחירים בהזמנות
            modelBuilder.Entity<Order>()
                .Property(o => o.TotalAmount)
                .HasColumnType("decimal(18,2)");
        }
    }
}