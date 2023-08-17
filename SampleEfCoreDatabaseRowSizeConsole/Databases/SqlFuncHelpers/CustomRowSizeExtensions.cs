using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace SampleEfCoreDatabaseRowSizeConsole.Databases.SqlFuncHelpers;

public static class CustomRowSizeExtensions
{
    public static long GetCustomRowSize<TEntity>(this IQueryable<TEntity> query, DbContext dbContext)
    {
        var lambda = GetTotalColumnDataSizeExpression<TEntity>(dbContext);
        return query.Sum(lambda);
    }

    public static Task<long> GetCustomRowSizeAsync<TEntity>(this IQueryable<TEntity> query, DbContext dbContext)
    {
        var lambda = GetTotalColumnDataSizeExpression<TEntity>(dbContext);
        return query.SumAsync(lambda);
    }

    private static Expression<Func<TEntity, long>> GetTotalColumnDataSizeExpression<TEntity>(DbContext dbContext)
    {
        var parameter = Expression.Parameter(typeof(TEntity), "n");

        var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
        if (entityType == null)
            throw new Exception($"not find Entity:{typeof(TEntity)} in dbContext");
        var properties = entityType.GetProperties();

        var sumExpressions = new List<Expression>();

        foreach (var property in properties)
        {
            if (!property.IsShadowProperty())
            {
                var pgColumnSizeExpression = Expression.Call(
                    typeof(SqlFuncHelper),
                    nameof(SqlFuncHelper.ColumnDataSize),
                    null,
                    Expression.Property(parameter, property.Name));

                sumExpressions.Add(pgColumnSizeExpression);
            }
        }

        var totalExpression = sumExpressions.Aggregate(
            (current, expression) => current == null ? expression : Expression.Add(current, expression));

        var lambda = Expression.Lambda<Func<TEntity, long>>(totalExpression, parameter);
        return lambda;
    }

}