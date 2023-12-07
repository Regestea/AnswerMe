using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AnswerMe.Domain.Entities;
using Models.Shared.Responses.Media;

namespace AnswerMe.Application.Extensions
{
    /// <summary>
    /// This class provides helper methods for working with file storage.
    /// </summary>
    public static class FileStorageHelper
    {
        /// <summary>
        /// The URL of the object storage.
        /// </summary>
        private static string _ObjectStorageUrl = "";

        /// <summary>
        /// This method initializes the object storage URL.
        /// </summary>
        /// <param name="objectStorageUrl">The URL of the object storage.</param>
        /// <remarks>
        /// You should call this method before using any other methods that interact with the object storage.
        /// </remarks>
        public static void Initialize(string objectStorageUrl)
        {
            _ObjectStorageUrl = objectStorageUrl;
        }

        /// <summary>
        /// Returns the full URL for a given file path.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <returns>The full URL.</returns>
        public static string GetUrl(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return "";
            }
            return _ObjectStorageUrl + filePath;
        }

        /// Determines the media response type based on the file format.
        /// @param fileFormat The file format to check.
        /// @returns The media response type.
        /// /
        public static MediaTypeResponse GetMediaResponseType(string fileFormat)
        {
                MediaTypeResponse mediaType;
            switch (fileFormat)
            {
                case "jpg":
                case "jpeg":
                case "png":
                    mediaType = MediaTypeResponse.image;
                    break;
                case "gif":
                    mediaType = MediaTypeResponse.gif;
                    break;
                case "mp4":
                case "mov":
                case "mkv":
                    mediaType = MediaTypeResponse.video;
                    break;
                case "mp3":
                case "ogg":
                case "wav":
                    mediaType = MediaTypeResponse.audio;
                    break;
                default:
                    mediaType = MediaTypeResponse.other;
                    break;
            }

            return mediaType;
        }

        /// <summary>
        /// Represents a media type.
        /// </summary>
        /// <remarks>
        /// The MediaType class is used to store and manipulate information about a specific media type.
        /// </remarks>
        public static MediaType GetMediaType(string fileFormat)
        {
            MediaType mediaType;
            switch (fileFormat)
            {
                case "jpg":
                case "jpeg":
                case "png":
                    mediaType = MediaType.image;
                    break;
                case "gif":
                    mediaType = MediaType.gif;
                    break;
                case "mp4":
                case "mov":
                case "mkv":
                    mediaType = MediaType.video;
                    break;
                case "mp3":
                case "ogg":
                case "wav":
                    mediaType = MediaType.audio;
                    break;
                default:
                    mediaType = MediaType.other;
                    break;
            }

            return mediaType;
        }
    }
}
