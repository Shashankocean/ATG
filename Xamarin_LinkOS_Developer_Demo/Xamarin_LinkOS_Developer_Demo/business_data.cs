using System.Collections.ObjectModel;
using Xamarin.Forms;

namespace Xamarin_LinkOS_Developer_Demo
{
    class business_data
    {
        static DatabaseQuery database;
        public static ObservableCollection<string> business_detail = new ObservableCollection<string>
        {
            "Business1","Business2","Business3"
        };
        public static DatabaseQuery db_connection
        {
            get
            { 
                    if (database == null)
                {
                    database = new DatabaseQuery(DependencyService.Get<IFileHelper>().GetLocalFilePath("ATG.db"));
                }
                return database;
            }
        }
    }
}
