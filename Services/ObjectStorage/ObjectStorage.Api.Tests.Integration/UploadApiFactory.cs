using Azure;
using Azure.Data.Tables;
using Azure.Storage;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using ObjectStorage.Api.Context;
using Testcontainers.Azurite;

namespace ObjectStorage.Api.Tests.Integration;

public class UploadApiFactory:WebApplicationFactory<IApiMarker>,IAsyncLifetime
{
   
    private BlobServiceClient _blobServiceClient;
    
    private AzuriteContainer _azuriteContainer = new AzuriteBuilder()
        .WithImage(" mcr.microsoft.com/azure-storage/azurite:latest")
        .WithName("azurite-testcontainer")
        .WithPortBinding(10000, 10000)  // Default Azurite port for Blob Service
        .WithCleanUp(true)              // Clean up the container after tests
        .Build();
    
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureLogging(logging =>
        {
            logging.ClearProviders();
        });
        
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll(typeof(IHostedService));
            
            services.RemoveAll(typeof(IBlobClientFactory));
            
            
            services.AddScoped<IBlobClientFactory>(provider =>
            {
                // Create BlobServiceClient and TableServiceClient using the "test" connection string
                var blobServiceClient = new BlobServiceClient(_azuriteContainer.GetConnectionString());
                var tableServiceClient = new TableServiceClient(_azuriteContainer.GetConnectionString());
        
                // Return new BlobClientFactory with the created clients
                return new BlobClientFactory(blobServiceClient, tableServiceClient);
            });
        });
    }

    public async Task InitializeAsync()
    {
        await _azuriteContainer.StartAsync();
    }

    public async Task DisposeAsync()
    {
        await _azuriteContainer.StopAsync();
        _azuriteContainer.DisposeAsync();
    }
    
    
    
}