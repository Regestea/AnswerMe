using Azure.Core;
using Grpc.Core;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using Models.Shared.Requests.ObjectStorage;
using Models.Shared.Responses.Shared;
using ObjectStorage.Api.Test.DataConvertor;
using ObjectStorage.Api.Test.DataGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace ObjectStorage.Api.Test.Controllers
{
    public class ObjectStorageControllerTest 
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly HttpClient _httpClient;

        public ObjectStorageControllerTest(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
        }

        [Fact]
        public async Task Should_Upload_ProfileImage_ByChunk()
        {
            var fileStream = TextToImageStream.ConvertTextToImageStream("test image");
            var chunks = await fileStream.ConvertStreamToChunksAsync(10);

            TokenResponse? token = null;

            for (int i = 0; i < chunks.Count; i++)
            {
                if (token == null)
                {
                    var request = new ImageUploadRequest
                    {
                        FileSizeMB = ConvertBytesToMegabytes(fileStream.Length),
                        FileFormat = "jpg"
                    };
                    var jsonContent = JsonSerializer.Serialize(request);
                    var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                    var tokenResponse = await _httpClient.PostAsync("/api/ObjectStorage/Profile", content);
                    var responseJsonContent = await tokenResponse.Content.ReadAsStringAsync();
                     token = JsonSerializer.Deserialize<TokenResponse>(responseJsonContent, new JsonSerializerOptions{
                        DictionaryKeyPolicy = JsonNamingPolicy.CamelCase,
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true,
                    });

                    _testOutputHelper.WriteLine(token.Token);

                    Assert.NotNull(token);
                }
            }
        }
        public double ConvertBytesToMegabytes(long bytes)
        {
            return (double)bytes / (1024 * 1024);
        }

        [Fact]
        public async Task Should_Return_OK()
        {
            var response = await _httpClient.GetAsync("/api/ObjectStorage");
            var stringResult = await response.Content.ReadAsStringAsync();

            Assert.Equal("hello world", stringResult);
        }

        [Fact]
        public async Task Should_Upload_Stream_And_Return_OK()
        {
            _httpClient.Timeout = TimeSpan.FromMinutes(2);

            Stream fileStream = TextToImageStream.ConvertTextToImageStream("test");

            var streamContent = new StreamContent(fileStream);

            streamContent.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
   
            var formData = new MultipartFormDataContent();

            string fileName = "something.png";

            formData.Add(streamContent, "file", "file");
            formData.Add(new StringContent(fileName), "fileName");

            var apiUrl = "/api/ObjectStorage/UploadStream"; // Replace with your API endpoint
            var response = await _httpClient.PostAsync(apiUrl, formData);


            if (response.IsSuccessStatusCode)
            {
                
            }

            

        }
    }
}
