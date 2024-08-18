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
builder.AddInfrastructureServices(builder.Configuration);


var identityServerUrl = builder.Configuration.GetConnectionString("IdentityServerApi");
var redisConnectionName = builder.Configuration.GetConnectionString("RedisCache");

if (identityServerUrl == null )
{
    throw new InvalidOperationException("Required connection identity server url strings are missing.");
}

if (redisConnectionName == null)
{
    throw new InvalidOperationException("Required connection redisConnectionName strings are missing.");
}

builder.AddIdentityServerClientServices(options =>
{
    options.IdentityServerUrl = "https://" + identityServerUrl;
    options.RedisConnectionName = redisConnectionName;
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

app.MigrateDatabaseServices();

app.UseAuthorization();

app.MapControllers();

app.MapHub<GroupRoomHub>("Group-Chat");
app.MapHub<OnlineHub>("Online-User");
app.MapHub<PrivateRoomHub>("Private-Chat");

app.Run();