using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Shared.Responses.Media
{
    public class MediaResponse
    {
        public Guid Id { get; set; }

        public string? BlurHash { get; set; }

        public MediaTypeResponse Type { get; set; }

        public string Path { get; set; } = null!;
    }
}
