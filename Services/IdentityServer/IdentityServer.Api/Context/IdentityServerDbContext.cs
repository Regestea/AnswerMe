using IdentityServer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Context
{
    public sealed class IdentityServerDbContext : DbContext
    {
      
        public IdentityServerDbContext(DbContextOptions<IdentityServerDbContext> options) :
            base(options)
        {
        }

        public DbSet<User> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
