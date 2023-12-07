using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Azure.Cosmos.Linq;
using Models.Shared.Requests.Shared;


namespace AnswerMe.Application.Extensions
{
    /// <summary>
    /// Extensions methods for Entity Framework.
    /// </summary>
    public static class EntityFrameworkExtensions
    {
        /// Checks if any entity in the given IQueryable sequence satisfies the specified predicate in an asynchronous manner.
        /// @param source The IQueryable sequence of entities to check.
        /// @param predicate The predicate to test each element against.
        /// @returns A Task representing the asynchronous operation, containing a boolean value indicating if any element satisfies
        /// the predicate.
        /// @throws ArgumentNullException if the source or predicate is null.
        /// /
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

        /// <summary>
        /// Provides pagination functionality for an <see cref
        /// ="IQueryable{T}"/> object.
        /// </summary>
        /// <typeparam name="BaseEntity">The type of the entities in the <see cref="IQueryable{T}"/>.</typeparam>
        /// <param name="queryable">The <see cref="IQueryable{T}"/> object to paginate.</param>
        /// <param name="request">The pagination request.</param>
        /// <returns>The paginated <see cref="IQueryable{T}"/> object.</returns>
        public static IQueryable<BaseEntity> Paginate<BaseEntity>(this IQueryable<BaseEntity> queryable, PaginationRequest request)
        {
            return queryable.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize);
        }
    }
}
