using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Shared
{
    public class PaginationResponse
    {
        public PaginationResponse(int currentPage, int totalCount, int totalPages)
        {
            CurrentPage = currentPage;
            TotalCount = totalCount;
            TotalPages = totalPages;
        }

        public int CurrentPage { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }
}
