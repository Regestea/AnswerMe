using AnswerMe.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Domain.Entities
{
    public class Message : BaseEntity
    {
        public string? Text { get; set; }

        public Guid? MediaId { get; set; }

        public Guid FromId { get; set; }

        public Guid ToId { get; set; }


        [ForeignKey("FromId")]
        public Media? Media { get; set; }

        [ForeignKey("FromId")]
        public User From { get; set; } = null!;

        [ForeignKey("ToId")]
        public User To { get; set; } = null!;

    }
}
