using System;
using System.IO;
using System.Windows;
using Newtonsoft.Json;

namespace MediaProcess.Model
{
    public class DataModel
    {
        #region Constructor
        public DataModel() { }

        public DataModel(string sourcePath, Size sourceSize,string targetDir = @"./MediaData/") :
            this(GenerateMediaDataPath(sourcePath,targetDir), 
                1,(int)Math.Min(sourceSize.Width/2,sourceSize.Height)/2)
        {
        }

        public DataModel(string dataPath, int minRadius, int maxRadius, int radius = 168,
            double intervalAngle = 0.8)
        {
            DataPath = dataPath;
            MinRadius = minRadius;
            MaxRadius = maxRadius;
            Radius = radius;
            IntervalAngle = intervalAngle;
        }

        #endregion

        #region Property

        public string DataPath { get; set; }
        public Rect SelectedRegion { get; set; }

        [JsonIgnore]
        public bool IsSampling { get; set; } = false;

        private int _maxRadius;
        public int MaxRadius
        {
            get => _maxRadius;
            set => _maxRadius = Math.Max(value, MinRadius);
        }

        private int _minRadius;
        public int MinRadius
        {
            get => _minRadius;
            set => _minRadius = Math.Min(value, MaxRadius);
        }

        private int _radius;
        public int Radius
        {
            get => _radius;
            set
            {
                value = Math.Max(Math.Min(MaxRadius, value), MinRadius);
                _radius = value;
            }
        }

        public double IntervalAngle { get; set; }

        #endregion

        #region Method

        private static string GenerateMediaDataPath(string sourcePath, string dir)
        {
            return dir + Path.GetFileNameWithoutExtension(sourcePath) + "/" +
                   Path.GetFileNameWithoutExtension(sourcePath) + ".bin";
        }

        public bool ValidateRadius()
        {
            return SelectedRegion.Height.Equals(SelectedRegion.Width) && SelectedRegion.Height >= Radius*2;
        }

        #endregion
    }
}
