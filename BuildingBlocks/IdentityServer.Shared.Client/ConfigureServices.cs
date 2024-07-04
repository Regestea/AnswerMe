using IdentityServer.Api.Protos;
using IdentityServer.Shared.Client.GrpcServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer.Shared.Client.Repositories;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using IdentityServer.Shared.Client.Service;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using Microsoft.Extensions.Options;


namespace IdentityServer.Shared.Client
{
    public static class ConfigureServices
    {
        public static WebApplicationBuilder AddIdentityServerClientServices(this WebApplicationBuilder builder,
            Action<ConfigureOptions> options)
        {
            var configureOptions = new ConfigureOptions();
            options.Invoke(configureOptions);
            
            builder.Services.AddGrpcClient<AuthorizationService.AuthorizationServiceClient>(o =>
                o.Address = new Uri(builder.Configuration.GetSection("services:IdentityServerApi:https:0").Value ?? throw new InvalidOperationException()));
            
            
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddScoped<AuthorizationGrpcServices>();

            builder.Services.AddScoped<IJwtCacheRepository, JwtCacheRepository>();

            builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();

            builder.Services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();
            
            builder.AddRedisClient("RedisCache");

            #region JwtBarer

            builder.Services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = builder.Configuration.GetSection("services:IdentityServerApi:https:0").Value,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });

            #endregion

            return builder;
        }
    }
}