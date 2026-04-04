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
        //modelBuilder.Entity<User>()
        //    .HasIndex(u => u.Username)
        //    .IsUnique();

        //modelBuilder.Entity<User>()
        //.HasMany(u => u.Products);

        //modelBuilder.Entity<Sale>()
        //    .HasOne<User>()
        //    .WithMany()
        //    .HasForeignKey(s => s.UserId)
        //    .OnDelete(DeleteBehavior.SetNull);

        base.OnModelCreating(modelBuilder);
    }

}
