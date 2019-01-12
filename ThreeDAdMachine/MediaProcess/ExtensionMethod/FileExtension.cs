using System.IO;

namespace MediaProcess.ExtensionMethod
{
    public static class StringExtension
    {
        public static void CreateDirectoryIfNotExist(this string path)
        {
            if (string.IsNullOrEmpty(path))
                return;
            string dir = Path.GetDirectoryName(path);
            if (string.IsNullOrEmpty(dir))
                return;
            if (Directory.Exists(dir))
                return;
            Directory.CreateDirectory(dir);
        }
    }
}
