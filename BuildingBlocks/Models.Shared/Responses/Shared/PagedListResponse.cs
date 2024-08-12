using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;

namespace Models.Shared.Responses.Shared
{
    public class PagedListResponse<T>
    {
        [JsonConstructor]
        private PagedListResponse(List<T> items, PaginationResponse pagination)
        {
            Items = items;
            Pagination = pagination;
        }
        
        public List<T> Items { get; set; }
        public PaginationResponse Pagination { get; set; }

        public static async Task<PagedListResponse<T>> CreateAsync(IQueryable<T> query, PaginationRequest request)
        {
            var totalCount = await query.CountAsync();
            var items = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize)
                .ToListAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / request.PageSize);
            var pagination = new PaginationResponse(request.CurrentPage, totalCount, totalPages);


            return new PagedListResponse<T>(items, pagination);
        }
    }
}