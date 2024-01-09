using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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
    }
}
