using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;

internal class SqlServerFuncHelperTranslations : ISqlFuncHelperTranslations
{
    public Func<IReadOnlyList<SqlExpression>, SqlExpression> TranslationColumnDataSize()
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
                columnExpression = (firstArg as SqlFunctionExpression)?.Arguments[1] as ColumnExpression;
            if (columnExpression == null)
                columnExpression = (firstArg as SqlBinaryExpression)?.Left as ColumnExpression;
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
}