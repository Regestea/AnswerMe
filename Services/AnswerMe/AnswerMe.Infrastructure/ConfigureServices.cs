using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.Extensions;
using AnswerMe.Infrastructure.Configs;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using AnswerMe.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ObjectStorage.Api.Protos;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using AnswerMe.Infrastructure.Common.Extensions;

namespace AnswerMe.Infrastructure
{
    /// This class provides methods to add infrastructure services to the IServiceCollection.
    public static class ConfigureServices
    {
        /// <summary>
        /// Adds infrastructure services to the <see cref="WebApplicationBuilder"/>.
        /// </summary>
        /// <param name="builder">The <see cref="WebApplicationBuilder"/> builder to configure.</param>
        /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration settings.</param>
        /// <returns>The modified <see cref="IServiceCollection"/> with added services.</returns>
        public static WebApplicationBuilder AddInfrastructureServices(this WebApplicationBuilder builder, IConfiguration configuration)
        {
            builder.AddNpgsqlDbContext<AnswerMeDbContext>("AnswerMeDB");

            builder.Services.AddGrpcClient<ObjectStorageService.ObjectStorageServiceClient>(o =>
                o.Address = new Uri(builder.Configuration.GetSection("services:ObjectStorageApi:https:0").Value ?? throw new InvalidOperationException()));
            builder.Services.AddScoped<FileStorageService>();
            builder.Services.AddScoped<ICacheRepository, CacheRepository>();
            builder.Services.AddScoped<IGroupHubService, GroupHubService>();
            builder.Services.AddScoped<IGroupInviteRepository, GroupInviteRepository>();
            builder.Services.AddScoped<IGroupMessageService, GroupMessageService>();
            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<IOnlineHubService, OnlineHubService>();
            builder.Services.AddScoped<IPrivateHubService, PrivateHubService>();
            builder.Services.AddScoped<IPrivateMessageService, PrivateMessageService>();
            builder.Services.AddScoped<IPrivateRoomRepository, PrivateRoomRepository>();
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddSignalR();
            builder.Services.AddSwagger();
            FileStorageHelper.Initialize(builder.Configuration.GetConnectionString("ObjectStorage").GetBlobEndpoint()+"/");
            return builder;
        }
    }
}
