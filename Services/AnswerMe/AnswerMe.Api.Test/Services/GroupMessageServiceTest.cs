﻿using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Api.Test.Services
{
    public class GroupMessageServiceTest
    {
        private readonly DbContextOptions<AnswerMeDbContext> dbContextOptions;

        private AnswerMeDbContext _inMemoryDbContext;

        public GroupMessageServiceTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<AnswerMeDbContext>()
                .UseInMemoryDatabase(databaseName: "AnswerMe")
                .Options;
            _inMemoryDbContext = new AnswerMeDbContext(dbContextOptions);
        }


        [Fact]
        public async Task MessageResponseList_Should_GetListOfPagedMessageResponse()
        {
            //Arrange
            
            var mockGroupMessage = new GroupMessageService(_inMemoryDbContext);

            var loggedInUserId = Guid.NewGuid();
            var groupId = Guid.NewGuid();

            await _inMemoryDbContext.Users.AddAsync(new User()
            {
                id = loggedInUserId,
                FullName = "TestUser",
                IdName = "UserNameTest",
                PhoneNumber = "123456789987"
            });

            await _inMemoryDbContext.GroupChats.AddAsync(new GroupChat()
            {
                id = groupId,
                Name = "groupName"
            });

            await _inMemoryDbContext.UserGroups.AddAsync(new UserGroup()
            {
                UserId = loggedInUserId,
                GroupId = groupId
            });

            var paginationRequest = new PaginationRequest()
            {
                CurrentPage = 1,
                PageSize = 5
            };

            for (int i = 0; i < 18; i++)
            {
                await _inMemoryDbContext.Messages.AddAsync(new Message()
                {
                    id = Guid.NewGuid(),
                    UserSenderId = loggedInUserId,
                    RoomChatId = groupId,
                    Text = "hello World" + Guid.NewGuid(),
                    MediaList = new List<Media>()
                    {
                        new Media(){Id = Guid.NewGuid(),MessageId = Guid.NewGuid(),Type = MediaType.image,Path = "some/where"},
                        new Media(){Id = Guid.NewGuid(),MessageId = Guid.NewGuid(),Type = MediaType.image,Path = "some/where"}
                    }
                });
            }

            await _inMemoryDbContext.SaveChangesAsync();

            //Act

            var pagedMessageList = await mockGroupMessage.GetListAsync(loggedInUserId, groupId, paginationRequest);

            //Assert

            Assert.NotNull(pagedMessageList);
            Assert.True(pagedMessageList.IsT0);
            Assert.IsType<PagedListResponse<MessageResponse>>(pagedMessageList.AsT0.Value);

            Dispose();
        }

        public void Dispose()
        {
            _inMemoryDbContext.Dispose(); // Dispose of the in-memory database context
        }
    }
}