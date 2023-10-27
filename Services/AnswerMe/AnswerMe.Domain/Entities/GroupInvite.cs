using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class GroupInvite : BaseEntity
    {
        [Required]
        public string Token { get; set; } = null!;

        [Required]
        [Range(0, int.MaxValue)]
        public int UserCount { get; set; }

        [Required]
        public Guid GroupId { get; set; }

        [Required]
        public DateTimeOffset ExpirationDate { get; set; }
    }
}
