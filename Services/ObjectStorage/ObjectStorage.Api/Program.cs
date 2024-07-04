using IdentityServer.Shared.Client;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Api.Configs;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.GrpcServices;
using ObjectStorage.Api.Services;
using ObjectStorage.Api.Services.InterFaces;
using Security.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.AddIdentityServerClientServices(options =>
{
    options.IdentityServerUrl ="https://"+builder.Configuration.GetConnectionString("IdentityServerApi") ?? throw new InvalidOperationException();
    options.RedisConnectionName = builder.Configuration.GetConnectionString("RedisCache") ?? throw new InvalidOperationException();
});
builder.Services.AddSwagger();
builder.Services.AddScoped<IBlobClientFactory, BlobClientFactory>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
builder.Services.AddScoped<IBlurHashService, BlurHashService>();
builder.Services.AddHostedService<BlobBackgroundService>();
builder.Services.AddGrpc();
builder.AddAzureBlobClient("ObjectStorage");
builder.AddAzureTableClient("ObjectIndexTable");

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();
app.MapGrpcService<ObjectStorageGrpcService>();
app.UseCors("AllowAllOrigins");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();