using IdentityServer.Shared.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using AnswerMe.Infrastructure;
using AnswerMe.Infrastructure.Configs;
using AnswerMe.Infrastructure.Hubs;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddInfrastructureServices(builder.Configuration);


builder.Services.AddIdentityServerClientServices(options =>
{
    options.IdentityServerGrpcUrl = builder.Configuration.GetSection("IdentityServer:GrpcUrl").Value ?? throw new NullReferenceException();
    options.RedisConnectionString = builder.Configuration.GetSection("Redis:ConnectionString").Value ?? throw new NullReferenceException();
    options.RedisInstanceName = builder.Configuration.GetSection("Redis:InstanceName").Value ?? throw new NullReferenceException();
    options.IssuerUrl = builder.Configuration.GetSection("Issuer:Url").Value ?? throw new NullReferenceException();
});


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

app.MapHub<GroupRoomHub>("Group-Chat");
app.MapHub<OnlineHub>("Online-User");
app.MapHub<PrivateRoomHub>("Private-Chat");

app.Run();
