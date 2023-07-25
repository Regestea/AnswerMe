using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace AnswerMe.Infrastructure
{
    public static class ConfigureServices
    {
        public static  IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<AnswerMeDbContext>(options =>
                options.UseCosmos(
                    configuration.GetSection("DatabaseSettings:ConnectionString").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:PrimaryKey").Value ?? throw new InvalidOperationException(),
                    configuration.GetSection("DatabaseSettings:DatabaseName").Value ?? throw new InvalidOperationException())
            );
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
