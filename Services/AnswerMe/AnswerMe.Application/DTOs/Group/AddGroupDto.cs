using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Entities;

namespace AnswerMe.Application.DTOs.Group
{
    public class AddGroupDto
    {
        public Guid UserId { get; set; }
        public string Name { get; set; } = null!;
        public string? ImageToken { get; set; }
    }
}
