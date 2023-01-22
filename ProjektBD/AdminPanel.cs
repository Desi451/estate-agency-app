using Google.Protobuf.WellKnownTypes;
using MySql.Data.MySqlClient;
using ProjektBD.sidebar_classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjektBD
{
    public partial class AdminPanel : Form
    {
        // zmienne
        List<int> idBuildings= new List<int>(); // tablica przechowujaca id budynkow dla dodawania i edytowania spotkan
        List<int> idMeetings = new List<int>(); // tablica przechowujaca id spotkan dla edytowania

        public AdminPanel()
        {
            InitializeComponent();
            HidePannels();
            Tools.LoadBasementListBox(ListBoxBasementEdit);
            Tools.LoadBasementListBox(ListBoxBuildingBasement);
        }

        // metody ogolne
        private void HidePannels()
        {
            foreach (Panel tb in this.Controls.OfType<Panel>())
            {
                tb.Visible = false;
            }
        }

        // metody zwiazane z edycja i usuwaniem profilu
        private void UpdateRow()
        {
            try
            {
                int selectedId = Tools.ReadComboId(userListBox.Text);
                int x = 1;
                string query = "UPDATE projektbd.users SET first_name= '" + first_nameBox.Text + "', last_name= '" + last_nameBox.Text +
                "', login= '" + loginBox.Text + "', password= '" + passwordBox.Text + "', email= '" + emailBox.Text + "', id_rank= '" + x + "'" +
                ", street= '" + streetBox.Text + "', no_building= '" + no_buildingBox.Text + "', no_apartament= '" + no_apartamentBox.Text + 
                "', zip_code=" + "'" + int.Parse(zip_codeBox.Text) + "', city= '" + cityBox.Text + "' WHERE id= '" + selectedId + "';";

                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteReader();
                con.Close();
                MessageBox.Show("Zaaktualizowano dane!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DeleteRow()
        {
            int selectedId = Tools.ReadComboId(userListBox.Text);

            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz usunac wiersz o indeksie: " + selectedId + "?", 
            "Usuwanie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if(wynik == DialogResult.Yes)
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
                    MessageBox.Show("Pomyślnie usunięto użytkownika!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void LoadEditPanel()
        {
            userListBox.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT id, first_name, last_name, login FROM projektbd.users";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                userListBox.Items.Add("id: " + reader.GetInt32(0) + ". Imie i nazwisko:" + reader.GetString(1) + " " + reader.GetString(2) + " login: " + reader.GetString(3));
            }
            con.Close();
        } // metoda odpowiadajaca tylko za wczytanie comboBoxa

        private void EditProfileBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editProfilePanel);
            LoadEditPanel();
        }

        private void ConfirmEditUserBtn_Click(object sender, EventArgs e)
        {
            UpdateRow();
        }

        private void ConfirmDelUserBtn_Click(object sender, EventArgs e)
        {
            DeleteRow();
            Tools.CleanTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
            rankBox, streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox);
            LoadEditPanel();
        }

        private void UserListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Tools.ReadComboId(userListBox.Text);
            string query = "SELECT * FROM projektbd.users WHERE id= '" + selectedId + "';";

            Tools.FillTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
            rankBox, streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox, query);
        }

        // metody zwiazane z dodawaniem spotkania jako administratorem
        private void AddMeetingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(addMeetingPanel);
            Tools.LoadStatusofMeeting(listBoxMeetingsStatus);
            Tools.LoadAddMeetingsPanelAgentsAndUsers(listBoxMeetingsAgent, listBoxMeetingsUser);
            Tools.LoadBuildings(listBoxBuildings,idBuildings);
            dateTimeAddMeeting.Value = DateTime.Now;
            dateTimeAddMeeting.MinDate = DateTime.Now;
        }

        private void ConfirmAddMeetingBtn_Click(object sender, EventArgs e)
        {
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz dodac to spotkanie" + 
            "?", "Dodawanie Spotkań", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            int status = Tools.ReadMeetingId(listBoxMeetingsStatus.Text);
            int user = Tools.ReadComboId(listBoxMeetingsUser.Text);
            int agent = Tools.ReadComboId(listBoxMeetingsAgent.Text);
            int building = idBuildings[listBoxBuildings.SelectedIndex];

            if (wynik == DialogResult.Yes && Tools.ValidateMeeting(building,status) == true)
            {
                try
                {
                    string query = "INSERT INTO projektbd.meetings(status_id,agent_id,user_id,date,buildings_id)" +
                    "VALUES('" + status + "', '" + agent + "', '" + user + "', '" + dateTimeAddMeeting.Text + "', '" + building + "');";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteReader();
                    con.Close();
                    MessageBox.Show("Pomyślnie dodano spotkanie!", "Dodawanie Spotkań", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        // metody zwiazane z edytowaniem/usuwaniem spotkania jako administrator
        private void LoadAddMeetingsPanelListBox()
        {
            try
            { 
                editMeetingsList.Items.Clear();
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                string query = "SELECT m.id, st.status_of_meeting, m.agent_id, u1.first_name, u1.last_name," +
                    "m.user_id, u.first_name, u.last_name, m.date, m.buildings_id, b.size," +
                    "CONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, " +
                    "CONCAT('/', b.no_apartament), ' '),' ', b.city, ' ', b.zip_code) as bAddr," +
                    "IF(b.transaction_id = 2 OR 3 ,CONCAT('cena: ', s.sell_price),'') as sprzedaz," +
                    "IF(b.transaction_id = 1 OR 3,CONCAT('cena najmu: ', r.rent_price,' min okres:', r.rent_min_time, ' msc'),'') as wynajem " +
                    "FROM projektbd.meetings m " +
                    "INNER JOIN projektbd.statusm st ON m.status_id = st.id " +
                    "INNER JOIN projektbd.users u ON  m.user_id = u.id " +
                    "INNER JOIN projektbd.users u1 ON  m.agent_id = u1.id " +
                    "INNER JOIN projektbd.buildings b ON m.buildings_id = b.id " +
                    "LEFT JOIN projektbd.sell_details s ON m.buildings_id = s.id_buildings " +
                    "LEFT JOIN projektbd.rent_details r ON m.buildings_id = r.id_buildings ";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();
                while (reader.Read())
                {
                    idMeetings.Add(reader.GetInt32(0));
                    editMeetingsList.Items.Add("id: " + reader.GetInt32(0) + ". Agent: " + reader.GetString(3) + " " + reader.GetString(4)
                    + " Klient: " + reader.GetString(6) + " " + reader.GetString(7) + " data: " + reader.GetDateTime(8));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void EditMeetingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editMeetingPanel);
            LoadAddMeetingsPanelListBox();
            Tools.LoadStatusofMeeting(listBoxEditMeetingsStatus);
            Tools.LoadAddMeetingsPanelAgentsAndUsers(listBoxEditMeetingsAgent, listBoxEditMeetingsUser);
            Tools.LoadBuildings(listBoxBuildingsEditMeetings,idBuildings);
            dateTimeEditMeeting.Value = DateTime.Now;
            dateTimeEditMeeting.MinDate= DateTime.Now;
        }

        private void ConfirmEditMeetingsBtn_Click(object sender, EventArgs e)
        {
            int status = (listBoxEditMeetingsStatus.SelectedIndex + 1);
            int agent = Tools.ReadComboId(listBoxEditMeetingsAgent.Text);
            int user = Tools.ReadComboId(listBoxEditMeetingsUser.Text);
            int building = idBuildings[listBoxBuildingsEditMeetings.SelectedIndex];
            string date= dateTimeEditMeeting.Text;
            int id = Tools.ReadComboId(editMeetingsList.Text);
            if(Tools.ValidateMeeting(building, status) == true)
            {
                try
                {
                    string query = "UPDATE projektbd.meetings SET  status_id ='" + status +
                    "', agent_id='" + agent + "', user_id='" + user
                     + "', date='" + date + "', buildings_id='" + building
                     + "' WHERE id ='" + id + "';";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteReader();
                    con.Close();
                    MessageBox.Show("Pomyślnie dokonano edycji!", "edycja", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ConfirmDelMeetingsBtn_Click(object sender, EventArgs e)
        {
           int temp = idMeetings[editMeetingsList.SelectedIndex];
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz usunac wiersz o indeksie: " + temp + "?", "Usuwanie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (wynik == DialogResult.Yes)
            {
                try
                {
                    string query = "DELETE FROM projektbd.meetings WHERE id= '" + temp + "';";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    cmd.ExecuteReader();
                    con.Close();
                    MessageBox.Show("Pomyślnie usunięto spotkanie!", "Usuwanie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void EditMeetingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                string query = "SELECT * FROM projektbd.meetings WHERE id = '" + Tools.ReadComboId(editMeetingsList.Text) + "'";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();
                while (reader.Read())
                {
                    listBoxEditMeetingsStatus.SelectedIndex = reader.GetInt32(1) - 1;
                    for (int i = 0; i < listBoxEditMeetingsAgent.Items.Count; i++)
                    {
                        if (Tools.ReadComboId(listBoxEditMeetingsAgent.Items[i].ToString()) == reader.GetInt32(2))
                        {
                            listBoxEditMeetingsAgent.SelectedIndex = i;
                        }
                    }
                    for (int i = 0; i < listBoxEditMeetingsUser.Items.Count; i++)
                    {
                        if (Tools.ReadComboId(listBoxEditMeetingsUser.Items[i].ToString()) == reader.GetInt32(3))
                        {
                            listBoxEditMeetingsUser.SelectedIndex = i;
                        }
                    }
                    dateTimeEditMeeting.Text = reader.GetString(4);

                    for (int i = 0; i < idBuildings.Count; i++)
                    {
                        if (idBuildings[i] == reader.GetInt32(5))
                        {
                            listBoxBuildingsEditMeetings.SelectedIndex = i;
                        }
                    }
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // metody zwiazane z dodawaniem budynku jako administrator
        private void AddBuildingBtn_Click(object sender, EventArgs e)
        {
            Tools.LockBackTextBox(TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
            HidePannels();
            Tools.SwitchVisibile(TextBoxBuildingsRentTime);
            Tools.LoadBuildingElements(ListBoxBuildingType, ListBoxBuildingTransaction);
        }

        private void ConfirmAddBuildingBtn_Click(object sender, EventArgs e)
        {
            int addedId=0;
            int type = (ListBoxBuildingType.SelectedIndex + 1);
            string size = TextBoxBuildingSize.Text;
            int basement = ListBoxBuildingBasement.SelectedIndex;
            string landSize = TextBoxBuildingSizeLand.Text;
            string street = TextBoxBuildingStreet.Text;
            string noBuilding = TextBoxBuildingNo_building.Text;
            string noApartament = TextBoxBuildingNo_apartament.Text;
            int zip = int.Parse(TextBoxBuildingZipCode.Text);
            string city = TextBoxBuildingCity.Text;
            int tran = (ListBoxBuildingTransaction.SelectedIndex + 1);
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz dodac to spotkanie" +
            "?", "Dodawanie Spotkań", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            if (wynik == DialogResult.Yes)
            {
                try
                {
                    string query = "INSERT INTO projektbd.buildings (type_id,size,basement,plot_of_land_size,street,no_building, " +
                   "no_apartament,zip_code,city,transaction_id) VALUES('" + type + "', '" + size + "', '"
                   + basement + "', '" + landSize + "', '" + street + "','" + noBuilding + "','"
                   + noApartament + "','" + zip + "','" + city + "','" + tran + "' );" +
                   "SELECT LAST_INSERT_ID();";

                    MySqlConnection con = new MySqlConnection();
                    con.ConnectionString = DataBase.Connstring;
                    con.Open();
                    MySqlCommand cmd = new MySqlCommand(query, con);
                    MySqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        addedId = reader.GetInt32(0);
                    }
                    con.Close();

                    string price = Tools.CheckIfNull(TextBoxBuildingsSellPrice.Text);
                    string rentPrice = Tools.CheckIfNull(TextBoxBuildingsRentPrice.Text);
                    string rentTime = Tools.CheckIfNull(TextBoxBuildingsTimeRent.Text);

                    string query1 = Tools.SelectTransaction(ListBoxBuildingTransaction.SelectedIndex, Int32.Parse(price),
                    Int32.Parse(rentPrice), Int32.Parse(rentTime), addedId);
                    con.Open();
                    MySqlCommand cmda = new MySqlCommand(query1, con);
                    cmda.ExecuteReader();

                    MessageBox.Show("Pomyślnie dodano budynek!", "Dodawanie budynkow", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    con.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        private void ListBoxBuildingTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools.LockBackTextBox(TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
            Tools.UnlockTextBox(ListBoxBuildingTransaction.SelectedIndex,
            TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
        }

        // metody zwiazane z edycja i usunieciem budynku
        private void EditBuildingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editBuildingPanel);
            Tools.LoadBuildings(LbSelectBuildingEdit, idBuildings);
            Tools.LoadBuildingElements(ListBoxTypeBuildingEdit, ListBoxTypeTransactionEdit);
            Tools.LockBackTextBox(TextBoxSellEdit, TextBoxRentTimeEdit, TextBoxRentEdit);
        }

        private void LbSelectBuildingEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            try 
            { 
                TextBoxSellEdit.Clear(); 
                TextBoxRentTimeEdit.Clear();
                TextBoxRentEdit.Clear();
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
            
                string query = "SELECT size, basement, plot_of_land_size,\r\n" +
                    "street, no_building, IF(no_apartament, no_apartament, ''), city, zip_code,\r\n" +
                    "t.id, s.sell_price, r.rent_price, r.rent_min_time FROM projektbd.buildings b\r\n" +
                    "INNER JOIN projektbd.transaction_type AS t ON b.transaction_id = t.id\r\n" +
                    "LEFT JOIN projektbd.sell_details s ON b.id = s.id_buildings\r\n" +
                    "LEFT JOIN projektbd.rent_details r ON b.id = r.id_buildings\r\n" +
                    "WHERE b.id = '" + idBuildings[LbSelectBuildingEdit.SelectedIndex] + "' ORDER BY b.id;";

                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();

                while (reader.Read())
                {
                    TBeditBuildigsSize.Text = reader.GetDecimal(0).ToString();
                    ListBoxBasementEdit.SelectedIndex = reader.GetInt32(1);
                    TBeditBuildigsLandSize.Text = reader.GetDecimal(2).ToString();
                    TextBoxEditBuildingStreet.Text = reader.GetString(3);
                    TextBoxEditBuildingNoB.Text = reader.GetString(4);
                    if(reader.IsDBNull(5)== false) TextBoxEditBuildingNoA.Text = reader.GetString(5);
                    TextBoxEditBuildingCity.Text = reader.GetString(6);
                    TextBoxEditBuildingZipCode.Text = reader.GetString(7);
                    ListBoxTypeTransactionEdit.SelectedIndex = reader.GetInt32(8)-1;
                    if (reader.IsDBNull(9) == false) TextBoxSellEdit.Text = reader.GetInt32(9).ToString();
                    if (reader.IsDBNull(10) == false) TextBoxRentEdit.Text = reader.GetInt32(10).ToString();
                    if (reader.IsDBNull(11) == false) TextBoxRentTimeEdit.Text = reader.GetInt32(11).ToString();
                }
                con.Close();
                Tools.LoadTransactionBuildings(ListBoxTypeTransactionEdit.SelectedIndex, TextBoxSellEdit, TextBoxRentEdit, TextBoxRentTimeEdit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ConfirmEditBuildingBtn_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            using (MySqlTransaction trans = con.BeginTransaction())
            {
                try
                {
                    using (MySqlCommand cmd = new MySqlCommand())
                    {
                        cmd.Connection = con;
                        cmd.Transaction = trans;

                        int buildingId = idBuildings[LbSelectBuildingEdit.SelectedIndex];
                        int type = (ListBoxTypeBuildingEdit.SelectedIndex + 1);
                        string size = TBeditBuildigsSize.Text;
                        int basement = ListBoxBasementEdit.SelectedIndex;
                        string landSize = TBeditBuildigsLandSize.Text;
                        string street = TextBoxEditBuildingStreet.Text;
                        string noBuilding = TextBoxEditBuildingNoB.Text;
                        string noApartament = TextBoxEditBuildingNoA.Text;
                        int zip = int.Parse(TextBoxEditBuildingZipCode.Text);
                        string city = TextBoxEditBuildingCity.Text;
                        int tran = (ListBoxTypeTransactionEdit.SelectedIndex + 1);

                        cmd.CommandText = "INSERT INTO projektbd.buildings (type_id, size, basement, plot_of_land_size," +
                            "street, no_building, no_apartament, zip_code, city, transaction_id) " +
                            "VALUES (@type, @size, @basement,@plot_of_land_size, @street, @no_building, " +
                            "@no_apartament, @zip_code, @city, @transaction_id)";
                        cmd.Parameters.AddWithValue("@type", type);
                        cmd.Parameters.AddWithValue("@size", size);
                        cmd.Parameters.AddWithValue("@basement", basement);
                        cmd.Parameters.AddWithValue("@plot_of_land_size", landSize);
                        cmd.Parameters.AddWithValue("@street", street);
                        cmd.Parameters.AddWithValue("@no_building", noBuilding);
                        cmd.Parameters.AddWithValue("@no_apartament", noApartament);
                        cmd.Parameters.AddWithValue("@zip_code", zip);
                        cmd.Parameters.AddWithValue("@city", city);
                        cmd.Parameters.AddWithValue("@transaction_id", tran);
                        cmd.ExecuteNonQuery();

                        string price = Tools.CheckIfNull(TextBoxSellEdit.Text);
                        string rentPrice = Tools.CheckIfNull(TextBoxBuildingsRentPrice.Text);
                        string rentTime = Tools.CheckIfNull(TextBoxRentEdit.Text);

                        if (ListBoxTypeTransactionEdit.SelectedIndex == 0 || ListBoxTypeTransactionEdit.SelectedIndex == 2)
                        {
                            cmd.CommandText = "UPDATE rent_details SET id_buildings, rent_price, rent_min_time VALUES (@bid, @rent, @time)";
                            cmd.Parameters.AddWithValue("@bid", buildingId);
                            cmd.Parameters.AddWithValue("@rent", rentPrice);
                            cmd.Parameters.AddWithValue("@time", rentTime);
                            cmd.ExecuteNonQuery();
                        }

                        if (ListBoxTypeTransactionEdit.SelectedIndex == 1 || ListBoxTypeTransactionEdit.SelectedIndex == 2)
                        {
                            cmd.CommandText = "UPDATE sell_details SET (id_buildings, sell_price) VALUES (@col1, @col2)";
                            cmd.Parameters.AddWithValue("@col1", buildingId);
                            cmd.Parameters.AddWithValue("@col2", price);
                            cmd.ExecuteNonQuery();
                        }
                        trans.Commit();
                    }
                }
                catch (MySqlException)
                {
                    trans.Rollback();
                    throw;
                }
            }
            con.Close();
        }

        private void ConfrimDelBuildingBtn_Click(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();


                string query = "DELETE b,s,r FROM projektbd.buildings AS b " +
                    "LEFT JOIN projektbd.rent_details AS r ON b.id = r.id_buildings " +
                    "LEFT JOIN projektbd.sell_details AS s ON b.id = s.id_buildings " +
                    "WHERE b.id = '"+ idBuildings[LbSelectBuildingEdit.SelectedIndex] +"';";

                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteReader();
                con.Close();
                MessageBox.Show("Pomyślnie usunięto budynek!", "Budynki", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // metoda odpowiedzialna za wylogowanie
        private void LogOutBtn_Click(object sender, EventArgs e)
        {

        }

        private void ListBoxTypeTransactionEdit_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools.LoadTransactionBuildings(ListBoxTypeTransactionEdit.SelectedIndex,
            TextBoxSellEdit, TextBoxRentEdit, TextBoxRentTimeEdit);
        }
    }
}
