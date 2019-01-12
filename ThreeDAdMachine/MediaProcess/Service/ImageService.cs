using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using MediaProcess.Model;
using Newtonsoft.Json;
using MediaProcess.ExtensionMethod;
using Point = System.Windows.Point;
using Size = System.Windows.Size;

namespace MediaProcess.Service
{
    public class ImageService
    {
        /// <summary>
        /// judge the type of the image according path
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static ImageType GetImageType(string path)
        {
            ImageType type = ImageType.UnKnown;
            if (string.IsNullOrEmpty(path))
                return type;
            //judge by extension name
            try
             {
                string exName = Path.GetExtension(path).ToLower(),
                       filter = "*.jpg|*.bmg|*.png|*.gif";
                string[] kinds = filter.Split('*', '|');
                foreach (var kind in kinds)
                {
                    if (string.IsNullOrEmpty(kind))
                        continue;
                    if (kind == exName)
                    {
                        if (Enum.TryParse(CultureInfo.CurrentCulture.TextInfo.ToTitleCase(kind.Remove(0,1)), out type))
                            break;
                    }
                }
                return type;
            }
            catch (Exception)
            {
                return ImageType.UnKnown;
            }
        }

        public static Size GetImageSize(string path)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException();
            if (!File.Exists(path))
                throw new ArgumentException("Image does not exist");
            using (Bitmap b = new Bitmap(path))
            {
                Size s = new Size(b.Width, b.Height);
                return s;
            }
        }

        public static bool TryGetImageSize(string path, out Size size)
        {
            bool result = true;
            size = default(Size);
            try
            {
                size = GetImageSize(path);
            }
            catch
            {
                result = false;
            }
            return result;
        }

        /// <summary>
        /// GetImageType async
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static async Task<ImageType> GetImageTypeAsync(string path)
        {
            Func<string, ImageType> func = GetImageType;
            return await Task.Run(new Func<ImageType>(() => func(path)));
        }

        public static bool Save(IEnumerable<ImageModel> collection,string fullPath)
        {
            bool result = true;
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath) ?? throw new ArgumentException("path error"));
                using (StreamWriter writer = File.CreateText(fullPath))
                {
                    JsonSerializer serializer = JsonSerializer.Create(new JsonSerializerSettings()
                    {
                        Formatting = Formatting.Indented
                    });
                    serializer.Serialize(writer, collection);
                }
            }
            catch
            {
                result = false;
            }
            return result;
        }

        public static bool LoadImagesFromFile<T>(string fullPath,out T imageModels) 
            where T : IEnumerable<MediaBaseModel>
        {
            bool result = false;
            if (File.Exists(fullPath))
                using (StreamReader reader = File.OpenText(fullPath))
                {
                    JsonSerializer serializer = JsonSerializer.Create();
                    imageModels = (T)serializer.Deserialize(reader, typeof(T));
                    result = true;
                }
            else
                imageModels = default(T);
            return result;
        }

        #region SampleMethod

        private static Image<TColor, TDepth> GetImageRoi<TColor,TDepth>(Image<TColor, TDepth> srcImg,Rect roiRect)
            where TColor :struct,IColor where TDepth : new()
        {
            //Rectangle srcRectangle = new Rectangle(new System.Drawing.Point(0, 0), srcImg.Size);
            Rectangle roiRectangle = new Rectangle((int)roiRect.X, (int)roiRect.Y,
                (int)roiRect.Width, (int)roiRect.Height);
            //if (!srcRectangle.Contains(roiRectangle))
            //    throw new ArgumentException("Selected region out of range");
            using (Mat roiImg = new Mat(srcImg.Mat, roiRectangle))
                return roiImg.ToImage<TColor, TDepth>();
        }

        private static Image<TColor, TDepth> GetImageRoi<TColor, TDepth>(Mat srcImg, Rect roiRect)
            where TColor : struct, IColor where TDepth : new()
        {
            Rectangle roiRectangle = new Rectangle((int)roiRect.X, (int)roiRect.Y,
                (int)roiRect.Width, (int)roiRect.Height);
            using (Mat roiImg = new Mat(srcImg, roiRectangle))
                return roiImg.ToImage<TColor, TDepth>();
        }

        private static void RadiusDirectionSample(int radius,int centerX,int centerY, double angle,
            byte[,,]imgData,byte[,]dataBuf)
        {
            int count = 0;
            Point ptNow = default(Point);
            for (int r = radius; r > 0; r--)
            {
                ptNow.X = centerX + r * Math.Cos(angle * Math.PI / 180);
                ptNow.Y = centerY + r * Math.Sin(angle * Math.PI / 180);
                for (int j = 0; j < imgData.GetLength(2); j++, count++)
                {
                    byte v = imgData[(int) (ptNow.X - 0.5), (int) (ptNow.Y - 0.5), j];
                    for (int bitCounts = 7; bitCounts >= 0; bitCounts--)
                    {
                        dataBuf[bitCounts, count / 8] |= (byte)(((v & 1 << bitCounts) >> bitCounts) <<
                                                                (7 - count % 8));
                    }
                }
            }
        }

        private static void WriteBitColData(Stream stream, byte[,] data)
        {
            for (int i = data.GetLength(0) - 1; i >= 0; i--)
            {
                for (int j = 0; j < data.GetLength(1); j++)
                    stream.WriteByte(data[i, j]);
            }
            Array.Clear(data, 0, data.Length);
        }

        public static void ImageSample(Mat mat, DataModel sampleSetting, Stream stream)
        {
             Image<Bgr,byte> imgToSample = GetImageRoi<Bgr,byte>(mat, sampleSetting.SelectedRegion);
            int bitCountOfRadius = (sampleSetting.Radius * imgToSample.NumberOfChannels),
                centerX = imgToSample.Width / 2,
                centerY = imgToSample.Height / 2;
            byte[,] dataBuf = new byte[8, bitCountOfRadius / 8 + (bitCountOfRadius % 8 == 0 ? 0 : 1)];
            byte[,,] imgData = imgToSample.Data;
            if (!Directory.Exists(Path.GetDirectoryName(sampleSetting.DataPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(sampleSetting.DataPath) ?? throw new InvalidOperationException());
            for (double angle = 0; angle < 360; angle += sampleSetting.IntervalAngle)
            {
                RadiusDirectionSample(sampleSetting.Radius, centerX, centerY, angle,
                    imgData, dataBuf);
                WriteBitColData(stream, dataBuf);
            }
        }

        /// <summary>
        /// Sample Image method
        /// </summary>
        /// <param name="imgToSample"></param>
        /// <param name="sampleSetting"></param>
        public static void ImageSample(Image<Bgr, byte> imgToSample, DataModel sampleSetting)
        {
            if (!sampleSetting.ValidateRadius())
                throw new ArgumentException("Invalidate radius");
            imgToSample = GetImageRoi(imgToSample,sampleSetting.SelectedRegion);
            int bitCountOfRadius = (sampleSetting.Radius * imgToSample.NumberOfChannels),
                centerX = imgToSample.Width / 2,
                centerY = imgToSample.Height / 2;

            byte[,] dataBuf = new byte[8, bitCountOfRadius / 8 + (bitCountOfRadius % 8 == 0 ? 0 : 1)];
            byte[,,] imgData = imgToSample.Data;
            if (!Directory.Exists(Path.GetDirectoryName(sampleSetting.DataPath)))
                Directory.CreateDirectory(Path.GetDirectoryName(sampleSetting.DataPath) ?? throw new InvalidOperationException());

            using (FileStream fs = new FileStream(sampleSetting.DataPath, FileMode.OpenOrCreate))
            {
                for (double angle = 0; angle < 360; angle += sampleSetting.IntervalAngle)
                {
                    RadiusDirectionSample(sampleSetting.Radius, centerX, centerY, angle,
                        imgData, dataBuf);
                    WriteBitColData(fs, dataBuf);
                }
                fs.Close();
            }
        }

        public static void ImageSample(string path, DataModel sampleSetting)
        {
            if (string.IsNullOrEmpty(path))
                return;
            try
            {
                ImageSample(new Image<Bgr, byte>(path), sampleSetting);
            }
            catch
            {// ignore
            }
        }

        public static async Task ImageSampleAsync(Image<Bgr, byte> imgToSample, DataModel sampleSetting)
        {
            await Task.Run(() => { ImageSample(imgToSample, sampleSetting); });
        }

        public static async Task ImageSampleAsync(string path, DataModel sampleSetting)
        {
            ImageType type = GetImageType(path);
            if (type == ImageType.Gif)
            {
                await Task.Run(() =>
                {
                    sampleSetting.DataPath.CreateDirectoryIfNotExist();
                    using (FileStream fs = new FileStream(sampleSetting.DataPath, FileMode.OpenOrCreate))
                    using (Capture c = new Capture(path))
                    {
                        using (Image img = Image.FromFile(path, true))
                        {
                            int frameCount = img.GetFrameCount(new System.Drawing.Imaging.FrameDimension(img.FrameDimensionsList[0]));
                            for (int i = 0; i < frameCount; i++)
                            {
                                c.SetCaptureProperty(CapProp.PosFrames, i);
                                using (Mat m = c.QueryFrame())
                                    ImageSample(m, sampleSetting, fs);
                            }
                        }
                            
                    }
                });
            }
            else
                await ImageSampleAsync(new Image<Bgr, byte>(path), sampleSetting);
        }

        #endregion
    }
}
