using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using ObjectStorage.Api.Context;
using ObjectStorage.Api.Services;
using ObjectStorage.Api.Services.InterFaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IBlobClientFactory, BlobClientFactory>();
builder.Services.AddScoped<IFileUploadService, FileUploadService>();


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
