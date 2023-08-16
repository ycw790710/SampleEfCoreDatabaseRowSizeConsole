﻿using Dapper;
using SampleEfCoreDatabaseRowSizeConsole.Databases;
using SampleEfCoreDatabaseRowSizeConsole.Databases.Models;
using Microsoft.EntityFrameworkCore;
using static Dapper.SqlMapper;

namespace SampleEfCoreDatabaseRowSizeConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {

            var showSqlServerTest = true;
            var showPostgreSqlTest = true;

            var databaseName = "SampleEfCoreDatabaseRowSizeDatabase";
            var sqlServerConnectionStr = $"Server=(localdb)\\mssqllocaldb;Database={databaseName};Trusted_Connection=True;MultipleActiveResultSets=true";
            var postgreSqlUserName = "";
            var postgreSqlPassword = "";

            var postgreSqlConnectionStr = $"Host=localhost; Database={databaseName}; Username={postgreSqlUserName}; Password={postgreSqlPassword}";

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"將會刪除並重建Database:{databaseName}, Enter繼續, 其他按鍵取消");
            Console.ResetColor();
            if (Console.ReadKey().Key != ConsoleKey.Enter)
                return;
            Console.WriteLine();

            if (showSqlServerTest)
                RunSqlServerTest(sqlServerConnectionStr);
            if (showPostgreSqlTest)
                RunPostgreSqlTest(postgreSqlConnectionStr);

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("---測試結束---");
            Console.ResetColor();
            Console.ReadLine();
        }

        private static void RunPostgreSqlTest(string postgreSQLConnectionStr)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-----Run PostgreSql Test-----");
                Console.ResetColor();

                var postgreSQLDbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                        .UseNpgsql(postgreSQLConnectionStr);

                using (var dbContextNpgsql = new TestDbContext(postgreSQLDbContextOptions.Options))
                {
                    var dbContext = dbContextNpgsql;
                    InitializeData(dbContext);

                    var user = dbContext.Users.First();

                    var dbConnection = dbContext.Database.GetDbConnection();

                    var userRowSize = dbConnection.QuerySingle<int>(
                        NpgsqlGetSize<User>(dbContext, $@"WHERE t.""Id"" = {user.Id}"));
                    var fileStoreRowSize = dbConnection.QuerySingle<int>(
                        NpgsqlGetSize<FileStore>(dbContext, $@"WHERE t.""UserId"" = {user.Id}"));
                    var userNotificationRowSize = dbConnection.QuerySingle<int>(
                        NpgsqlGetSize<UserNotification>(dbContext, $@"WHERE t.""UserId"" = {user.Id}"));

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Custom Command");
                    Console.ResetColor();

                    Console.WriteLine($"userRowSize:{userRowSize}");
                    Console.WriteLine($"fileStoreRowSize:{fileStoreRowSize}");
                    Console.WriteLine($"userNotificationRowSize:{userNotificationRowSize}");


                    var userRowSize2 = dbContext.Users
                        .Where(n => n.Id == user.Id)
                        .Sum(n => dbContext.NpgsqlPgColumnSize_Int(n.Id));
                    var fileStoreRowSize2 = dbContext.FileStores
                        .Where(n => n.UserId == user.Id)
                        .Sum(n => dbContext.NpgsqlPgColumnSize_Int(n.Id) +
                        dbContext.NpgsqlPgColumnSize_Int(n.UserId) +
                        dbContext.NpgsqlPgColumnSize_Bytes(n._data));
                    var userNotificationRowSize2 = dbContext.UserNotifications
                        .Where(n => n.UserId == user.Id)
                        .Sum(n => dbContext.NpgsqlPgColumnSize_Int(n.Id) +
                        dbContext.NpgsqlPgColumnSize_Int(n.UserId) +
                        dbContext.NpgsqlPgColumnSize_String(n.Message));

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("User-defined function mapping");
                    Console.ResetColor();

                    Console.WriteLine($"userRowSize2:{userRowSize2}");
                    Console.WriteLine($"fileStoreRowSize2:{fileStoreRowSize2}");
                    Console.WriteLine($"userNotificationRowSize2:{userNotificationRowSize2}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run PostgreSql Test failed.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        private static void RunSqlServerTest(string sqlServerConnectionStr)
        {
            try
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("-----Run SqlServer Test-----");
                Console.ResetColor();

                var sqlServerDbContextOptions = new DbContextOptionsBuilder<TestDbContext>()
                        .UseSqlServer(sqlServerConnectionStr);

                using (var dbContextMssql = new TestDbContext(sqlServerDbContextOptions.Options))
                {
                    var dbContext = dbContextMssql;
                    InitializeData(dbContext);

                    var user = dbContext.Users.First();

                    var dbConnection = dbContext.Database.GetDbConnection();
                    var userRowSize = dbConnection.QuerySingle<int>(
                        MssqlGetSize<User>(dbContext, $"WHERE t.Id = {user.Id}"));
                    var fileStoreRowSize = dbConnection.QuerySingle<int>(
                        MssqlGetSize<FileStore>(dbContext, $"WHERE t.UserId = {user.Id}"));
                    var userNotificationRowSize = dbConnection.QuerySingle<int>(
                        MssqlGetSize<UserNotification>(dbContext, $"WHERE t.UserId = {user.Id}"));

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Custom Command");
                    Console.ResetColor();

                    Console.WriteLine($"userRowSize:{userRowSize}");
                    Console.WriteLine($"fileStoreRowSize:{fileStoreRowSize}");
                    Console.WriteLine($"userNotificationRowSize:{userNotificationRowSize}");

                    var userRowSize2 = dbContext.Users
                        .Where(n => n.Id == user.Id)
                        .Sum(n => dbContext.SqlServerDATALENGTH_Int(n.Id));
                    var fileStoreRowSize2 = dbContext.FileStores
                        .Where(n => n.UserId == user.Id)
                        .Sum(n => dbContext.SqlServerDATALENGTH_Int(n.Id) +
                        dbContext.SqlServerDATALENGTH_Int(n.UserId) +
                        dbContext.SqlServerDATALENGTH_Bytes(n._data));
                    var userNotificationRowSize2 = dbContext.UserNotifications
                        .Where(n => n.UserId == user.Id)
                        .Sum(n => dbContext.SqlServerDATALENGTH_Int(n.Id) +
                        dbContext.SqlServerDATALENGTH_Int(n.UserId) +
                        dbContext.SqlServerDATALENGTH_String(n.Message));

                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("User-defined function mapping");
                    Console.ResetColor();

                    Console.WriteLine($"userRowSize2:{userRowSize2}");
                    Console.WriteLine($"fileStoreRowSize2:{fileStoreRowSize2}");
                    Console.WriteLine($"userNotificationRowSize2:{userNotificationRowSize2}");

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Run SqlServer Test failed.");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex.ToString());
                Console.ResetColor();
            }
            Console.WriteLine();
        }

        static string MssqlGetSize<T>(DbContext dbContext, 
            string where)
        {
            var type = typeof(T);
            var entityType = dbContext.Model.FindEntityType(type);
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();
            var columnNames = entityType.GetProperties().Select(n => n.GetColumnName()).ToArray();
            var alias = "t";

            var schemaStr = string.IsNullOrEmpty(schema) ? "" : $"[{schema}].";
            var fullTableNameStr = $"{schemaStr}[{tableName}]";
            var selectFrom = $@"
SELECT {string.Join('+', columnNames.Select(n => $"SUM(DATALENGTH({alias}.{n}))"))} AS N'TotalSize'
FROM {fullTableNameStr} AS {alias}
{where}";
            return selectFrom;
        }

        static string NpgsqlGetSize<T>(DbContext dbContext,
            string where)
        {
            var type = typeof(T);
            var entityType = dbContext.Model.FindEntityType(type);
            var schema = entityType.GetSchema();
            var tableName = entityType.GetTableName();
            var columnNames = entityType.GetProperties().Select(n => n.GetColumnName()).ToArray();
            var alias = "t";

            var schemaStr = string.IsNullOrEmpty(schema) ? "" : $@"""{schema}"".";
            var fullTableNameStr = $@"{schemaStr}""{tableName}""";
            var selectFrom = $@"
SELECT {string.Join('+', columnNames.Select(n => $@"SUM(pg_column_size({alias}.""{n}""))"))} AS ""TotalSize""
FROM {fullTableNameStr} AS {alias}
{where}";
            return selectFrom;
        }


        static void InitializeData(TestDbContext dbContext)
        {
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            var user = new User();
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            var fileStore = new FileStore();
            fileStore.UserId = user.Id;
            fileStore._data = new byte[1024];
            dbContext.Add(fileStore);
            dbContext.SaveChanges();

            var userNotification = new UserNotification();
            userNotification.UserId = user.Id;
            userNotification.Message = string.Join("", Enumerable.Repeat("一二", 250));
            dbContext.Add(userNotification);
            dbContext.SaveChanges();

        }
    }

}