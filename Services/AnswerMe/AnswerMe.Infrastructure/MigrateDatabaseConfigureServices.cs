using AnswerMe.Infrastructure.Common.Extensions;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.Extensions.Hosting;

namespace AnswerMe.Infrastructure
{
    public static class MigrateDatabaseConfigureServices
    {
        public static IHost MigrateDatabaseServices(this IHost host)
        {
            host.MigrateDatabase<AnswerMeDbContext>();

            return host;
        }
    }
}
