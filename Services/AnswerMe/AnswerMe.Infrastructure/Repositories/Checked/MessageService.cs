using AnswerMe.Application.Common.Interfaces.Checked;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;
using OneOf.Types;
using AnswerMe.Infrastructure.Services;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using Models.Shared.Responses.Media;

namespace AnswerMe.Infrastructure.Repositories.Checked
{
    public class MessageService : IMessageService
    {
        private AnswerMeDbContext _context;
        private readonly FileStorageService _fileStorageService;

        public MessageService(AnswerMeDbContext context, FileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }


        public async Task<ReadResponse<bool>> IsUserInGroupChat(Guid userId, Guid groupId)
        {
            var result = await _context.UserGroups.IsAnyAsync(x => x.UserId == userId && x.GroupId == groupId);

            return new Success<bool>(result);
        }

        public async Task<CreateResponse<IdResponse>> SendPrivateMessage(Guid userId, Guid userReceiverId, SendMessageRequest request)
        {
            var room = await _context.PrivateChats
                .Where(x => (x.User2Id == userId || x.User2Id == userReceiverId) && (x.User1Id == userId || x.User1Id == userReceiverId))
                .FirstOrDefaultAsync();

            if (room == null)
            {
                room = new PrivateChat()
                {
                    id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    User1Id = userId,
                    User2Id = userReceiverId
                };
                await _context.PrivateChats.AddAsync(room);
                await _context.SaveChangesAsync();
            }

            var message = new Message()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UserSenderId = userId,
                Text = request.Text,
                RoomChatId = room.id,
                MediaList = new List<Media>()
            };
            if (request.ReplyMessageId != null)
            {
                var existMessage = await _context.Messages.IsAnyAsync(x => x.id == request.ReplyMessageId);
                if (existMessage)
                {
                    message.ReplyMessageId = request.ReplyMessageId;
                }
            }

            if (request.MediaTokenList != null)
            {
                foreach (var tokenRequest in request.MediaTokenList)
                {
                    var storageResponse = await _fileStorageService.GetObjectPathAsync(userId, tokenRequest.Token);
                    if (!string.IsNullOrWhiteSpace(storageResponse.FilePath))
                    {
                        var media = new Media()
                        {
                            Id = Guid.NewGuid(),
                            MessageId = message.id,
                            Path = storageResponse.FilePath,
                            Type = FileStorageHelper.GetMediaType(storageResponse.FileFormat),
                        };

                        if (!string.IsNullOrWhiteSpace(storageResponse.BlurHash))
                        {
                            media.BlurHash = storageResponse.BlurHash;
                        }
                        message.MediaList.Add(media);
                    }
                }
            }

            var userSenderPreview = await _context.Users
                .Where(x => x.id == message.UserSenderId)
                .Select(x =>
                new UserPreviewResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                }).SingleAsync();

            var messageResponse = new MessageResponse()
            {
                id = message.id,
                CreatedDate = message.CreatedDate,
                ModifiedDate = message.ModifiedDate,
                Text = message.Text,
                UserSender = userSenderPreview,
                ReplyMessageId = message.ReplyMessageId,
                MediaList = new List<MediaResponse>()
            };
            if (message.MediaList != null)
            {
                foreach (Media media in message.MediaList)
                {
                    messageResponse.MediaList.Add(new MediaResponse()
                    {
                        Id = media.Id,
                        Type = (MediaTypeResponse)media.Type,
                        BlurHash = media.BlurHash,
                        Path = FileStorageHelper.GetUrl(media.Path)
                    });
                }
            }

            // send message by Signal r 

            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { Id = message.id });
        }

        public async Task<CreateResponse<IdResponse>> SendGroupMessage(Guid userId, Guid groupId, SendMessageRequest request)
        {
            var isUserInGroup = await _context.UserGroups.IsAnyAsync(x => x.UserId == userId && x.GroupId == groupId);

            if (!isUserInGroup)
            {
                return new Error<string>("you can't send message to this group");
            }


            var message = new Message()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UserSenderId = userId,
                Text = request.Text,
                RoomChatId = groupId,
                MediaList = new List<Media>()
            };
            if (request.ReplyMessageId != null)
            {
                var existMessage = await _context.Messages.IsAnyAsync(x => x.id == request.ReplyMessageId);
                if (existMessage)
                {
                    message.ReplyMessageId = request.ReplyMessageId;
                }
            }

            if (request.MediaTokenList != null)
            {
                foreach (var tokenRequest in request.MediaTokenList)
                {
                    var storageResponse = await _fileStorageService.GetObjectPathAsync(userId, tokenRequest.Token);
                    if (!string.IsNullOrWhiteSpace(storageResponse.FilePath))
                    {
                        var media = new Media()
                        {
                            Id = Guid.NewGuid(),
                            MessageId = message.id,
                            Path = storageResponse.FilePath,
                            Type = FileStorageHelper.GetMediaType(storageResponse.FileFormat),
                        };

                        if (!string.IsNullOrWhiteSpace(storageResponse.BlurHash))
                        {
                            media.BlurHash = storageResponse.BlurHash;
                        }
                        message.MediaList.Add(media);
                    }
                }
            }


            var userSenderPreview = await _context.Users
                .Where(x => x.id == message.UserSenderId)
                .Select(x =>
                    new UserPreviewResponse()
                    {
                        Id = x.id,
                        Name = x.FullName,
                        ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                    }).SingleAsync();

            var messageResponse = new MessageResponse()
            {
                id = message.id,
                CreatedDate = message.CreatedDate,
                ModifiedDate = message.ModifiedDate,
                Text = message.Text,
                UserSender = userSenderPreview,
                ReplyMessageId = message.ReplyMessageId,
                MediaList = new List<MediaResponse>()
            };
            if (message.MediaList != null)
            {
                foreach (Media media in message.MediaList)
                {
                    messageResponse.MediaList.Add(new MediaResponse()
                    {
                        Id = media.Id,
                        Type = (MediaTypeResponse)media.Type,
                        BlurHash = media.BlurHash,
                        Path = FileStorageHelper.GetUrl(media.Path)
                    });
                }
            }
            // send message by Signal r 

            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { Id = message.id });
        }

        public Task<ReadResponse<List<MessageResponse>>> GetPrivateMessages(Guid userId, Guid roomId, PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReadResponse<List<MessageResponse>>> GetGroupMessages(Guid userId, Guid roomId, PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ReadResponse<List<MessageResponse>>> GetMessages(Guid userId, Guid roomId, PaginationRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResponse> UpdateMessage(Guid userId, Guid messageId, UpdateMessageRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResponse> UpdateMessageMedia(Guid userId, Guid messageId, UpdateMessageMediaRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteResponse> DeleteMessage(Guid userId, Guid messageId)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteResponse> DeleteMessageMedia(Guid userId, Guid messageId, Guid mediaId)
        {
            throw new NotImplementedException();
        }
    }
}
