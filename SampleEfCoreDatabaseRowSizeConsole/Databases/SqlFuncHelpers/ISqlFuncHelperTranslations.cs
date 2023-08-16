using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;

public interface ISqlFuncHelperTranslations
{
    Func<IReadOnlyList<SqlExpression>, SqlExpression> TranslationColumnDataSize();
}