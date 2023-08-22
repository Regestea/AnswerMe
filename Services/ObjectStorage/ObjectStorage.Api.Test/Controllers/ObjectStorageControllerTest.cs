using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using ObjectStorage.Api.Test.DataGenerator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ObjectStorage.Api.Test.Controllers
{
    public class ObjectStorageControllerTest 
    {
        private readonly HttpClient _httpClient;

        public ObjectStorageControllerTest()
        {
            var webAppFactory = new WebApplicationFactory<Program>();
            _httpClient = webAppFactory.CreateDefaultClient();
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
