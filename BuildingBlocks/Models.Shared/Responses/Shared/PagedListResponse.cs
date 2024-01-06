using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;

namespace Models.Shared.Responses.Shared
{    public class PagedListResponse<T> : PaginationResponse
{
    // Parameterless constructor for deserialization
    public PagedListResponse() : base(0, 0, 0)
    {
        Items = new List<T>();
    }

    // Parameterized constructor for construction
    [JsonConstructor]
    private PagedListResponse(int page, int totalCount, int totalPages, List<T> items) : base(page, totalCount, totalPages)
    {
        Items = items;
    }

    public List<T> Items { get; set; }

    public static async Task<PagedListResponse<T>> CreateAsync(IQueryable<T> query, PaginationRequest request)
    {
        var totalCount = await query.CountAsync();
        var items = await query.Skip((request.CurrentPage - 1) * request.PageSize).Take(request.PageSize).ToListAsync();
        var totalPages = (int)Math.Ceiling((decimal)totalCount / request.PageSize);

        return new PagedListResponse<T>(request.CurrentPage, totalCount, totalPages, items);
    }
}


}
