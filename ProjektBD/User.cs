using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjektBD
{
    public static class User // klasa przechowuje zmienne aktualnie zalogowanego użytkownika
    {
        // wartosci typu int
        private static int id;
        public static int Id { get => id; set => id = value; }

        private static int id_rank;
        public static int Id_rank { get => id_rank; set => id_rank = value; }

        private static string zip_code; // musi zostac zmieniony z powrotem na wartosc typu int
        public static string ZipCode { get => zip_code; set => zip_code = value; }

        // wartosci typu string
        private static string first_name;
        public static string FirstName { get => first_name; set =>first_name = value; }

        private static string last_name;
        public static string LastName { get => last_name; set => last_name = value; }

        private static string login;
        public static string Login { get => login; set => login = value; }

        private static string password;
        public static string Password { get => password; set => password = value; }

        private static string email;
        public static string Email { get => email; set => email = value; }

        private static string street;
        public static string Street { get => street;set => street = value; }

        private static string no_building;
        public static string No_Building { get => no_building; set => no_building = value; }

        private static string no_apartament;
        public static string No_apartament { get => no_apartament; set => no_apartament = value; }

        private static string city;
        public static string City { get => city; set => city = value; }
    }
}
