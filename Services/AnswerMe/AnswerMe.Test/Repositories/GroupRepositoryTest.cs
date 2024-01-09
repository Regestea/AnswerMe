using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Test.Repositories
{
    public class GroupRepositoryTest
    {
        private readonly DbContextOptions<AnswerMeDbContext> dbContextOptions;

        private readonly AnswerMeDbContext _inMemoryDbContext;

        public GroupRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<AnswerMeDbContext>()
                .UseInMemoryDatabase(databaseName: "AnswerMe")
                .Options;
            _inMemoryDbContext = new AnswerMeDbContext(dbContextOptions);
        }


        [Fact]
        public async Task GroupGetList_Should_GetListOfUserGroup()
        {
            //Arrange
            var mockGroupRepository = new GroupRepository(_inMemoryDbContext, null!);

            var userId = Guid.NewGuid();

            var groupChats = new List<GroupChat>()
            {
                new()
                {
                    id = Guid.NewGuid(),
                    Name = "Group Name 1 "
                },
                new()
                {
                    id = Guid.NewGuid(),
                    Name = "Group Name 2 "
                },
                new()
                {
                    id = Guid.NewGuid(),
                    Name = "Group Name 3 "
                },
            };

            var userGroups = new List<UserGroup>();

            foreach (var group in groupChats)
            {
                userGroups.Add(new UserGroup()
                {
                    id = Guid.NewGuid(),
                    GroupId = group.id,
                    UserId = userId
                });
            }

            await _inMemoryDbContext.GroupChats.AddRangeAsync(groupChats);

            await _inMemoryDbContext.GroupChats.AddAsync(new GroupChat() { id = Guid.NewGuid(), Name = "other one" });

            await _inMemoryDbContext.UserGroups.AddRangeAsync(userGroups);

            await _inMemoryDbContext.SaveChangesAsync();

            var paginationRequest = new PaginationRequest()
            {
                CurrentPage = 1,
                PageSize = 5
            };


            //Act

            var pagedGroupList = await mockGroupRepository.GetListAsync(userId, paginationRequest);

            //Assert

            Assert.NotNull(pagedGroupList);
            Assert.True(pagedGroupList.IsSuccess);
            Assert.IsType<PagedListResponse<GroupResponse>>(pagedGroupList.AsSuccess.Value);
            Assert.NotNull(pagedGroupList.AsSuccess.Value.Items);
            Assert.True(pagedGroupList.AsSuccess.Value.Items.Any());
            Assert.Equal(3, pagedGroupList.AsSuccess.Value.Items.Count);

            Dispose();
        }


        [Fact]
        public async Task GroupGetUserList_Should_GetListOfUserInGroup()
        {
            //Arrange
            var mockGroupRepository = new GroupRepository(_inMemoryDbContext, null!);

            var groupChat = new GroupChat()
            {
                id = Guid.NewGuid(),
                Name = "Group Name"
            };

            var userList = new List<User>()
            {
                new (){id = Guid.NewGuid(),FullName = "user full name 1",IdName = "user id name 1",PhoneNumber = "99999999999"},
                new (){id = Guid.NewGuid(),FullName = "user full name 2",IdName = "user id name 2",PhoneNumber = "99999999999"},
                new (){id = Guid.NewGuid(),FullName = "user full name 3",IdName = "user id name 3",PhoneNumber = "99999999999"},
                new (){id = Guid.NewGuid(),FullName = "user full name 4",IdName = "user id name 4",PhoneNumber = "99999999999"},
            };

            var userGroups = new List<UserGroup>();

            foreach (var user in userList)
            {
                userGroups.Add(new UserGroup()
                {
                    id = Guid.NewGuid(),
                    GroupId = groupChat.id,
                    UserId = user.id
                });
            }

            var groupAdmin = new GroupAdmin()
            {
                UserId = userList.First().id, 
                RoomId = groupChat.id
            };

            var paginationRequest = new PaginationRequest()
            {
                CurrentPage = 1,
                PageSize = 5
            };

            await _inMemoryDbContext.GroupChats.AddAsync(groupChat);
            await _inMemoryDbContext.GroupAdmins.AddAsync(groupAdmin);
            await _inMemoryDbContext.Users.AddRangeAsync(userList);
            await _inMemoryDbContext.UserGroups.AddRangeAsync(userGroups);

            await _inMemoryDbContext.SaveChangesAsync();

            //Act

            var pagedUser= await mockGroupRepository.UserListAsync(userList.Last().id, groupChat.id, paginationRequest);

            //Assert
            Assert.NotNull(pagedUser);
            Assert.True(pagedUser.IsSuccess);
            Assert.IsType<PagedListResponse<PreviewGroupUserResponse>>(pagedUser.AsT0.Value);
            Assert.NotNull(pagedUser.AsSuccess.Value.Items);
            Assert.True(pagedUser.AsSuccess.Value.Items.Any());
            Assert.Equal(4, pagedUser.AsSuccess.Value.Items.Count);

            Dispose();
        }

        private void Dispose()
        {
            _inMemoryDbContext.Dispose(); // Dispose of the in-memory database context
        }
    }
}
