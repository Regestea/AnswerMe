using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Shared
{
    public class IdResponse
    {
        public required string FieldName { get; set; }
        public required Guid Id { get; set; }
    }
}
