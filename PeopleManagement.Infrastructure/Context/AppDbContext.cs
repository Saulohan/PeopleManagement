using Microsoft.EntityFrameworkCore;
using PeopleManagement.Domain.Entities;
using PeopleManagement.Infrastructure.Configuration;
using System.Reflection;

namespace PeopleManagement.Infrastructure.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Person> People { get; set; } = null!;
        public DbSet<Token> Tokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}