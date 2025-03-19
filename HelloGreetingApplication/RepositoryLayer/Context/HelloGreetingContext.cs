using System;
using ModelLayer.Model;
using Microsoft.EntityFrameworkCore;
using RepositoryLayer.Services;

namespace RepositoryLayer.Context
{
	public class HelloGreetingContext:DbContext
	{
		public HelloGreetingContext(DbContextOptions<HelloGreetingContext> options) : base(options) { }
		public DbSet<GreetingModel> Greetings { get; set;}
        public DbSet<User> Users { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=GreetingDatabase;User Id=sa;Password=StrongPassword@123;TrustServerCertificate=True;");
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<GreetingModel>()
                .HasOne(g => g.User)  // Greeting belongs to User
                .WithMany(u => u.Greetings)  // User has many Greetings
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade); // If user is deleted, delete greetings

            base.OnModelCreating(modelBuilder);
        }

    }
}

