using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.Extensions;
using AnswerMe.Infrastructure.Hubs;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using AnswerMe.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectStorage.Api.Protos;
using System.Reflection;
using AnswerMe.Infrastructure.Configs;


namespace AnswerMe.Infrastructure
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnswerMeDbContext>(options =>
                options.UseCosmos(
                    configuration.GetSection("DatabaseSettings:ConnectionString").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:PrimaryKey").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:DatabaseName").Value ?? throw new InvalidOperationException())
            );
            //services.AddScoped<IUserRepository, UserRepository>();
            services.AddGrpcClient<ObjectStorageService.ObjectStorageServiceClient>(o =>
                o.Address = new Uri(configuration.GetSection("ObjectStorageServer:GrpcUrl").Value ?? throw new InvalidOperationException()));
            services.AddScoped<FileStorageService>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddSignalR();
            #region Redis Cache

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = configuration.GetSection("Redis:ConnectionString").Value ?? throw new InvalidOperationException();
                options.InstanceName = configuration.GetSection("Redis:InstanceName").Value ?? throw new InvalidOperationException();
            });

            #endregion
            services.AddSignalR();
            services.AddSwagger();
            //services.AddSignalR()
            //    .AddAzureSignalR(configuration.GetConnectionString("Azure:SignalRUrl"));

            FileStorageHelper.Initialize(configuration.GetSection("ObjectStorageServer:GrpcUrl").Value ?? throw new InvalidOperationException());
            return services;
        }
    }
}
