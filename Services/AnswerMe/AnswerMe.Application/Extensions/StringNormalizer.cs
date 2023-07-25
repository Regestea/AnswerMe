using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.Extensions
{
    public static class StringNormalizer
    {
        public static string NormalizeString(this string content)
        {
            return content.ToLower().Trim();
        }
    }
}
