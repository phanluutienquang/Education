using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using MyEducation.MyDomain.Entities.Users;
using MyEducation.MyIdentity.Models.Auth;

namespace MyEducation.Infra;
public partial class SmartCertifyContext : DbContext
{
    public SmartCertifyContext(DbContextOptions<SmartCertifyContext> options)
        : base(options)
    {
    }

    public virtual DbSet<UserProfile> UserProfiles { get; set; }
    
    // Auth Entities
    public virtual DbSet<ClientSource> ClientSources { get; set; }
    public virtual DbSet<TokenPair> TokenPairs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<UserProfile>(entity =>
        {
            entity.HasKey(e => e.UserId);

            entity.Property(e => e.DisplayName).HasMaxLength(100);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(100);
        });
        
        // Configure ClientSource entity
        modelBuilder.Entity<ClientSource>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.ClientId).IsUnique();
            
            entity.Property(e => e.ClientId).IsRequired().HasMaxLength(100);
            entity.Property(e => e.ClientName).IsRequired().HasMaxLength(200);
            entity.Property(e => e.ApiSecret).IsRequired().HasMaxLength(500);
            entity.Property(e => e.Metadata).HasMaxLength(4000);
            
            // Store List<string> as comma-separated string or JSON
            entity.Property(e => e.AllowedScopes)
                  .HasConversion(
                      v => string.Join(",", v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                  .HasMaxLength(2000);
                  
            entity.Property(e => e.AllowedIPs)
                  .HasConversion(
                      v => string.Join(",", v),
                      v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList())
                  .HasMaxLength(2000);
            
            entity.Property(e => e.IsEnabled).HasDefaultValue(true);
            entity.Property(e => e.ValidFrom).HasDefaultValueSql("NOW()");
            entity.Property(e => e.RateLimitPerMinute).HasDefaultValue(100);
            
            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
        
        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            entity.HasIndex(e => e.Token).IsUnique();
            entity.HasIndex(e => e.JwtId);
            
            entity.Property(e => e.Token).IsRequired().HasMaxLength(500);
            entity.Property(e => e.JwtId).IsRequired().HasMaxLength(100);
            
            entity.HasOne(e => e.ClientSource)
                  .WithMany()
                  .HasForeignKey(e => e.ClientSourceId)
                  .OnDelete(DeleteBehavior.Cascade);
            
            // Soft delete filter
            entity.HasQueryFilter(e => !e.IsDeleted);
        });
    }
}