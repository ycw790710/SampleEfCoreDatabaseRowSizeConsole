﻿using SampleEfCoreDatabaseRowSizeConsole.Databases.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases;

public class TestDbContext : DbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<FileStore> FileStores { get; set; }
    public DbSet<UserNotification> UserNotifications { get; set; }

    public TestDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(
            (str) => Debug.WriteLine(str), new[] { DbLoggerCategory.Database.Command.Name }, LogLevel.Trace);
    }

    public int NpgsqlPgColumnSize_Int(int column)
    {
        throw new NotImplementedException();
    }
    public int NpgsqlPgColumnSize_String(string column)
    {
        throw new NotImplementedException();
    }
    public int NpgsqlPgColumnSize_Bytes(byte[] column)
    {
        throw new NotImplementedException();
    }

    public long SqlServerDATALENGTH_Int(int column)
    {
        throw new NotImplementedException();
    }
    public long SqlServerDATALENGTH_String(string column)
    {
        throw new NotImplementedException();
    }
    public long SqlServerDATALENGTH_Bytes(byte[] column)
    {
        throw new NotImplementedException();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(NpgsqlPgColumnSize_Int)))
           .HasTranslation(TranslationNpgsqlPgColumnSize());
        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(NpgsqlPgColumnSize_String)))
           .HasTranslation(TranslationNpgsqlPgColumnSize());
        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(NpgsqlPgColumnSize_Bytes)))
           .HasTranslation(TranslationNpgsqlPgColumnSize());

        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(SqlServerDATALENGTH_Int)))
           .HasTranslation(TranslationSqlServerDATALENGTH());
        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(SqlServerDATALENGTH_String)))
           .HasTranslation(TranslationSqlServerDATALENGTH());
        modelBuilder.HasDbFunction(GetType().GetMethod(nameof(SqlServerDATALENGTH_Bytes)))
           .HasTranslation(TranslationSqlServerDATALENGTH());

        SetModels(modelBuilder);

    }

    private static Func<IReadOnlyList<SqlExpression>, SqlExpression> TranslationNpgsqlPgColumnSize()
    {
        return args =>
        {
            var functionName = "pg_column_size";
            if (args.Count != 1)
                throw new ArgumentException($"{functionName} function expects one argument");

            var firstArg = args.First();
            ColumnExpression columnExpression = firstArg as ColumnExpression;
            if (columnExpression == null)
                columnExpression = (firstArg as SqlUnaryExpression)?.Operand as ColumnExpression;
            if (columnExpression == null)
                throw new Exception($"Unhandled arg type:{firstArg.GetType()}");
            var sqlArguments = new SqlExpression[] { columnExpression };
            var returnType = typeof(int);

            var sqlFunctionExpression = new SqlFunctionExpression(
                functionName,
                sqlArguments,
                false,
                new[] { false, false },
                returnType,
                null);

            return sqlFunctionExpression;
        };
    }

    private static Func<IReadOnlyList<SqlExpression>, SqlExpression> TranslationSqlServerDATALENGTH()
    {
        return args =>
        {
            var functionName = "DATALENGTH";
            if (args.Count != 1)
                throw new ArgumentException($"{functionName} function expects one argument");

            var firstArg = args.First();
            ColumnExpression columnExpression = firstArg as ColumnExpression;
            if (columnExpression == null)
                columnExpression = (firstArg as SqlUnaryExpression)?.Operand as ColumnExpression;
            if (columnExpression == null)
                throw new Exception($"Unhandled arg type:{firstArg.GetType()}");
            var sqlArguments = new SqlExpression[] { columnExpression };
            var returnType = typeof(long);

            var sqlFunctionExpression = new SqlFunctionExpression(
                functionName,
                sqlArguments,
                false,
                new[] { false, false },
                returnType,
                null);

            return sqlFunctionExpression;
        };
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
    }
}