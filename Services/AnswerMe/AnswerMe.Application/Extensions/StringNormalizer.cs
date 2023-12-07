using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Application.Extensions
{
    /// <summary>
    /// This class provides methods for normalizing strings.
    /// </summary>
    public static class StringNormalizer
    {
        /// <summary>
        /// Normalizes the given string by converting it to lowercase and removing leading and trailing whitespace.
        /// </summary>
        /// <param name="content">The string to be normalized.</param>
        /// <returns>The normalized string.</returns>
        public static string NormalizeString(this string content)
        {
            return content.ToLower().Trim();
        }
    }
}
