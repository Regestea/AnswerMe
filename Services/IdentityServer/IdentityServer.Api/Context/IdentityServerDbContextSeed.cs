using IdentityServer.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace IdentityServer.Api.Context
{
    public static class IdentityServerDbContextSeed
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            var user = new User()
            {
                id = Guid.Parse("86bfab7e-aea9-405c-bdfa-939539f4b8e0"),
                PhoneNumber = "1999999999999",
                
                IdName = "test101",
                Password = "s9RkY5HqZqDfKz7tW2fJ7H9YJL7xA0yVl2w2Ukm3vPd9c7QKsK7dXl6q4Xjj2sLc5VjZ9c8oLj7x9Mq0sHzXQ==",
            };

            modelBuilder.Entity<User>().HasData(user);
        }
    }
}
