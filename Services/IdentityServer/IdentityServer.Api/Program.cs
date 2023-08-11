using System.Reflection;
using System.Security.Claims;
using System.Text;
using Configs.Shared;
using IdentityServer.Api.Context;
using IdentityServer.Api.GrpcServices;
using IdentityServer.Api.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Security.Shared;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddGrpc();
builder.Services.AddControllers();

#region DatabaseConfig

builder.Services.AddDbContext<IdentityServerDbContext>(options =>
    options.UseCosmos(
        builder.Configuration.GetSection("DatabaseSettings:ConnectionString").Value ?? throw new InvalidOperationException(),
        builder.Configuration.GetSection("DatabaseSettings:PrimaryKey").Value ?? throw new InvalidOperationException(),
        builder.Configuration.GetSection("DatabaseSettings:DatabaseName").Value ?? throw new InvalidOperationException())
);
#endregion

builder.Services.AddSwagger(options =>
    {
        options.Title = "Identity Server";
        options.Version = "v1";
    }
);
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

#region JwtBearer

//JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "https://localhost:7126",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                builder.Configuration.GetValue<string>("JWT:SecretKey") ?? throw new InvalidOperationException()))
        };
    });

#endregion

builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

#region Authorization

builder.Services.AddAuthorization();

#endregion

var app = builder.Build();
app.UseCors("AllowAllOrigins");

app.MapGrpcService<AuthorizationGrpcService>();
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