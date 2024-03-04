using AnswerMe.Client.Core.Auth;
using AnswerMe.Client.Core.DTOs.Base;
using AnswerMe.Client.Core.Enums;
using AnswerMe.Client.Core.Services;
using AnswerMe.Client.Core.Services.Interfaces;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;

namespace AnswerMe.Client.Core
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddCoreServices(this IServiceCollection services,AppSettings appSettings)
        {
            services.AddAuthorizationCore();
            services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();
            services.AddScoped<IPrivateMessageService, PrivateMessageService>();
            services.AddScoped<IObjectStorageService, ObjectStorageService>();
            services.AddScoped<IGroupMessageService, GroupMessageService>();
            services.AddScoped<IPrivateRoomService, PrivateRoomService>();
            services.AddScoped<IGroupInviteService, GroupInviteService>();
            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IObjectHelperService, ObjectHelperService>();
            services.AddScoped<AuthStateProvider>();
            services.AddBlazoredLocalStorage();
            services.AddCascadingAuthenticationState();
            services.AddSingleton<OnlineHubService>();
            services.AddSingleton<PvHubService>();
            services.AddSingleton<GrHubService>();


            services.AddHttpClient(nameof(HttpClients.AnswerMe),
                client => { client.BaseAddress = new Uri(appSettings.AnswerMe ?? throw new InvalidOperationException()); });

            services.AddHttpClient(nameof(HttpClients.IdentityServer),
                client => { client.BaseAddress = new Uri(appSettings.IdentityServer ?? throw new InvalidOperationException()); });

            services.AddHttpClient(nameof(HttpClients.ObjectStorage),
                client => { client.BaseAddress = new Uri(appSettings.ObjectStorage ?? throw new InvalidOperationException()); });
            
            return services;
        }
    }
}