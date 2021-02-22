using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

namespace Recipe.Api.Models.Queries
{
    internal static class QueryExtensions
    {
        public static IIncludableQueryable<TEntity, TProperty> IncludeExpression<TEntity, TProperty>(
            this IQueryable<TEntity> source,
            Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, TProperty>> expression
        )
            where TEntity : class
        {
            return expression(source);
        }
        
        public static IIncludableQueryable<TEntity, TProperty> IncludePrefixed<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this IQueryable<TEntity> source,
            Expression<Func<TEntity, TPreviousProperty>> prefixPath,
            [NotNull] Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            return source.Include(prefixPath).ThenInclude(navigationPropertyPath);
        }
        
        public static IIncludableQueryable<TEntity, TProperty> IncludePrefixed<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this IQueryable<TEntity> source,
            Expression<Func<TEntity, IEnumerable<TPreviousProperty>>> prefixPath,
            [NotNull] Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath)
            where TEntity : class
        {
            return source.Include(prefixPath).ThenInclude(navigationPropertyPath);
        }
        
        public static IIncludableQueryable<TEntity, TProperty> IncludeMany<TEntity, TProperty>(
            [NotNull] this IQueryable<TEntity> source,
            Expression<Func<TEntity, IEnumerable<TProperty>>> navigationPropertyPath)
            where TEntity : class
        {
            return source
                .Include(navigationPropertyPath)
                .ThenInclude<TEntity, TProperty, TProperty>(r => r);
        }

        public static IQueryable<TEntity> OptionalInclude<TEntity, TProperty>(
            [NotNull] this IQueryable<TEntity> source,
            [NotNull] Expression<Func<TEntity, TProperty>> navigationPropertyPath,
            bool performInclude)
            where TEntity : class
        {
            return !performInclude ? source : source.Include(navigationPropertyPath);
        }

        public static IQueryable<TEntity> OptionalThenInclude<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this IIncludableQueryable<TEntity, TPreviousProperty> source,
            [NotNull] Expression<Func<TPreviousProperty, TProperty>> navigationPropertyPath,
            bool performInclude)
            where TEntity : class
        {
            return !performInclude ? (IQueryable<TEntity>) source : source.ThenInclude(navigationPropertyPath);
        }

        public static IIncludableQueryable<TEntity, TProperty> ThenIncludeMany<TEntity, TPreviousProperty, TProperty>(
            [NotNull] this IIncludableQueryable<TEntity, TPreviousProperty> source,
            Expression<Func<TPreviousProperty, IEnumerable<TProperty>>> navigationPropertyPath)
            where TEntity : class
        {
            return source
                .ThenInclude(navigationPropertyPath)
                .ThenInclude<TEntity, TProperty, TProperty>(r => r);
        }

        public static IQueryable<TEntity> AsNoTracking<TEntity>(
            [NotNull] this IQueryable<TEntity> source,
            bool noTracking)
            where TEntity : class
        {
            return noTracking ? source.AsNoTracking() : source;
        }
        
        
        public static async Task RemoveAsync<TSource>(
            [NotNull] this DbSet<TSource> dbSet,
            Expression<Func<TSource, bool>> where,
            CancellationToken cancellationToken = default)
        where TSource : class
        {
            var list = await  dbSet.AsQueryable().Where(where).ToListAsync(cancellationToken);
            dbSet.RemoveRange(list);
        }
        
        public static async Task RemoveAsync<TSource>(
            [NotNull] this DbSet<TSource> dbSet,
            Func<IQueryable<TSource>, IQueryable<TSource>> query,
            CancellationToken cancellationToken = default)
            where TSource : class
        {
            var list = await  query(dbSet.AsQueryable()).ToListAsync(cancellationToken);
            dbSet.RemoveRange(list);
        }
    }
}