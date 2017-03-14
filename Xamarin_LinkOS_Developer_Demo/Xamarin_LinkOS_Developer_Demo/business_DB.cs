using SQLite;

namespace Xamarin_LinkOS_Developer_Demo
{
    class business_DB
    {
        [PrimaryKey, AutoIncrement]
        public int business_Id { get; set; }
        public string business_name { get; set; }
        [Unique]
        public string asset_number { get; set; }
        public string phone_number { get; set; }
        public string location { get; set; }
        public string date { get; set; }
        public string due_date { get; set; }
        public string date_time { get; set; }
    }
}
