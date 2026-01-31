using Microsoft.EntityFrameworkCore;
using Probbot.Database;

namespace Probbot.Database;

public class AppDbContext : DbContext
{
    public DbSet<User> Users {get; set;}

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source = Probbot.db");
    }

    public override int SaveChanges()
    {
        var entries = ChangeTracker.Entries().Where(e => e.Entity is User && (e.State == Entity.Added || e.State == EntityState.Modified));

        foreach (var entityEntry in entries)
        {
            ((User)entityEntry.Entity).LastUpdateAt = DateTime.UtcNow;

            if (entityEntry.State == EntityState.Added)
            {
                ((User)entityEntry.Entity).CreatedAt = DateTime.UtcNow;
            }
        }

        return base.SaveChanges();
    }
}