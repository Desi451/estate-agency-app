using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektBD.sidebar_classes
{
    public static class Tools
    {
        public static bool CheckLogin(TextBox tx)
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
                    if (i != 0 && i != 10)
                    {
                        tb.Text = reader.GetString(i++);
                    }
                    else tb.Text = reader.GetInt32(i++).ToString();
                }
                c6.Text = reader.GetInt32(6).ToString();
            }
            con.Close();
        }

        public static void CleanTextBoxesUser(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
        TextBox t5, ComboBox c6, TextBox t7, TextBox t8, TextBox t9, TextBox t10, TextBox t11)
        {
            TextBox[] array = { t1, t2, t3, t4, t5, t7, t8, t8, t9, t10, t11 };
            foreach (TextBox tb in array)
            {
                tb.Text = "";
            }
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

        // metoda odpowiedzialna za wyswietlanie panelu w oknie
        public static void SwitchVisibile(Panel tb)
        {
            if (tb.Visible == true)
            {
                tb.Visible = false;
            }
            else
                tb.Visible = true;
            tb.BringToFront();
        }

        // metoda do czytania id po ":" do "." ze stringa
        public static int ReadComboId(string text)
        {
            int start = text.IndexOf(':');
            int end = text.IndexOf(".");
            text = text.Substring(start + 1, end - 3);
            end = Int32.Parse(text);
            return end;
        }

        // metoda do czytania id statusu spotkania "." ze stringa
        public static int ReadMeetingId(string text)
        {
            int start = 0;
            int end = text.IndexOf(".");
            text = text.Substring(start, end);
            end = Int32.Parse(text);
            return end;
        }

        // metoda sprawdzajaca czy nie ustawiono statusu sprzedazy dla nieruchomosci ktora jest tylko na wynajem itp
        public static bool ValidateMeeting(int building, int status)
        {
            string query = "SELECT transaction_id FROM projektbd.buildings WHERE id = '" + building + "'";
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                if (reader.GetInt32(0) == 1 && (status == 4 || status == 5))
                {
                    MessageBox.Show("Nie mozesz dodawac statusu zwiazanego z sprzedaza" +
                    "dla budunyku ktory jest tylko dla wynajmu!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
                else if (reader.GetInt32(0) == 2 && (status == 2 || status == 3))
                {
                    MessageBox.Show("Nie mozesz dodawac statusu zwiazanego z najmem" +
                    "dla budunyku ktory jest tylko dla sprzedazy!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            return true;
        }

        // metoda wczytajaca jaki jest to rodzaj spotkania (tabela statusm)
        public static void LoadStatusofMeeting(ComboBox cb)
        {
            cb.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT * from projektbd.statusm";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                cb.Items.Add(reader.GetInt32(0) + ". " + reader.GetString(1));
            }
            con.Close();
        }

        // metoda wczytujaca do comboboxa liste uzytkownikow oraz agentow
        public static void LoadAddMeetingsPanelAgentsAndUsers(ComboBox cbA, ComboBox cbU)
        {
            cbA.Items.Clear();
            cbU.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT id_rank,id,first_name,last_name,email FROM projektbd.users WHERE id_rank IN (2,1);";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetInt32(0) == 2)
                    cbA.Items.Add("id: " + reader.GetInt32(1) + ". Imie i nazwisko:" +
                    reader.GetString(2) + " " + reader.GetString(3) + " mail: " + reader.GetString(4));

                if (reader.GetInt32(0) == 1)
                    cbU.Items.Add("id: " + reader.GetInt32(1) + ". Imie i nazwisko:" +
                reader.GetString(2) + " " + reader.GetString(3) + " mail: " + reader.GetString(4));
            }
            con.Close();
        }

        // metoda wczytajaca do Listboxa budynki oraz zapisujace id w liscie
        public static void LoadBuildings(ListBox cb, List<int> list)
        {
            cb.Items.Clear();
            list.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT b.id, b.size, b.basement, b.plot_of_land_size," +
                "\r\nCONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, CONCAT('/', b.no_apartament), ''),' ', b.city, ' ', b.zip_code ),\r\n" +
                "t.type_of, s.sell_price, r.rent_price, r.rent_min_time FROM projektbd.buildings b \r\n" +
                "INNER JOIN projektbd.transaction_type t ON b.transaction_id = t.id\r\n" +
                "LEFT JOIN projektbd.sell_details s ON b.id = s.id_buildings\r\n" +
                "LEFT JOIN projektbd.rent_details r ON b.id = r.id_buildings;";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                string piwnica;

                if (reader.GetInt32(2) == 1) piwnica = "Tak";
                else piwnica = "Nie";
                list.Add(reader.GetInt32(0));
                if(reader.IsDBNull(6) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " cena najmu: " + reader.GetInt32(7) 
                    + " PLN min. okres najmu: " + reader.GetInt32(8) + " msc");
                }
                else if(reader.IsDBNull(7) == true || reader.IsDBNull(8) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " cena: " + reader.GetInt32(6) + " PLN");
                }
                else
                cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " cena najmu: " + reader.GetInt32(7)
                + " PLN min. okres najmu: " + reader.GetInt32(8) + " msc. Kwota Zakupu: " + reader.GetInt32(6) + " PLN");

            }
            con.Close();
        }

        //metoda odblokowujaca textboxy w dodawaniu budynkow 
        // textBox -> sprzedaz
        // textBox2 i textBox3 -> wynajem 2(czas) 3(kwota)
        // index -> wybrana wartosc
        public static void UnlockTextBox(int index,TextBox textBox, TextBox textBox2, TextBox textBox3)
        {
            if (index == 0)
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
            else if(index == 1)
            {
                textBox.Enabled = true;
            }
            else if(index==2)
            {
                textBox.Enabled = true;
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
        }

        // metoda odpowiedzialna do powrot panelu do poczatkowego stanu
        public static void LockBackTextBox(TextBox textBox, TextBox textBox2, TextBox textBox3)
        {
            textBox.Enabled = false;
            textBox2.Enabled = false;
            textBox3.Enabled = false;
        }

        // metoda odpowiedzialna za wybór odpowiedniego query do dodania transackcji do budynku
        public static string SelectTransaction(int index, int sell, int rent, int rentTime, int buildingId)
        {
            if (index == 0)
            {
                return "INSERT INTO projektbd.rent_details (id_buildings, rent_price, rent_min_time) VALUES('"
                + buildingId + "', '" + rent + "', '" + rentTime + "');";

            }
            else if (index == 1)
            {
                return "INSERT INTO projektbd.sell_details (id_buildings,sell_price) VALUES('" + buildingId + "', '" + sell + "');";

            }
            else if (index == 2)
            {
                return "BEGIN; INSERT INTO projektbd.rent_details (id_buildings,rent_price, " +
                "rent_min_time) VALUES('" + buildingId + "', '" + rent + "', '" + rentTime + "' );" +
                "INSERT INTO projektbd.sell_details (id_buildings,sell_price) " +
                "VALUES('" + buildingId + "', '" + sell + "' ); COMMIT; ";
            }
            else return "empty";
        }

        // metoda pomocnicza do wyboru transackji sprawdza czy TB nie jest pusty(np gdy wybieramy sama sprzedaz)
        public static string CheckIfNull(string text)
        {
            if (string.IsNullOrEmpty(text)) return "0";
            return text;
        }

        // metoda odpowiedzialna za czwytywanie wartosci do comboBoxów zwiazanych z budynkami
        public static void LoadBuildingElements(ComboBox cb1, ComboBox cb2)
        {
            cb1.Items.Clear();
            cb2.Items.Clear();
            try
            {
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                string query = "SELECT _type, type_of FROM projektbd.type_buildings tp " +
                "LEFT JOIN projektbd.transaction_type t ON tp.id = t.id UNION SELECT " +
                "_type, type_of FROM projektbd.type_buildings tp " +
                "RIGHT JOIN projektbd.transaction_type t ON tp.id = t.id";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false) cb1.Items.Add(reader.GetString(0));
                    if (reader.IsDBNull(1)== false) cb2.Items.Add(reader.GetString(1));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // metoda odpowiedzialna za wczytanie parametrow do combo dot. piwnicy
        public static void LoadBasementListBox(ComboBox cb1)
        {
            cb1.Items.Add("Nie");
            cb1.Items.Add("Tak");
        }

        // metoda odpowiedzialna za wczytanie parametrow wybranego budynku do edycji // nwm co z tym chyba out
        public static void LoadEditingBuilding(ListBox cb, List<int> list)
        {
            
        }

        // metoda odpowiedzialna do blokowania/odblokowywania textboxow w zakladkach dot budynkow
        public static void LoadTransactionBuildings(int index, TextBox sell, TextBox rent, TextBox rentTime)
        {
            LockBackTextBox(sell,rent,rentTime);
            if (index == 0)
            {
                rent.Enabled= true;
                rentTime.Enabled= true;
            }
            else if (index == 1)
            {
                sell.Enabled = true;
            }
            else if (index == 2)
            {
                sell.Enabled = true;
                rent.Enabled = true;
                rentTime.Enabled = true;
            }
        }

        // metoda do walidacji i i edytowania stringa z zipcode
        public static int ZipCodeValidate(string zipcode)
        {
            if (Regex.IsMatch(zipcode, @"^\d{2}-\d{3}$"))
            {
                zipcode = zipcode.Replace("-", "");
                return int.Parse(zipcode);
            }
            else
            {
                Console.WriteLine("Invalid Polish zip code");
                return 0;
            }
            
        }

        //metoda do pobierania wartosci(kodu pocztowego) int z bazy danych i przerobienie na polski kod pocztowy
        public static string ZipCodeToString(int zipcode)
        {
            string zipCodeString = zipcode.ToString();
            zipCodeString = zipCodeString.Insert(2, "-");
            return zipCodeString;
        }

    }
}
