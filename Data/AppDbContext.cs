using System;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    
    public DbSet<Account> Accounts { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Sale> Sales { get; set; }
    public DbSet<Sale_Detail> Sale_Details { get; set; }
    public DbSet<Archive> Archives { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>()
            .Property(p => p.Price)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Archive>()
            .Property(a => a.Price)
            .HasPrecision(18, 2);

        base.OnModelCreating(modelBuilder);
    }

    internal async Task SaveChangesAsync(Account account)
    {
        throw new NotImplementedException();
    }
}
