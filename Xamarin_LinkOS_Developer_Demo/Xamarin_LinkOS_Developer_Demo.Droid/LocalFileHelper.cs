using Xamarin_LinkOS_Developer_Demo;
using Xamarin.Forms;
using ATG.Droid;

[assembly: Dependency(typeof(LocalFileHelper))]
namespace ATG.Droid
{
    class LocalFileHelper : IFileHelper
    {
        public string GetLocalFilePath(string filename)
        {
            string folder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);

            return System.IO.Path.Combine(folder, filename);
        }
    }
}