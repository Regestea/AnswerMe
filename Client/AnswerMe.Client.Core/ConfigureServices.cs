using AnswerMe.Client.Core.Auth;
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
            services.AddScoped<AuthStateProvider>();
            services.AddBlazoredLocalStorage();
            services.AddCascadingAuthenticationState();
          
            return services;
        }
    }
}