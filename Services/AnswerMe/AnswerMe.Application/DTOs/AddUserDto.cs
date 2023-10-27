using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.DTOs
{
    public class AddUserDto
    {
        public Guid id { get; set; }
        public string IdName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
    }
}
