using AnswerMe.Client.Core.DTOs.Response;

namespace AnswerMe.Client.Core.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<HttpResponseDto> SendRequestAsync(this HttpClient httpClient, string url,
            HttpMethod method, StringContent? jsonBody = null)
        {
            using var request = new HttpRequestMessage(method, url);

            if (jsonBody != null)
            {
                request.Content = jsonBody;
            }

            using var response = await httpClient.SendAsync(request);

            var responseDto = new HttpResponseDto()
            {
                StatusCode = response.StatusCode,
                Content = await response.Content.ReadAsStringAsync()
            };
            return responseDto;
        }
    }
}