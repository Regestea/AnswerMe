using System.Net;

namespace AnswerMe.Client.Core.DTOs.Response
{
    public class HttpResponseDto
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Content { get; set; } = null!;
    }
}