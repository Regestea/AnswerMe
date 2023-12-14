using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Shared
{
    public class BooleanResponse
    {
        public required string FieldName { get; set; }
        public bool Result { get; set; }
    }
}
