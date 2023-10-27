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


        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<RoomLastSeen> RoomLastSeen { get; set; }
        public DbSet<GroupAdmin> GroupAdmins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<OnlineStatusUser> OnlineStatusUsers { get; set; }
        public DbSet<GroupInvite> GroupInvitations { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserGroup>().ToContainer("GroupUsers");
            modelBuilder.Entity<Message>().ToContainer("Messages");
            modelBuilder.Entity<Message>().HasMany(x => x.MediaList);
            modelBuilder.Entity<PrivateChat>().ToContainer("PrivateRoomChats");
            modelBuilder.Entity<GroupChat>().ToContainer("GroupRoomChats");
            modelBuilder.Entity<RoomLastSeen>().ToContainer("RoomLastSeen");
            modelBuilder.Entity<GroupAdmin>().ToContainer("GroupRoomAdmins");
            modelBuilder.Entity<OnlineStatusUser>().ToContainer("OnlineStatusUsers");
            modelBuilder.Entity<User>().ToContainer("Users");
            modelBuilder.Entity<GroupInvite>().ToContainer("GroupInvitations");
            modelBuilder.SeedData();
        }
    }
}
