using Microsoft.EntityFrameworkCore;
using WL.Host.Entities;

namespace WL.Host.DbContexts;

public class WishesContext : DbContext
{
    public WishesContext(DbContextOptions<WishesContext> contextOptions)
        : base(contextOptions)
    {

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Wish>()
            .HasData(
                new Wish()
                {
                    Id = Guid.NewGuid(),
                    Name = "первое желание",
                    Description = "Description of first willing",
                    WishCategoryId = 1
                },
                new Wish()
                {
                    Id = Guid.NewGuid(),
                    Name = "второе желание",
                    Description = "Description of second willing",
                    WishCategoryId = 2
                },
                new Wish()
                {
                    Id = Guid.NewGuid(),
                    Name = "третье желание",
                    Description = "Description of third willing",
                    WishCategoryId = 2
                });
        modelBuilder.Entity<WishCategory>()
            .HasData(
                new WishCategory()
                {
                    Id = 1,
                    Name = "Best",
                },
                new WishCategory()
                {
                    Id = 2,
                    Name = "So so",
                });
        base.OnModelCreating(modelBuilder);
    }

    public DbSet<Wish> Wishes { get; set; } = null!;
    public DbSet<WishCategory> WithCategories { get; set; } = null!;
}