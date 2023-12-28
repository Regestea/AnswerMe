using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.Extensions;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using AnswerMe.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ObjectStorage.Api.Protos;
using AnswerMe.Infrastructure.Configs;


namespace AnswerMe.Infrastructure
{
    /// This class provides methods to add infrastructure services to the IServiceCollection.
    public static class ConfigureServices
    {
        /// <summary>
        /// Adds infrastructure services to the <see cref="
        /// IServiceCollection"/>.
        /// </summary>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration settings.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> with added services.</returns>
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnswerMeDbContext>(options =>
                options.UseCosmos(
                    configuration.GetSection("DatabaseSettings:ConnectionString").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:PrimaryKey").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:DatabaseName").Value ?? throw new InvalidOperationException())
            );
            services.AddGrpcClient<ObjectStorageService.ObjectStorageServiceClient>(o =>
                o.Address = new Uri(configuration.GetSection("ObjectStorageServer:GrpcUrl").Value ?? throw new InvalidOperationException()));
            services.AddScoped<FileStorageService>();
            services.AddScoped<ICacheRepository, CacheRepository>();
            services.AddScoped<IGroupHubService, GroupHubService>();
            services.AddScoped<IGroupInviteRepository, GroupInviteRepository>();
            services.AddScoped<IGroupMessageService, GroupMessageService>();
            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IOnlineHubService, OnlineHubService>();
            services.AddScoped<IPrivateHubService, PrivateHubService>();
            services.AddScoped<IPrivateMessageService, PrivateMessageService>();
            services.AddScoped<IPrivateRoomRepository, PrivateRoomRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
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

            FileStorageHelper.Initialize(configuration.GetSection("Blob:StorageUrl").Value ?? throw new InvalidOperationException());
            return services;
        }
    }
}
