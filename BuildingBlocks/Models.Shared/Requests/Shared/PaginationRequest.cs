using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.Shared
{
    public class PaginationRequest
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
}
