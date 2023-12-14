using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.Common.Interfaces;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using AnswerMe.Infrastructure.Persistence;
using AnswerMe.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Models.Shared.OneOfTypes;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Requests.Message;
using Models.Shared.Requests.Shared;
using Models.Shared.Responses.Media;
using Models.Shared.Responses.Message;
using Models.Shared.Responses.Shared;
using OneOf.Types;

namespace AnswerMe.Infrastructure.Repositories
{
    public class PrivateMessageService : IPrivateMessageService
    {
        private readonly AnswerMeDbContext _context;
        private readonly FileStorageService _fileStorageService;
        private readonly IPrivateHubService _privateHubService;

        public PrivateMessageService(AnswerMeDbContext context, FileStorageService fileStorageService, IPrivateHubService privateHubService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
            _privateHubService = privateHubService;
        }


        public async Task<CreateResponse<IdResponse>> SendAsync(Guid loggedInUserId, Guid userReceiverId, SendMessageRequest request)
        {
            var room = await _context.PrivateChats
                .Where(x => (x.User2Id == loggedInUserId || x.User2Id == userReceiverId) && (x.User1Id == loggedInUserId || x.User1Id == userReceiverId))
                .FirstOrDefaultAsync();

            var existReceiver = await _context.Users.IsAnyAsync(x => x.id == userReceiverId);

            if (!existReceiver)
            {
                return new NotFound();
            }

            if (room == null)
            {
                room = new PrivateChat()
                {
                    id = Guid.NewGuid(),
                    CreatedDate = DateTimeOffset.UtcNow,
                    User1Id = loggedInUserId,
                    User2Id = userReceiverId
                };
                await _context.PrivateChats.AddAsync(room);
                await _context.SaveChangesAsync();
            }

            var message = new Message()
            {
                id = Guid.NewGuid(),
                CreatedDate = DateTimeOffset.UtcNow,
                UserSenderId = loggedInUserId,
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
                    var storageResponse = await _fileStorageService.GetObjectPathAsync(loggedInUserId, tokenRequest.Token);
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
                new PreviewUserResponse()
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

            await _privateHubService.SendMessageAsync(room.id, messageResponse);
            await _context.Messages.AddAsync(message);

            await _context.SaveChangesAsync();

            return new Success<IdResponse>(new IdResponse() { FieldName = "MessageId", Id = message.id });
        }

        public async Task<ReadResponse<PagedListResponse<MessageResponse>>> GetListAsync(Guid loggedInUserId, Guid roomId, PaginationRequest paginationRequest)
        {
            var existRoom = await _context.PrivateChats.IsAnyAsync(x => x.id == roomId);

            if (!existRoom)
            {
                return new NotFound();
            }

            var isUserInRoom = await _context.PrivateChats.IsAnyAsync(x => x.id == roomId && (x.User1Id == loggedInUserId || x.User2Id == loggedInUserId));
            if (!isUserInRoom)
            {
                return new AccessDenied();
            }

            var messagesQuery = _context.Messages
                .Where(x => x.RoomChatId == roomId)
                .OrderByDescending(x => x.CreatedDate)
                .Select(message =>
                    new MessageResponse
                    {
                        id = message.id,
                        CreatedDate = message.CreatedDate,
                        MediaList = message.MediaList!.Select(x => new MediaResponse
                        {
                            Id = x.Id,
                            Type = (MediaTypeResponse)x.Type,
                            BlurHash = x.BlurHash,
                            Path = x.Path
                        }).ToList(),

                        Text = message.Text,
                        GroupInviteToken = message.GroupInvitationToken,
                        ModifiedDate = message.ModifiedDate,
                        ReplyMessageId = message.ReplyMessageId,
                        UserSender = _context.Users.Select(x => new PreviewUserResponse()
                        {
                            Id = x.id,
                            Name = x.FullName,
                            ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                        }).Single()
                    }).AsQueryable();

            var pagedResult = await PagedListResponse<MessageResponse>.CreateAsync(
                messagesQuery,
                paginationRequest.PageSize,
                paginationRequest.CurrentPage
            );

            return new Success<PagedListResponse<MessageResponse>>(pagedResult);

        }

        public async Task<UpdateResponse> UpdateAsync(Guid loggedInUserId, Guid messageId, EditMessageRequest request)
        {
            var message = await _context.Messages
                .Where(x => x.UserSenderId == loggedInUserId && x.id == messageId)
                .Include(message => message.MediaList)
                .SingleOrDefaultAsync();

            if (message == null)
            {
                return new NotFound();
            }

            message.Text = request.Text;
            message.ModifiedDate = DateTimeOffset.UtcNow;

            _context.Messages.Update(message);

            await _context.SaveChangesAsync();

            var userSender = await _context.Users
                .Where(x => x.id == message.UserSenderId)
                .Select(x => new PreviewUserResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                })
                .SingleAsync();

            var messageResponse = new MessageResponse()
            {
                id = message.id,
                CreatedDate = message.CreatedDate,
                ModifiedDate = message.ModifiedDate,
                ReplyMessageId = message.ReplyMessageId,
                Text = message.Text,
                UserSender = userSender,
                MediaList = new List<MediaResponse>()
            };

            if (message.MediaList != null && message.MediaList.Any())
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


            await _privateHubService.UpdateMessageAsync(message.RoomChatId, messageResponse);

            return new Success();

        }

        public async Task<UpdateResponse> UpdateMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId, EditMessageMediaRequest request)
        {
            var existMessage = await _context.Messages.IsAnyAsync(x => x.id == messageId);

            if (existMessage)
            {
                return new NotFound();
            }

            var message = await _context.Messages
                .Where(x => x.UserSenderId == loggedInUserId && x.id == messageId)
                .Include(message => message.MediaList)
                .SingleOrDefaultAsync();

            if (message == null)
            {
                return new AccessDenied();
            }

            if (string.IsNullOrWhiteSpace(request.MediaToken))
            {
                return new ValidationFailed() { Field = nameof(request.MediaToken), Error = $"Invalid {request.MediaToken}" };
            }


            var storageResponse = await _fileStorageService.GetObjectPathAsync(loggedInUserId, request.MediaToken);

            if (string.IsNullOrWhiteSpace(storageResponse.FilePath))
            {
                return new ValidationFailed() { Field = nameof(request.MediaToken), Error = $"Invalid {request.MediaToken}" };
            }

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

            if (message.MediaList == null)
            {
                return new ValidationFailed() { Field = nameof(messageId), Error = $"Invalid {nameof(messageId)}" };
            }

            var messageMedias = message.MediaList.ToList();
            var oldMedia = messageMedias.SingleOrDefault(x => x.Id == mediaId);

            if (oldMedia == null)
            {
                return new ValidationFailed() { Field = nameof(mediaId), Error = $"Invalid {nameof(mediaId)}" };
            }

            await _fileStorageService.DeleteObjectAsync(loggedInUserId, oldMedia.Path);

            var oldIndex = messageMedias.IndexOf(oldMedia);

            var newMedia = new Media()
            {
                Id = oldMedia.Id,
                MessageId = oldMedia.MessageId,
                Path = storageResponse.FilePath,
                Type = FileStorageHelper.GetMediaType(storageResponse.FileFormat),

            };

            if (!string.IsNullOrWhiteSpace(storageResponse.BlurHash))
            {
                media.BlurHash = storageResponse.BlurHash;
            }

            messageMedias[oldIndex] = newMedia;

            message.MediaList = messageMedias;

            _context.Messages.Update(message);

            await _context.SaveChangesAsync();

            var userSender = await _context.Users
                .Where(x => x.id == message.UserSenderId)
                .Select(x => new PreviewUserResponse()
                {
                    Id = x.id,
                    Name = x.FullName,
                    ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                })
                .SingleAsync();

            var messageResponse = new MessageResponse()
            {
                id = message.id,
                CreatedDate = message.CreatedDate,
                ModifiedDate = DateTimeOffset.UtcNow,
                UserSender = userSender,
                Text = message.Text,
                ReplyMessageId = message.ReplyMessageId,
                MediaList = new List<MediaResponse>()
            };

            if (message.MediaList != null && message.MediaList.Any())
            {
                foreach (Media messageMedia in message.MediaList)
                {
                    messageResponse.MediaList.Add(new MediaResponse()
                    {
                        Id = messageMedia.Id,
                        Type = (MediaTypeResponse)messageMedia.Type,
                        BlurHash = messageMedia.BlurHash,
                        Path = FileStorageHelper.GetUrl(messageMedia.Path)
                    });
                }
            }


            await _privateHubService.UpdateMessageAsync(message.RoomChatId, messageResponse);

            return new Success();
        }

        public async Task<DeleteResponse> DeleteAsync(Guid loggedInUserId, Guid messageId)
        {
            var message = await _context.Messages
                .Where(x => x.UserSenderId == loggedInUserId && x.id == messageId)
                .Include(message => message.MediaList)
                .SingleOrDefaultAsync();

            if (message != null)
            {
                if (message.MediaList != null && message.MediaList.Any())
                {
                    foreach (var media in message.MediaList)
                    {
                        await _fileStorageService.DeleteObjectAsync(loggedInUserId, media.Path);
                    }
                }

                _context.Messages.Remove(message);
                await _context.SaveChangesAsync();

                await _privateHubService.RemoveMessageAsync(message.RoomChatId, message.id);

                return new Success();
            }

            return new NotFound();
        }

        public async Task<DeleteResponse> DeleteMediaAsync(Guid loggedInUserId, Guid messageId, Guid mediaId)
        {
            var existMessage = await _context.Messages.IsAnyAsync(x => x.id == messageId);

            if (!existMessage)
            {
                return new NotFound();
            }

            var message = await _context.Messages
                .Where(x => x.UserSenderId == loggedInUserId && x.id == messageId)
                .Include(message => message.MediaList)
                .SingleOrDefaultAsync();

            if (message?.MediaList != null && message.MediaList.Any())
            {
                var media = message.MediaList.SingleOrDefault(x => x.Id == mediaId);
                if (media != null)
                {
                    await _fileStorageService.DeleteObjectAsync(loggedInUserId, media.Path);

                    message.MediaList.Remove(media);

                    _context.Messages.Update(message);

                    await _context.SaveChangesAsync();

                    var userSender = await _context.Users
                        .Where(x => x.id == message.UserSenderId)
                        .Select(x => new PreviewUserResponse()
                        {
                            Id = x.id,
                            Name = x.FullName,
                            ProfileImage = FileStorageHelper.GetUrl(x.ProfileImage)
                        })
                        .SingleAsync();

                    var messageResponse = new MessageResponse()
                    {
                        id = message.id,
                        CreatedDate = message.CreatedDate,
                        ModifiedDate = DateTimeOffset.UtcNow,
                        UserSender = userSender,
                        Text = message.Text,
                        ReplyMessageId = message.ReplyMessageId,
                        MediaList = new List<MediaResponse>()
                    };

                    if (message.MediaList != null && message.MediaList.Any())
                    {
                        foreach (var messageMedia in message.MediaList)
                        {
                            messageResponse.MediaList.Add(new MediaResponse()
                            {
                                Id = messageMedia.Id,
                                Type = (MediaTypeResponse)messageMedia.Type,
                                BlurHash = messageMedia.BlurHash,
                                Path = FileStorageHelper.GetUrl(messageMedia.Path)
                            });
                        }
                    }

                    await _privateHubService.UpdateMessageAsync(message.RoomChatId, messageResponse);

                    return new Success();
                }
            }

            return new NotFound();
        }
    }
}
