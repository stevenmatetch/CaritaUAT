using CaritaUAT.UWP;
using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_UWP))]

namespace CaritaUAT.UWP
{
    public class SQLite_UWP : ISQLite
    {
        public SQLite.SQLiteConnection GetConnection()
        {

            bool createtables = false;
            var sqliteFilename = "CaritaUATdb.db3";
            string path = Path.Combine(ApplicationData.Current.LocalFolder.Path, sqliteFilename);

            if (!File.Exists(path))
            {
                createtables = true;
            }

            var conn = new SQLite.SQLiteConnection(path);

            var t = conn.Query<int>("PRAGMA key='#CaritaUAT23#'");


            if (createtables)
            {
                CaritaUAT.Data.CaritaUATdb.CreateDatabase(conn);
            }

            return conn;
        }
    }
}
