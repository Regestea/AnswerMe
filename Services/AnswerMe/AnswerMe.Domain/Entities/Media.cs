using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class Media
    {
        public Guid Id { get; set; }

        public Guid MessageId { get; set; }

        public string? BlurHash { get; set; }
        

        [Required]
        public MediaType Type { get; set; }

        [Required]
        public string Path { get; set; } = null!;
    }
}
