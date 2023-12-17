using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Data;

namespace Thesis.courseWebApp.Backend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Define DbSet properties for your entities
        public DbSet<Course> Courses { get; set; }
        public DbSet<User> Users { get; set; }
        // Add other DbSet properties as needed

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints here
            modelBuilder.Entity<Course>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            // Add other configurations
            base.OnModelCreating(modelBuilder);
        }
    }

}
