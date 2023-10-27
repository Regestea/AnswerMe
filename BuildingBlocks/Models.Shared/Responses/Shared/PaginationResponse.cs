using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Shared
{
    public class PaginationResponse
    {
        public PaginationResponse(int page, int totalCount, int totalPages)
        {
            Page = page;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }

        public int Page { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
