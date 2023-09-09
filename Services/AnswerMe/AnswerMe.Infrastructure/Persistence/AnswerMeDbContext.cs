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


        //public DbSet<Media> Medias { get; set; }
        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<LastSeen> LastSeen { get; set; }
        public DbSet<GroupAdmin> GroupAdmins { get; set; }
        public DbSet<User> Users { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<Media>().ToContainer("Medias");

            modelBuilder.Entity<UserGroup>().HasNoKey().ToContainer("GroupUsers");
            modelBuilder.Entity<Message>().ToContainer("Messages");
            modelBuilder.Entity<PrivateChat>().ToContainer("PrivateRoomChats");
            modelBuilder.Entity<GroupChat>().ToContainer("GroupRoomChats");
            modelBuilder.Entity<LastSeen>().HasNoKey().ToContainer("RoomsLastSeen");
            modelBuilder.Entity<GroupAdmin>().HasNoKey().ToContainer("GroupRoomAdmins");
            modelBuilder.Entity<User>().ToContainer("Users");
            modelBuilder.SeedData();
        }
    }
}
