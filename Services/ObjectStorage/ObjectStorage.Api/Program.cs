using Configs.Shared;
using IdentityServer.Shared.Client;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Services;
using ObjectStorage.Api.Services.InterFaces;
using Security.Shared.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddIdentityServerClientServices(options =>
{
    options.IdentityServerGrpcUrl = builder.Configuration.GetSection("IdentityServer:GrpcUrl").Value ?? throw new NullReferenceException();
    options.RedisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value ?? throw new NullReferenceException();
    options.RedisInstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value ?? throw new NullReferenceException();
    options.IssuerUrl = builder.Configuration.GetSection("Issuer:Url").Value ?? throw new NullReferenceException();
});
builder.Services.AddSwagger(options =>
    {
        options.Title = "Object Storage";
        options.Version = "v1";
    }
);
builder.Services.AddScoped<IBlobClientFactory, BlobClientFactory>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();
//builder.Services.AddHostedService<BlobBackgroundService>();

var app = builder.Build();

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
