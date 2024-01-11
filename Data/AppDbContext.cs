using Microsoft.EntityFrameworkCore;
using Thesis.courseWebApp.Backend.Data;
using Thesis.courseWebApp.Backend.Models;

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
        public DbSet<UserSession> UserSessions { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure entity relationships and constraints here
            modelBuilder.Entity<Course>().HasKey(e => e.Id);
            modelBuilder.Entity<User>().HasKey(e => e.Id);
            modelBuilder.Entity<UserSession>()
                .HasOne(us => us.User)
                .WithOne(u => u.Session)
                .HasForeignKey<UserSession>(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            modelBuilder.Entity<Message>().HasKey(e => e.Id);

            // Add other configurations
            base.OnModelCreating(modelBuilder);
        }
    }

}

