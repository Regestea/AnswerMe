﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Common;

namespace AnswerMe.Domain.Entities
{
    public class GroupChat:BaseEntity
    {
        public string Name { get; set; } = null!;

        public string? RoomImage { get; set; }
    }
}
