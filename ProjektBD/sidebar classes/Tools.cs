using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Utilities.Collections;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Management.Instrumentation;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektBD.sidebar_classes
{
    public static class Tools
    {
        // 
        // metody ogólne i admina
        //

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

        //wczytywanie wartosci dal wybranego id uzytkownika do edycji
        public static void FillTextBoxesAdmin(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
        TextBox t5, ComboBox c6, TextBox t7, TextBox t8, TextBox t9, TextBox t10, TextBox t11, string query)
        {
            TextBox[] array = { t1, t2, t3, t4, t5, t7, t8, t9, t10, t11 };
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
                    if (i != 0)
                    {
                        tb.Text = reader.GetString(i++);
                        if (i == 6) i++;
                    }
                }
                t10.Text = ZipCodeToString(reader.GetString(10));
                c6.SelectedIndex = reader.GetInt32(6) - 1;
            }
            con.Close();
        }

        // zapelnienie comboboxa z poziomami uprawnienia
        public static void FillRankBox(ComboBox cb)
        {
            cb.Items.Add("1 - użytkownik");
            cb.Items.Add("2 - agent");
            cb.Items.Add("3 - admin");
        }

        // czyszczenie textboxw i 1 combo dla admina
        public static void CleanTextBoxesUser(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
        TextBox t5, ComboBox c6, TextBox t7, TextBox t8, TextBox t9, TextBox t10, TextBox t11)
        {
            TextBox[] array = { t1, t2, t3, t4, t5, t7, t8, t8, t9, t10, t11 };
            foreach (TextBox tb in array)
            {
                tb.Text = "";
            }
            c6.SelectedIndex = -1;
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
        public static void LoadBuildings(ListBox cb, List<int> list, List<int> transaction)
        {
            cb.Items.Clear();
            list.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT b.id, b.size, b.basement, b.plot_of_land_size," +
                "\r\nCONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, CONCAT('/', b.no_apartament), ' '),' ', b.city),\r\n" +
                "b.zip_code, t.type_of, t.id, s.sell_price, r.rent_price, r.rent_min_time FROM projektbd.buildings b \r\n" +
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
                transaction.Add(reader.GetInt32(7));
                if (reader.IsDBNull(8) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena najmu: " + reader.GetInt32(9)
                    + " PLN min. okres najmu: " + reader.GetInt32(10) + " msc");
                }
                else if (reader.IsDBNull(9) == true || reader.IsDBNull(10) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena: " + reader.GetInt32(8) + " PLN");
                }
                else
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena najmu: " + reader.GetInt32(9)
                    + " PLN min. okres najmu: " + reader.GetInt32(10) + " msc. Kwota Zakupu: " + reader.GetString(8) + " PLN");

            }
            con.Close();
        }

        //metoda odblokowujaca textboxy w dodawaniu budynkow 
        // textBox -> sprzedaz
        // textBox2 i textBox3 -> wynajem 2(czas) 3(kwota)
        // index -> wybrana wartosc
        public static void UnlockTextBox(int index, TextBox textBox, TextBox textBox2, TextBox textBox3)
        {
            if (index == 0)
            {
                textBox2.Enabled = true;
                textBox3.Enabled = true;
            }
            else if (index == 1)
            {
                textBox.Enabled = true;
            }
            else if (index == 2)
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
                string query = "SELECT typeB, type_of FROM projektbd.type_buildings tp " +
                "LEFT JOIN projektbd.transaction_type t ON tp.id = t.id UNION SELECT " +
                "typeB, type_of FROM projektbd.type_buildings tp " +
                "RIGHT JOIN projektbd.transaction_type t ON tp.id = t.id";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();

                while (reader.Read())
                {
                    if (reader.IsDBNull(0) == false) cb1.Items.Add(reader.GetString(0));
                    if (reader.IsDBNull(1) == false) cb2.Items.Add(reader.GetString(1));
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
            LockBackTextBox(sell, rent, rentTime);
            if (index == 0)
            {
                rent.Enabled = true;
                rentTime.Enabled = true;
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
        public static string ZipCodeValidate(string zipcode)
        {
            try
            {
                if (Regex.IsMatch(zipcode, @"^\d{2}-\d{3}$"))
                {
                    zipcode = zipcode.Replace("-", "");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return zipcode;
        }

        //metoda do pobierania wartosci(kodu pocztowego) int z bazy danych i przerobienie na polski kod pocztowy
        public static string ZipCodeToString(string zipcode)
        {
            try
            {
                zipcode = zipcode.Insert(2, "-");

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            return zipcode;
        }

        // metoda zwracajace tablice z query aktualizujace dane dot sprzedazy/najmu do edycji budynkow
        public static string[] SelectQueryTransaction(int input, int output)
        {
            string[] qry = new string[2];
            if (input == 1 && output == 1)
            {
                qry[0] = "UPDATE projektbd.rent_details SET rent_price = @rent, rent_min_time = @time WHERE id_buildings = @bid ";
                return qry;
            }
            else if (input == 1 && output == 2)
            {
                qry[0] = "DELETE FROM projektbd.rent_details WHERE id_buildings = @bid";
                qry[1] = "INSERT INTO projektbd.sell_details (id_buildings, sell_price) VALUES (@bid, @price)";
                return qry;
            }
            else if (input == 1 && output == 3)
            {
                qry[0] = "UPDATE projektbd.rent_details SET id_buildings  = @bid, rent_price = @rent, rent_min_time = @time WHERE id_buildings = @bid ";
                qry[1] = "INSERT INTO projektbd.sell_details (id_buildings, sell_price) VALUES (@bid, @price)";
                return qry;
            }
            else if (input == 2 && output == 1)
            {
                qry[0] = "INSERT INTO projektbd.rent_details (id_buildings, rent_price, rent_min_time) VALUES (@bid, @rent, @time)";
                qry[1] = "DELETE FROM projektbd.sell_details WHERE id_buildings = @bid";
                return qry;
            }
            else if (input == 2 && output == 2)
            {
                qry[0] = "UPDATE projektbd.sell_details SET id_buildings = @bid, sell_price = @price WHERE id_buildings = @bid ";
                return qry;
            }
            else if (input == 2 && output == 3)
            {
                qry[0] = "UPDATE projektbd.sell_details SET sell_price = @price WHERE id_buildings = @bid ";
                qry[1] = "INSERT INTO projektbd.rent_details (id_buildings, rent_price, rent_min_time) VALUES (@bid, @rent, @time)";
                return qry;
            }
            else if (input == 3 && output == 1)
            {
                qry[0] = "UPDATE projektbd.rent_details SET rent_price = @rent, rent_min_time = @time WHERE id_buildings = @bid";
                qry[1] = "DELETE FROM projektbd.sell_details WHERE id_buildings = @bid";
                return qry;
            }
            else if (input == 3 && output == 2)
            {
                qry[0] = "DELETE FROM projektbd.rent_details WHERE id_buildings = @bid";
                qry[1] = "UPDATE projektbd.sell_details SET sell_price = @price WHERE id_buildings = @bid";
                return qry;
            }
            else if (input == 3 && output == 3)
            {
                qry[0] = "UPDATE projektbd.rent_details SET id_buildings  = @bid, rent_price = @rent, rent_min_time = @time";
                qry[1] = "UPDATE projektbd.sell_details SET sell_price = @price WHERE id_buildings = @bid ";
                return qry;
            }
            return qry;
        }

        // metoda walidujaca adres mailowy
        public static bool IsValidMail(string email)
        {
            var valid = true;

            try
            {
                var emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }

            return valid;
        }

        // metoda walidujaca text bez cyfr
        public static void allowLettersOnly(TextBox tb, KeyPressEventArgs e)
        {
            if (!char.IsLetter(e.KeyChar) && !char.IsControl(e.KeyChar) && !char.IsWhiteSpace(e.KeyChar))
                e.Handled = true;
        }

        // metoda walidujaca kod pczotowy
        public static void allowPostalOnly(TextBox tb, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '-')
                e.Handled = true;
        }

        // metoda walidujaca liczby dziesietne
        public static void allowDecimalOnly(TextBox tb, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar) && e.KeyChar != '.')
                e.Handled = true;
        }

        // metoda walidujaca liczby  całkowite
        public static void allowNumbersOnly(TextBox tb, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar))
                e.Handled = true;
        }

        // 
        // metody dla uzytkownika i agenta
        //

        //wczytywanie wartosci dla zalogowanego id uzytkownika do edycji
        public static void FillTextBoxesUser(TextBox t1, TextBox t2, TextBox t3, TextBox t4,
        TextBox t5, TextBox t6, TextBox t7, TextBox t8, TextBox t9, TextBox t10, string query)
        {
            TextBox[] array = { t1, t2, t3, t4, t5, t6, t7, t8, t9, t10 };
            int i = 0;

            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                foreach (TextBox tb in array)
                {
                    tb.Text = reader.GetString(i++);
                }
                t9.Text = ZipCodeToString(reader.GetString(8));
            }
            con.Close();
        }

        //metoda aktualizujaca edycje profilu
        public static void UpdateRowUser(TextBox first_name, TextBox last_name, TextBox login, TextBox password,
        TextBox email, TextBox street, TextBox no_building, TextBox no_apartament, TextBox zip_code, TextBox city)
        {
            if (Tools.IsValidMail(email.Text))
            {
                try
                {

                    int selectedId = User.LoggedUserId;
                    string zipCode = Tools.ZipCodeValidate(zip_code.Text);
                    int x = 1;
                    string query = "UPDATE projektbd.users SET first_name= '" + first_name.Text + "', last_name= '" + last_name.Text +
                    "', login= '" + login.Text + "', password= '" + password.Text + "', email= '" + email.Text + "',  street= '" + street.Text
                    + "', no_building= '" + no_building.Text + "', no_apartament= '" + no_apartament.Text +
                    "', zip_code=" + "'" + zipCode + "', city= '" + city.Text + "' WHERE id= '" + selectedId + "';";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteReader();
                    con.Close();
                    MessageBox.Show("Zaaktualizowano dane!", "Edycja Profilu", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("Wprowadz poprawnie dane", "Edycja Profilu", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //usuniecie konta dla usera i agenta
        public static void DeleteRow()
        {
            int selectedId = User.LoggedUserId;

            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz usunac wiersz o indeksie: " + selectedId + "?",
            "Usuwanie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (wynik == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM projektbd.users  WHERE id= '" + selectedId + "';";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteReader();
                    con.Close();
                    MessageBox.Show("Pomyślnie usunięto konto!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // metoda wczytujaca wszystkie budnyki do listboxa
        public static void LoadBuildingsUser(ListBox cb)
        {
            cb.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT b.id, b.size, b.basement, b.plot_of_land_size," +
                "\r\nCONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, CONCAT('/', b.no_apartament), ' '),' ', b.city),\r\n" +
                "b.zip_code, t.type_of, t.id, s.sell_price, r.rent_price, r.rent_min_time FROM projektbd.buildings b \r\n" +
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
                if (reader.IsDBNull(8) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena najmu: " + reader.GetInt32(9)
                    + " PLN min. okres najmu: " + reader.GetInt32(10) + " msc");
                }
                else if (reader.IsDBNull(9) == true || reader.IsDBNull(10) == true)
                {
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena: " + reader.GetInt32(8) + " PLN");
                }
                else
                    cb.Items.Add("Metraż: " + reader.GetInt32(1) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                    reader.GetDouble(3) + " Adres: " + reader.GetString(4) + " " + ZipCodeToString(reader.GetString(5)) + " cena najmu: " + reader.GetInt32(9)
                    + " PLN min. okres najmu: " + reader.GetInt32(10) + " msc. Cena: " + reader.GetString(8) + " PLN");

            }
            con.Close();
        }

        // wczytuje spotkania dla uzytkownika 
        public static void LoadAllMeetings(ListBox cb)
        {
            try
            {
                cb.Items.Clear();
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                string query = "SELECT st.status_of_meeting, u1.first_name, u1.last_name,\r\n" +
                    " m.date, m.buildings_id, b.size, tb.typeB,\r\n" +
                    "CONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament,\r\n " +
                    "CONCAT('/', b.no_apartament), ' '),' ', b.city, ' ', b.zip_code) as bAddr,\r\n" +
                    "IF(b.transaction_id = 2 OR 3, 'sprzedaz', ''),\r\n" +
                    "IF(b.transaction_id = 1 OR 3, 'najem', '') FROM projektbd.meetings m \r\n" +
                    "INNER JOIN projektbd.statusm st ON m.status_id = st.id \r\n" +
                    "INNER JOIN projektbd.users u ON  m.user_id = u.id\r\n " +
                    "INNER JOIN projektbd.users u1 ON  m.agent_id = u1.id \r\n" +
                    "INNER JOIN projektbd.buildings b ON m.buildings_id = b.id \r\n " +
                    "LEFT JOIN projektbd.sell_details s ON m.buildings_id = s.id_buildings \r\n " +
                    "LEFT JOIN projektbd.rent_details r ON m.buildings_id = r.id_buildings \r\n " +
                    "LEFT JOIN projektbd.type_buildings tb ON b.type_id = tb.id \r\n " +
                    "WHERE m.user_id = '" + User.LoggedUserId + "' ORDER BY m.date";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();
                while (reader.Read())
                {
                    cb.Items.Add("Typ: "+ reader.GetString(0) + ". Agent: " + reader.GetString(1) + " " + reader.GetString(2)
                    + " data: " + reader.GetDateTime(3) + " Nieruchomość:  " + reader.GetString(6) + " " + reader.GetString(5) + "m/2"
                    + " adres: " + reader.GetString(7) + "" );
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // wczytanie klientow dla agenta
        public static void LoadAddMeetingsPanelUsers(ComboBox cbU)
        {
            cbU.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT id_rank,id,first_name,last_name,email FROM projektbd.users WHERE id_rank = 1;";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                if (reader.GetInt32(0) == 1)
                    cbU.Items.Add("id: " + reader.GetInt32(1) + ". Imie i nazwisko:" +
                reader.GetString(2) + " " + reader.GetString(3) + " mail: " + reader.GetString(4));
            }
            con.Close();
        }

        // resetowanie parametrów
        public static void ResetParameters()
        {
            User.LoggedUserId = 0;
            User.Id = 0;
            User.FirstName = null;
            User.LastName = null;
            User.Login = null;
            User.Password = null;
            User.Email = null;
            User.Id_rank = 0;
            User.Street = null;
            User.No_Building = null;
            User.No_apartament = null;
            User.ZipCode = 0;
            User.City = null;
    }
    }
}
