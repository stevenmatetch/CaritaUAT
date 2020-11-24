using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.Content.Res;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CaritaUAT.Droid;
using Java.Security;
using SQLite;
using Xamarin.Forms;

[assembly: Dependency(typeof(SQLite_Android))]

namespace CaritaUAT.Droid
{
    public class SQLite_Android : ISQLite
    {
        public SQLite.SQLiteConnection GetConnection()
        {

            bool createtables = false;

            var sqliteFilename = "CaritaUATdb.db3";
            var path = Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal), sqliteFilename);

            if (!File.Exists(path))
            {
                createtables = true;
            }

            var options = new SQLiteConnectionString(path, true,
                key: "#CaritaUAT23#");
            var conn = new SQLiteConnection(options);

            if (createtables)
            {
                CaritaUAT.Data.CaritaUATdb.CreateDatabase(conn);

            }

            return conn;
        }

    }
}