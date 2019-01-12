using System.Windows;
using MediaProcess.Service;

namespace MediaProcess.Model
{
    public enum ImageType
    {
        Bmp = 6677,
        Gif = 7173,
        Png = 13780,
        Jpg = 255216,
        UnKnown
    }

    public class ImageModel : MediaBaseModel
    {
        #region Constructor

        public ImageModel() { }

        public ImageModel(string url)
        {
            Init(url);
        }

        #endregion

        #region Property

        public ImageType ImageType { get; set; }

        /// <summary>
        /// Size of the Image X:Width Y:Height
        /// </summary>
        public override Size FrameSize
        {
            get => ImageService.GetImageSize(Path);
        }

        #endregion

        #region Method

        private void Init(string url)
        {
            if (string.IsNullOrEmpty(url))
                return;
            Path = url;
            ImageType = ImageService.GetImageType(url);
            DataModel = new DataModel(url, FrameSize);
        }

        #endregion
    }
}
