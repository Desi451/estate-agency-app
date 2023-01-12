using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektBD.sidebar_classes
{
    public static class Tools
    {
        public static bool CheckLogin(TextBox  tx)
        {
            string query = "SELECT COUNT(*) AS liczba FROM projektbd.users WHERE login= '" + tx.Text + "' ";

            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                if (reader["liczba"].ToString() == "1")
                {
                    MessageBox.Show("login juz wykorzstywany", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    con.Close();
                    return false;
                }
                else
                con.Close();
                return true;
            }
            MessageBox.Show("Nie odczytano danych", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        } // do usuniecia?

        public static void FillTextBoxesUser(TextBox t1, TextBox t2, TextBox t3, TextBox t4, 
            TextBox t5, ComboBox c6, TextBox t7, TextBox t8, TextBox t9, TextBox t10, TextBox t11, string query)
        {
            TextBox[] array = { t1, t2, t3, t4, t5, t7, t8, t8, t9, t10, t11 };
            int i = 1;

            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                foreach (TextBox tb in array)
                {
                    if(i != 0 && i != 10)
                    {
                        tb.Text = reader.GetString(i++);
                    }
                    else tb.Text = reader.GetInt32(i++).ToString();
                }
                c6.Text = reader.GetInt32(6).ToString();
            }
            con.Close();
        }

        public static void FillAllData(int id1, string first_name1, string last_name1, string login1, string password1, string email1,
           int id_rank1, string street1, string no_building1, string no_apartament1, int zip_code1, string city1)
        {
            User.Id = id1;
            User.FirstName = first_name1;
            User.LastName = last_name1;
            User.Login = login1;
            User.Password = password1;
            User.Email = email1;
            User.Id_rank = id_rank1;
            User.Street = street1;
            User.No_Building = no_building1;
            User.No_apartament = no_apartament1;
            User.ZipCode = zip_code1;
            User.City = city1;
        }

        public static int ReadComboId(string text)
        {
            int start= text.IndexOf(':');
            int end= text.IndexOf(".");
            text = text.Substring(start +1, end-3);
            end = Int32.Parse(text);
            return end;
        }

    }
}
