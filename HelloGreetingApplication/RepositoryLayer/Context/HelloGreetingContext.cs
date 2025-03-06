using System;
using ModelLayer.Model;
using Microsoft.EntityFrameworkCore;
namespace RepositoryLayer.Context
{
	public class HelloGreetingContext:DbContext
	{
		public HelloGreetingContext(DbContextOptions<HelloGreetingContext> options) : base(options) { }
		public DbSet<GreetingModel> Greetings { get; set;}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=localhost;Database=GreetingDb;User Id=sa;Password=StrongPassword@123;TrustServerCertificate=True;");
            }
        }

    }
}

