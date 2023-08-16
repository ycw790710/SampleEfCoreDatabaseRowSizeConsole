namespace SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;
public class EFCoreHelperProviderNames
{
    public string ProviderName { get; init; }

    // https://learn.microsoft.com/en-us/ef/core/providers/?tabs=dotnet-core-cli
    public static EFCoreHelperProviderNames SqlServer { get; } = new("Microsoft.EntityFrameworkCore.SqlServer");
    public static EFCoreHelperProviderNames PostgreSQL { get; } = new("Npgsql.EntityFrameworkCore.PostgreSQL");
    public static EFCoreHelperProviderNames InMemory { get; } = new("Microsoft.EntityFrameworkCore.InMemory");
    public static EFCoreHelperProviderNames Sqlite { get; } = new("Microsoft.EntityFrameworkCore.Sqlite");

    public EFCoreHelperProviderNames(string providerName)
    {
        ProviderName = providerName
            ?? throw new ArgumentNullException(nameof(providerName));
    }
}