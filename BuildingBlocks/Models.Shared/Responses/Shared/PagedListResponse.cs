using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Models.Shared.Responses.Shared
{
    public class PagedListResponse<T>:PaginationResponse
    {
        private PagedListResponse(int page, int totalCount, int totalPages, List<T> items) : base(page, totalCount, totalPages)
        {
            Items = items;
        }

        

        public List<T> Items { get; set; }

        public static async Task<PagedListResponse<T>> CreateAsync(IQueryable<T> query, int pageSize, int page)
        {
            var totalCount =await query.CountAsync();
            var items=await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            var totalPages = (int)Math.Ceiling((decimal)totalCount / pageSize);

            return new PagedListResponse<T>(page, totalCount, totalPages, items);
        }
    }
}
