using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Xamarin_LinkOS_Developer_Demo
{
    
    class DatabaseQuery
    {
        readonly SQLiteAsyncConnection connection;

        public DatabaseQuery(string db_path)
        {
            connection = new SQLiteAsyncConnection(db_path);
            connection.CreateTableAsync<business_DB>().Wait();
        }
        public Task<List<business_DB>> get_business_data()
        {
            connection.Table<business_DB>();
            return connection.QueryAsync<business_DB>("select * from business_DB ORDER BY date_time DESC limit 50");
        }
        public Task<List<business_DB>> get_asset()
        {
            connection.Table<business_DB>();
            return connection.QueryAsync<business_DB>("select * from business_DB ORDER BY date_time DESC limit 1");
        }
        public Task<List<business_DB>> check_asset(string asset)
        {
            return connection.QueryAsync<business_DB>("select * from business_DB where asset_number='"+asset+"'");
        }
        public Task<int> SaveBusinessAsync(business_DB item)
        {
            if (item.business_Id != 0)
            {
                return connection.UpdateAsync(item);
            }
            else
            {
                return connection.InsertAsync(item);
            }
        }
    }
}
