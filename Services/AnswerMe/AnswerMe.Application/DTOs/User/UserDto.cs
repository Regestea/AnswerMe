using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.DTOs.User
{
    public class UserDto: BaseEntityDto
    {
        public string? FullName { get; set; }
        public string? IdName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? ProfileImage { get; set; }
    }
}
