using IdentityServer.Api.Protos;
using IdentityServer.Shared.Client.GrpcServices;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using IdentityServer.Shared.Client.Repositories;
using IdentityServer.Shared.Client.Repositories.Interfaces;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;

namespace IdentityServer.Shared.Client
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddIdentityServerClientServices(this IServiceCollection services, Action<ConfigureOptions> options)
        {
            var configureOptions = new ConfigureOptions();
            options.Invoke(configureOptions);

            services.AddGrpcClient<AuthorizationService.AuthorizationServiceClient>(o =>
                o.Address = new Uri(configureOptions.IdentityServerGrpcUrl));
            services.AddHttpContextAccessor();
            services.AddScoped<AuthorizationGrpcServices>();

            services.AddScoped<ICacheRepository, CacheRepository>();

            services.AddScoped<IJwtTokenRepository, JwtTokenRepository>();

            #region Redis Cache

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configureOptions.RedisConnectionString;
                options.InstanceName = configureOptions.RedisInstanceName;

            });

            #endregion

            #region Redis Multiplexer

            services.AddSingleton<IConnectionMultiplexer>(provider => ConnectionMultiplexer.Connect("localhost:9191,password=123456"));

            #endregion

            #region JwtBarer

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidIssuer = configureOptions.IssuerUrl,
                        ValidateAudience = false,
                        ValidateLifetime = true
                    };
                });

            #endregion

            return services;
        }
    }
}
