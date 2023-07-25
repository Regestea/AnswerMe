using AnswerMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AnswerMe.Infrastructure.Persistence
{
    public sealed class AnswerMeDbContext : DbContext
    {
      
        public AnswerMeDbContext(DbContextOptions<AnswerMeDbContext> options) :
            base(options)
        {
            Database.EnsureCreated();
        }


        public DbSet<Media> Medias { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRoom> UserRooms { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Media>().ToContainer("Medias");
            modelBuilder.Entity<Message>().ToContainer("Messages");
            modelBuilder.Entity<Room>().ToContainer("Rooms");
            modelBuilder.Entity<User>().ToContainer("Users");
            modelBuilder.Entity<UserRoom>().ToContainer("UserRooms");
            
            modelBuilder.SeedData();
        }
    }
}
