using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos.Linq;
using Models.Shared.Requests.Shared;


namespace AnswerMe.Application.Extensions
{
    public static class EntityFrameworkExtensions
    {
        public static async Task<bool> IsAnyAsync<TEntity>(
            this IQueryable<TEntity> source,
            Expression<Func<TEntity, bool>> predicate)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));

            return await source.Where(predicate).CountAsync() >= 1;
        }

        public static IQueryable<BaseEntity> Paginate<BaseEntity>(this IQueryable<BaseEntity> queryable, PaginationRequest request)
        {
            return queryable.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize);
        }
    }
}
