using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using MySql.Data.MySqlClient;

namespace ProjektBD
{
    public static class DataBase // zmienny zwiazne z bazą danych
    {
        private static string _connstring = "server=localhost;uid=root;pwd=Kutas123;database=projektbd;Convert Zero Datetime=True";
        public static string Connstring => _connstring;


    }
}
