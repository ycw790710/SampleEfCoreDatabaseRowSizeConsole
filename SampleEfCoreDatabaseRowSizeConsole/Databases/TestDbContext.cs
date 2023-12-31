﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SampleEfCoreDatabaseRowSizeConsole.Databases.Models;
using SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;
using System.Diagnostics;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases;

public class TestDbContext : DbContext
{
    private SqlFuncHelperBuilder sqlFuncHelperBuilder;

    public DbSet<User> Users { get; set; }
    public DbSet<FileStore> FileStores { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }
    public DbSet<UserNotificationItem> UserNotificationItems { get; set; }
    public DbSet<UserNotificationDetail> UserNotificationDetails { get; set; }

    public TestDbContext(DbContextOptions options) : base(options)
    {
        sqlFuncHelperBuilder = new();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(
            (str) => Debug.WriteLine(str), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Information);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        sqlFuncHelperBuilder.SetModelBuilder(modelBuilder,
            Database.ProviderName ?? throw new Exception("Empty ProviderName"));
        SetModels(modelBuilder);
    }

    private static void SetModels(ModelBuilder modelBuilder)
    {
        var userB = modelBuilder.Entity<User>();
        userB.ToTable(nameof(Users));
        userB.HasKey(x => x.Id);
        userB.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        var fileStoreB = modelBuilder.Entity<FileStore>();
        fileStoreB.ToTable(nameof(FileStores));
        fileStoreB.HasKey(x => x.Id);
        fileStoreB.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        fileStoreB
            .Property<byte[]>("_data")
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .HasColumnName("Data")
            .IsRequired();

        fileStoreB.Property(x => x.UserId);

        var userNotificationB = modelBuilder.Entity<UserNotification>();
        userNotificationB.ToTable(nameof(UserNotifications));
        userNotificationB.HasKey(x => x.Id);
        userNotificationB.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        userNotificationB.Property(x => x.Message);
        userNotificationB.Property(x => x.UserId);

        userNotificationB.Property(o => o.Type)
            .UsePropertyAccessMode(PropertyAccessMode.Field)
            .IsRequired()
            .HasConversion(
            v => v.Id,
            v => Enumeration.FromValue<UserNotificationType>(v)
            );

        userNotificationB
            .HasMany(n => n.UserNotificationItems)
            .WithOne()
            .HasForeignKey(n => n.UserNotificationId);

        var userNotificationItemB = modelBuilder.Entity<UserNotificationItem>();
        userNotificationItemB.ToTable(nameof(UserNotificationItem));
        userNotificationItemB.HasKey(x => x.Id);
        userNotificationItemB.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        userNotificationItemB.Property(x => x.Detail);

        var userNotificationDetailB = modelBuilder.Entity<UserNotificationDetail>();
        userNotificationDetailB.ToTable(nameof(UserNotificationDetail));
        userNotificationDetailB.HasKey(x => x.Id);
        userNotificationDetailB.Property(x => x.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();
        userNotificationDetailB.Property(x => x.Content);
    }

}