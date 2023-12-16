using Microsoft.EntityFrameworkCore;

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
        // Configure your entity relationships and constraints here
        modelBuilder.Entity<YourEntity>().HasKey(e => e.Id);
        // Add other configurations
    }
}
