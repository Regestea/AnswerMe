using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Entities;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Group;
using Models.Shared.Responses.PrivateRoom;
using Models.Shared.Responses.Shared;
using AnswerMe.Application.Extensions;

namespace AnswerMe.Api.Test.Repositories
{
    public class PrivateRoomRepositoryTest
    {
        private readonly DbContextOptions<AnswerMeDbContext> dbContextOptions;

        private AnswerMeDbContext _inMemoryDbContext;

        public PrivateRoomRepositoryTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<AnswerMeDbContext>()
                .UseInMemoryDatabase(databaseName: "AnswerMe")
                .Options;
            _inMemoryDbContext = new AnswerMeDbContext(dbContextOptions);
        }


        [Fact]
        public async Task PrivateRoomList_Should_GetListOfPrivateChat()
        {
            //Arrange
            var mockGroupRepository = new PrivateRoomRepository(_inMemoryDbContext);

            var userId = Guid.NewGuid();

            var userList = new List<User>()
            {
                new (){id = Guid.NewGuid(),FullName = "User 1",IdName = "User Id 1",PhoneNumber = "99999999999"},
                new (){id = Guid.NewGuid(),FullName = "User 2",IdName = "User Id 2",PhoneNumber = "99999999999",ProfileImage = "/something/some"},
                new (){id = Guid.NewGuid(),FullName = "User 3",IdName = "User Id 3",PhoneNumber = "99999999999"},
                new (){id = Guid.NewGuid(),FullName = "User 4",IdName = "User Id 4",PhoneNumber = "99999999999"},
            };

            var privateChat = new List<PrivateChat>();

            foreach (var user in userList)
            {
                privateChat.Add(new PrivateChat()
                {
                    id = Guid.NewGuid(),
                    User1Id = userId,
                    User2Id = user.id
                });
            }

            var paginationRequest = new PaginationRequest()
            {
                CurrentPage = 1,
                PageSize = 5
            };
            
            await _inMemoryDbContext.Users.AddRangeAsync(userList);
            await _inMemoryDbContext.PrivateChats.AddRangeAsync(privateChat);
            await _inMemoryDbContext.SaveChangesAsync();

            //Act
            var pagedPrivateRoom =await mockGroupRepository.GetListAsync(userId, paginationRequest);

            //Assert

            Assert.NotNull(pagedPrivateRoom);
            Assert.True(pagedPrivateRoom.IsT0);
            Assert.IsType<PagedListResponse<PrivateRoomResponse>>(pagedPrivateRoom.AsT0.Value);
            Assert.NotNull(pagedPrivateRoom.AsT0.Value.Items);
            Assert.True(pagedPrivateRoom.AsT0.Value.Items.Any());
            Assert.Equal(4, pagedPrivateRoom.AsT0.Value.Items.Count);

            Dispose();
        }

        public void Dispose()
        {
            _inMemoryDbContext.Dispose(); // Dispose of the in-memory database context
        }
    }
}
