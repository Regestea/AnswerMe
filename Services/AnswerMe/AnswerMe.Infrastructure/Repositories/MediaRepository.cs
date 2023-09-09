using AnswerMe.Application.Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Application.DTOs.Media;
using AnswerMe.Application.Extensions;
using AnswerMe.Domain.Entities;
using Models.Shared.RepositoriesResponseTypes;
using Models.Shared.Responses.Media;
using Models.Shared.Responses.Shared;
using AnswerMe.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using OneOf.Types;
using AnswerMe.Infrastructure.Services;
using Models.Shared.DTOs.Error;


namespace AnswerMe.Infrastructure.Repositories
{
    public class MediaRepository : IMediaRepository
    {
        private readonly AnswerMeDbContext _context;
        private readonly FileStorageService _fileStorageService;

        public MediaRepository(AnswerMeDbContext context, FileStorageService fileStorageService)
        {
            _context = context;
            _fileStorageService = fileStorageService;
        }

        //public async Task<ReadResponse<MediaResponse>> GetMediaByIdAsync(Guid id)
        //{
        //    var media = await _context.Medias.Select(x => new MediaResponse()
        //    {
        //        id = x.id,
        //        BlurHash = x.BlurHash,
        //        CreatedDate = x.CreatedDate,
        //        ModifiedDate = x.ModifiedDate,
        //        Type = (MediaTypeResponse)x.Type,
        //        Path = FileStorageHelper.GetUrl(x.Path)
        //    }).SingleOrDefaultAsync(x => x.id == id);

        //    if (media == null)
        //    {
        //        return new NotFound();
        //    }

        //    return new Success<MediaResponse>(media);
        //}

        //public async Task<CreateResponse<IdResponse>> AddMediaAsync(Guid userId, AddMediaDto request)
        //{
        //    var objectStorage = await _fileStorageService.GetObjectPathAsync(userId, request.MediaToken);

        //    if (string.IsNullOrWhiteSpace(objectStorage.FilePath))
        //    {
        //        var validationFailedList = new List<ValidationFailedDto>
        //        {
        //            new ValidationFailedDto()
        //            {
        //                Field = nameof(request.MediaToken),
        //                Error = $"Invalid {request.MediaToken}"
        //            }
        //        };
        //        return validationFailedList;
        //    }

        //    var mediaType = (MediaType)FileStorageHelper.GetMediaType(objectStorage.FileFormat);

        //    var media = new Media()
        //    {
        //        id = Guid.NewGuid(),
        //        CreatedDate = DateTimeOffset.UtcNow,
        //        Path = objectStorage.FilePath,
        //        Type = (MediaType)FileStorageHelper.GetMediaType(objectStorage.FileFormat)
        //    };

        //    if (mediaType == MediaType.image)
        //    {
        //        media.BlurHash = objectStorage.BlurHash;
        //    }

        //    await _context.AddAsync(media);

        //    await _context.SaveChangesAsync();

        //    return new Success<IdResponse>(new IdResponse() { Id = media.id });
        //}

        //public async Task<UpdateResponse> EditMediaAsync(Guid userId, Guid mediaId, EditMediaDto request)
        //{
        //    var mediaTask = _context.Medias.SingleOrDefaultAsync(x => x.id == mediaId);

        //    var objectStorageTask = _fileStorageService.GetObjectPathAsync(userId, request.MediaToken);

        //    await Task.WhenAll(mediaTask, objectStorageTask);

        //    var media = await mediaTask;
        //    var objectStorage = await objectStorageTask;

        //    if (media == null)
        //    {
        //        return new NotFound();
        //    }

        //    if (string.IsNullOrWhiteSpace(objectStorage.FilePath))
        //    {
        //        var validationFailedList = new List<ValidationFailedDto>
        //        {
        //            new ValidationFailedDto()
        //            {
        //                Field = nameof(request.MediaToken),
        //                Error = $"Invalid {request.MediaToken}"
        //            }
        //        };
        //        return validationFailedList;
        //    }
        //    var mediaType = (MediaType)FileStorageHelper.GetMediaType(objectStorage.FileFormat);

        //    media.Type = mediaType;
        //    media.Path = objectStorage.FilePath;
        //    media.ModifiedDate = DateTimeOffset.UtcNow;
        //    if (mediaType == MediaType.image)
        //    {
        //        media.BlurHash = objectStorage.BlurHash;
        //    }

        //    _context.Medias.Update(media);

        //    await _context.SaveChangesAsync();

        //    return new Success();
        //}

        //public async Task<DeleteResponse> DeleteMediaAsync(Guid userId, Guid mediaId)
        //{
        //    var media = await _context.Medias.SingleOrDefaultAsync(x => x.id == mediaId);

        //    if (media != null)
        //    {
        //        await _fileStorageService.DeleteObjectAsync(userId, media.Path);
        //        _context.Medias.Remove(media);

        //        await _context.SaveChangesAsync();
        //    }

        //    return new Success();
        //}
        public Task<ReadResponse<MediaResponse>> GetMediaByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<CreateResponse<IdResponse>> AddMediaAsync(Guid userId, AddMediaDto request)
        {
            throw new NotImplementedException();
        }

        public Task<UpdateResponse> EditMediaAsync(Guid userId, Guid mediaId, EditMediaDto request)
        {
            throw new NotImplementedException();
        }

        public Task<DeleteResponse> DeleteMediaAsync(Guid userId, Guid mediaId)
        {
            throw new NotImplementedException();
        }
    }
}
