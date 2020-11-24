using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace CaritaUAT
{
    public interface ISQLite
    {
        SQLiteConnection GetConnection();
    }

}
