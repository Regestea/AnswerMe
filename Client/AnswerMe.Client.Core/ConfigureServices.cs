using AnswerMe.Client.Core.Auth;
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
        public static IServiceCollection AddCoreServices(this IServiceCollection services)
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

            services.AddHttpClient(nameof(HttpClients.AnswerMe),
                client => { client.BaseAddress = new Uri("https://localhost:7156/api/"); });

            services.AddHttpClient(nameof(HttpClients.IdentityServer),
                client => { client.BaseAddress = new Uri("https://localhost:7216/api/"); });

            services.AddHttpClient(nameof(HttpClients.ObjectStorage),
                client => { client.BaseAddress = new Uri("https://localhost:7205/api/"); });

            return services;
        }
    }
}