using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;

namespace ProjektBD
{
    public static class DataBase
    {
        // zmienny zwiazne z bazami danych
        private static string _connstring = "server=localhost;uid=root;pwd=Kutas123;database=projektbd";

        public static string Connstring => _connstring;
    }
}
