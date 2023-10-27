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
    public static class FileStorageHelper
    {
        private static string _ObjectStorageUrl = "";

        public static void Initialize(string objectStorageUrl)
        {
            _ObjectStorageUrl = objectStorageUrl;
        }

        public static string GetUrl(string? filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return "";
            }
            return _ObjectStorageUrl + filePath;
        }

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
