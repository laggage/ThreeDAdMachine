using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Emgu.CV;
using MediaProcess.Model;
using Newtonsoft.Json;

namespace MediaProcess.Service
{
    public enum MediaType
    {
        Video,
        Image,
        Unknown
    }

    public class MediaService
    {
        public static MediaType GetMediaTypeByUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return MediaType.Unknown;
            else if (ImageService.GetImageType(url) != ImageType.UnKnown)
                return MediaType.Image;
            string exName = Path.GetExtension(url);
            switch (exName)
            {
                case ".mp4":
                case ".avi":
                    return MediaType.Video;
                default:
                    return MediaType.Unknown;
            }
        }

        public static bool SaveMediaList<T>(T list, string fullPath, MediaType type) where T : IEnumerable<MediaBaseModel>
        {
            bool result;

            if (list == null || !list.Any())
                result = false;
            else if (type == MediaType.Image)
                result = ImageService.Save((IEnumerable<ImageModel>)list, fullPath);
            else if (type == MediaType.Video)
                result = VideoService.SaveVideoList((IEnumerable<VideoModel>)list, fullPath);
            else
                result = false;

            return result;
        }

        public static bool LoadMediaList<T>(string fullPath, MediaType type, out T list) where T : IEnumerable<MediaBaseModel>
        {
            bool result = true;
            list = default(T);
            if (File.Exists(fullPath))
                if (type == MediaType.Image)
                    result = ImageService.LoadImagesFromFile<T>(fullPath, out list);
                else if (type == MediaType.Video)
                    result = VideoService.LoadVideoList(fullPath, out list);
                else
                {
                    result = false;
                }
            return result;
        }

        public static void ClearMediaDataCache(MediaBaseModel media)
        {
            string url = Path.GetDirectoryName(media.DataModel.DataPath);
            if (string.IsNullOrEmpty(url))
                return;
            if (Directory.Exists(url))
            {
                string[] files = Directory.GetFiles(url);
                foreach (var file in files)
                    File.Delete(file);
                Directory.Delete(url);
            }
        }
    }
}
