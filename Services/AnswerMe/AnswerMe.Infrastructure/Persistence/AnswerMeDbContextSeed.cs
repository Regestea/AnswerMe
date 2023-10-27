using AnswerMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnswerMe.Infrastructure.Persistence
{
    public static class AnswerMeDbContextSeed
    {
        public static void SeedData(this ModelBuilder modelBuilder)
        {
            var user = new User()
            {
                id = Guid.Parse("86bfab7e-aea9-405c-bdfa-939539f4b8e0"),
                PhoneNumber = "1999999999999",
                FullName = "test",
                IdName = "test101",
              
                
            };

          
           
            var media = new Media()
            {
                BlurHash = "LG9aU+r#Dzrp#MIQOv%7DgK7Vt$~",
                Type = MediaType.image,
                Path = "https://wallpapers.microsoft.design/images/feature-16.jpg"
            };
           

            //modelBuilder.Entity<Media>().HasData(media);
            //modelBuilder.Entity<Message>().HasData(message);
            //modelBuilder.Entity<RoomChat>().HasData(room);
            //modelBuilder.Entity<User>().HasData(user);
        }
    }
}
