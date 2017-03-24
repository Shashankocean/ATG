using System;
using System.IO;
using Xamarin_LinkOS_Developer_Demo.iOS;
using Xamarin.Forms;

[assembly: Dependency(typeof(FileHelper))]
namespace Xamarin_LinkOS_Developer_Demo.iOS
{
    class FileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string docFolder = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            string libFolder = Path.Combine(docFolder, "..", "Library", "Databases");

            if (!Directory.Exists(libFolder))
            {
                Directory.CreateDirectory(libFolder);
            }

            return Path.Combine(libFolder, filename);
        }
    }
}