using Microsoft.EntityFrameworkCore;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;

public class SqlFuncHelperBuilder
{
    private ISqlFuncHelperTranslations sqlFuncHelperTranslations = null!;

    public void SetModelBuilder(ModelBuilder modelBuilder, string providerName)
    {
        SetSqlFuncHelperTranslations(providerName);

        foreach (var method in typeof(SqlFuncHelper).GetMethods().Where(n => n.Name == nameof(SqlFuncHelper.ColumnDataSize)))
        {
            modelBuilder.HasDbFunction(method)
               .HasTranslation(sqlFuncHelperTranslations.TranslationColumnDataSize());
        }

    }

    private void SetSqlFuncHelperTranslations(string providerName)
    {
        if (providerName == EFCoreHelperProviderNames.SqlServer.ProviderName)
        {
            sqlFuncHelperTranslations = new SqlServerFuncHelperTranslations();
        }
        else if (providerName == EFCoreHelperProviderNames.PostgreSQL.ProviderName)
        {
            sqlFuncHelperTranslations = new NpgSqlFuncHelperTranslations();
        }
        else
        {
            throw new Exception($"{nameof(sqlFuncHelperTranslations)} unset. providerName = {providerName}.");
        }
    }
}