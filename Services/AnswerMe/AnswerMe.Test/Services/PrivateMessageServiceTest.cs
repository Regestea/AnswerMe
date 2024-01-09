using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;

namespace AnswerMe.Test.Services
{
    public class PrivateMessageServiceTest
    {
        private readonly DbContextOptions<AnswerMeDbContext> dbContextOptions;

        private AnswerMeDbContext _inMemoryDbContext;

        public PrivateMessageServiceTest()
        {
            dbContextOptions = new DbContextOptionsBuilder<AnswerMeDbContext>()
                .UseInMemoryDatabase(databaseName: "AnswerMe")
                .Options;
        }


        [Fact]
        public async Task MessageResponseList_Should_JumpToUnReadMessages_GetListOfPagedMessageResponse()
        {
            //Arrange
            _inMemoryDbContext = new AnswerMeDbContext(dbContextOptions);

            var mockPrivateMessage = new PrivateMessageService(_inMemoryDbContext, null!, null!);

            var loggedInUserId = Guid.NewGuid();
            var contactId = Guid.NewGuid();
            var dateTime = DateTimeOffset.UtcNow;

            var userList = new List<User>()
            {
                new User()
                {
                    id = loggedInUserId,
                    FullName = "loggedInUser",
                    IdName = "loggedInUserIdName",
                    PhoneNumber = "123456789987"
                },
                new User()
                {
                    id = contactId,
                    FullName = "contactName",
                    IdName = "contactIdName",
                    PhoneNumber = "123456789988"
                }
            };
            var privateChat = new PrivateChat()
            {
                id = Guid.NewGuid(),
                User1Id = loggedInUserId,
                User2Id = contactId
            };
            await _inMemoryDbContext.Users.AddRangeAsync(userList);

            await _inMemoryDbContext.PrivateChats.AddAsync(privateChat);
            await _inMemoryDbContext.RoomLastSeen.AddAsync(new RoomLastSeen()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UserId = loggedInUserId,
                RoomId = privateChat.id,
                LastSeenUtc = dateTime.AddDays(-1)
            });

            var paginationRequest = new PaginationRequest()
            {
                CurrentPage = 1,
                PageSize = 10
            };
            
            for (int i = 0; i < 16; i++)
            {
                await _inMemoryDbContext.Messages.AddAsync(new Message()
                {
                    id = Guid.NewGuid(),
                    UserSenderId = contactId,
                    RoomChatId = privateChat.id,
                    Text = "old hello World" + i,
                    CreatedDate = dateTime.AddDays(-2),
                    MediaList = new List<Media>()
                    {
                        new Media()
                        {
                            Id = Guid.NewGuid(), MessageId = Guid.NewGuid(), Type = MediaType.image, Path = "some/where"
                        },
                        new Media()
                        {
                            Id = Guid.NewGuid(), MessageId = Guid.NewGuid(), Type = MediaType.image, Path = "some/where"
                        }
                    }
                });
            }

            for (int i = 0; i < 28; i++)
            {
                await _inMemoryDbContext.Messages.AddAsync(new Message()
                {
                    id = Guid.NewGuid(),
                    UserSenderId = contactId,
                    RoomChatId = privateChat.id,
                    Text = "new hello World" + i,
                    CreatedDate = dateTime,
                    MediaList = new List<Media>()
                    {
                        new Media()
                        {
                            Id = Guid.NewGuid(), MessageId = Guid.NewGuid(), Type = MediaType.image, Path = "some/where"
                        },
                        new Media()
                        {
                            Id = Guid.NewGuid(), MessageId = Guid.NewGuid(), Type = MediaType.image, Path = "some/where"
                        }
                    }
                });
            }

            await _inMemoryDbContext.SaveChangesAsync();

            //Act

            var pagedMessageList = await mockPrivateMessage.GetListAsync(loggedInUserId, privateChat.id,true, paginationRequest);

            //Assert

            Assert.NotNull(pagedMessageList);
            Assert.True(pagedMessageList.IsSuccess);
            Assert.IsType<PagedListResponse<MessageResponse>>(pagedMessageList.AsSuccess.Value);
            Assert.Equal(3,pagedMessageList.AsSuccess.Value.Page);

            Dispose();
        }

        private void Dispose()
        {
            _inMemoryDbContext.Dispose(); // Dispose of the in-memory database context
        }
    }
}