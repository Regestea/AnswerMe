using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Requests.Shared
{
    public class PaginationRequest
    {
        [Range(1,int.MaxValue)]
        public int CurrentPage { get; set; }

        [Range(1,30)]
        public int PageSize { get; set; }
    }
}
