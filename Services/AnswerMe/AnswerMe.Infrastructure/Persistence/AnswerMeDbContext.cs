using AnswerMe.Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace AnswerMe.Infrastructure.Persistence
{
    public sealed class AnswerMeDbContext : DbContext
    {
      
        public AnswerMeDbContext(DbContextOptions<AnswerMeDbContext> options) :
            base(options)
        {
            
        }


        public DbSet<UserGroup> UserGroups { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<PrivateChat> PrivateChats { get; set; }
        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<RoomLastSeen> RoomLastSeen { get; set; }
        public DbSet<GroupAdmin> GroupAdmins { get; set; }
        public DbSet<User> Users { get; set; }
        
        public DbSet<Media> Medias { get; set; }
        public DbSet<OnlineStatusUser> OnlineStatusUsers { get; set; }
        public DbSet<GroupInvite> GroupInvitations { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Message>().HasMany(x => x.MediaList).WithOne();
        }
    }
}
