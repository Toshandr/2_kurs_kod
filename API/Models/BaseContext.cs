using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace API.Models;

public partial class BaseContext : DbContext
{
    public BaseContext()
    {
    }

    public BaseContext(DbContextOptions<BaseContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var connectionString = Environment.GetEnvironmentVariable("PATH_TO_DB");
        if (string.IsNullOrEmpty(connectionString))
        {
            Console.WriteLine("WARNING: PATH_TO_DB environment variable is not set!");
            throw new InvalidOperationException("PATH_TO_DB environment variable is not set");
        }
        var host = GetHostFromConnectionString(connectionString);
        Console.WriteLine($"Подключение к БД: Host={host}");
        optionsBuilder.UseNpgsql(connectionString);
    }
    
    private static string GetHostFromConnectionString(string connectionString)
    {
        var parts = connectionString.Split(';');
        var hostPart = parts.FirstOrDefault(p => p.StartsWith("Host=", StringComparison.OrdinalIgnoreCase));
        if (hostPart != null)
        {
            return hostPart.Substring(5); // Убираем "Host="
        }
        return "unknown";
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Age).HasColumnName("age");
            entity.Property(e => e.CityLater)
                .HasColumnType("character varying")
                .HasColumnName("city_later");
            entity.Property(e => e.CityNow)
                .HasColumnType("character varying")
                .HasColumnName("city_now");
            entity.Property(e => e.Name)
                .HasColumnType("character varying")
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasMaxLength(255)
                .HasColumnName("role");
            entity.Property(e => e.TelegramTeg)
                .HasColumnType("character varying")
                .HasColumnName("telegram_teg");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
