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
                IdName = "test213",
                Password = "s9RkY5HqZqDfKz7tW2fJ7H9YJL7xA0yVl2w2Ukm3vPd9c7QKsK7dXl6q4Xjj2sLc5VjZ9c8oLj7x9Mq0sHzXQ==",
            };

            var room = new Room()
            {
                id = Guid.Parse("51889543-477f-43bf-823f-e857e7425c24"),
                Name = "test room",
                Type = RoomType.SharedChat,
            };
            var userRoom = new UserRoom()
            {
                id = Guid.NewGuid(),
                RoomId = room.id,
                UserId = user.id
            };
            var media = new Media()
            {
                id = Guid.Parse("ce38d738-1ea7-4ef0-8d46-ffe55b1d619a"),
                BlurHash = "LG9aU+r#Dzrp#MIQOv%7DgK7Vt$~",
                Type = MediaType.Image,
                Path = "https://wallpapers.microsoft.design/images/feature-16.jpg"
            };
            var message = new Message()
            {
                id = Guid.Parse("de8c39d3-6893-4cca-a849-c9eff63eb233"),
                FromId = Guid.Parse("86bfab7e-aea9-405c-bdfa-939539f4b8e0"),
                ToId = Guid.Parse("86bfab7e-aea9-405c-bdfa-939539f4b8e0"),
                MediaId = Guid.Parse("ce38d738-1ea7-4ef0-8d46-ffe55b1d619a"),
                Text = "some text"
            };

            modelBuilder.Entity<Media>().HasData(media);
            modelBuilder.Entity<Message>().HasData(message);
            modelBuilder.Entity<Room>().HasData(room);
            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<UserRoom>().HasData(userRoom);
        }
    }
}
