using MySql.Data.MySqlClient;
using ProjektBD.sidebar_classes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjektBD
{
    public partial class AgentPanel : Form
    {
        List<int> idMeetings = new List<int>(); // tablica przechowujaca id spotkan dla edytowania
        List<int> idBuildings = new List<int>(); // tablica przechowujaca id budynkow dla dodawania i edytowania spotkan
        List<int> buildingTransactions = new List<int>();// tablica przechowujaca id transakcji dla edycji budynków

        public AgentPanel()
        {
            InitializeComponent();
            HidePannels();
            Tools.LoadBasementListBox(ListBoxBasementEdit);
            Tools.LoadBasementListBox(ListBoxBuildingBasement);
            panelBtn.Visible = true;
        }

        private void HidePannels()
        {
            foreach (Panel tb in this.Controls.OfType<Panel>())
            {
                tb.Visible = false;
            }
            panelBtn.Visible = true;
        }

        private void LoadProfile()
        {
            int selectedId = User.LoggedUserId;
            string query = "SELECT first_name, last_name,login,password,email,street,no_building" +
            ",no_apartament,zip_code,city FROM projektbd.users WHERE id= '" + selectedId + "';";

            Tools.FillTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
             streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox, query);
        }

        // edycja profilu
        private void editProfileBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editProfilePanel);
            LoadProfile();
        }

        private void confirmEditUserBtn_Click(object sender, EventArgs e)
        {
            Tools.UpdateRowUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
             streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox);
        }

        private void confirmDelUserBtn_Click(object sender, EventArgs e)
        {
            Tools.DeleteRow();
            this.Close();
        }

        // dodawanie spotkan
        private void addMeetingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(addMeetingPanel);
            Tools.LoadStatusofMeeting(listBoxMeetingsStatus);
            Tools.LoadAddMeetingsPanelUsers(listBoxMeetingsUser);
            Tools.LoadBuildings(listBoxBuildings, idBuildings, buildingTransactions);
            dateTimeAddMeeting.Value = DateTime.Now;
            dateTimeAddMeeting.MinDate = DateTime.Now;
        }

        private void ConfirmAddMeetingBtn_Click(object sender, EventArgs e)
        {
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz dodac to spotkanie" +
            "?", "Dodawanie Spotkań", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            int status = Tools.ReadMeetingId(listBoxMeetingsStatus.Text);
            int user = Tools.ReadComboId(listBoxMeetingsUser.Text);
            int agent = User.LoggedUserId;
            int building = idBuildings[listBoxBuildings.SelectedIndex];

            if (wynik == DialogResult.Yes && Tools.ValidateMeeting(building, status) == true)
            {
                try
                {
                    string query = "INSERT INTO projektbd.meetings(status_id,agent_id,user_id,date,buildings_id)" +
                    "VALUES('" + status + "', '" + User.LoggedUserId + "', '" + user + "', '" + dateTimeAddMeeting.Text + "', '" + building + "');";

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

        //edytowajnie spotkan
        private void LoadAgentMeetings() // wczytywanie spotkan agenta
        {
            try
            {
                editMeetingsList.Items.Clear();
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                string query = "SELECT m.id, st.status_of_meeting, m.agent_id, u1.first_name, u1.last_name,\r\n" +
                    "m.user_id, u.first_name, u.last_name, m.date, m.buildings_id, b.size,\r\n" +
                    "CONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, \r\n" +
                    "CONCAT('/', b.no_apartament), ' '),' ', b.city, ' ', b.zip_code) as bAddr,\r\n" +
                    "IF(b.transaction_id = 2 OR 3 ,CONCAT('cena: ', s.sell_price),'') as sprzedaz,\r\n" +
                    "IF(b.transaction_id = 1 OR 3,CONCAT('cena najmu: ', r.rent_price,' min okres:', r.rent_min_time, ' msc'),'') as wynajem\r\n " +
                    "FROM projektbd.meetings m \r\n" +
                    "INNER JOIN projektbd.statusm st ON m.status_id = st.id \r\n" +
                    "INNER JOIN projektbd.users u ON  m.user_id = u.id\r\n " +
                    "INNER JOIN projektbd.users u1 ON  m.agent_id = u1.id\r\n " +
                    "INNER JOIN projektbd.buildings b ON m.buildings_id = b.id " +
                    "LEFT JOIN projektbd.sell_details s ON m.buildings_id = s.id_buildings " +
                    "LEFT JOIN projektbd.rent_details r ON m.buildings_id = r.id_buildings " +
                    "WHERE m.agent_id = '" + User.LoggedUserId + "';";
                MySqlCommand sda = new MySqlCommand(query, con);
                MySqlDataReader reader = sda.ExecuteReader();
                while (reader.Read())
                {
                    idMeetings.Add(reader.GetInt32(0));
                    editMeetingsList.Items.Add("id: " + reader.GetInt32(0)
                    + ".  Klient: " + reader.GetString(6) + " " + reader.GetString(7) + " data: " + reader.GetDateTime(8));
                }
                con.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
        private void editMeetingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editMeetingPanel);
            LoadAgentMeetings();
            Tools.LoadStatusofMeeting(listBoxEditMeetingsStatus);
            Tools.LoadBuildings(listBoxBuildingsEditMeetings, idBuildings, buildingTransactions);
            Tools.LoadAddMeetingsPanelUsers(listBoxEditMeetingsUser);

        }

        private void editMeetingsList_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;

                string query = "SELECT * FROM projektbd.meetings WHERE id = '" + Tools.ReadComboId(editMeetingsList.Text) + "'";
                MySqlCommand sda = new MySqlCommand(query, con);
                con.Open();
                MySqlDataReader reader = sda.ExecuteReader();
                while (reader.Read())
                {
                    listBoxEditMeetingsStatus.SelectedIndex = reader.GetInt32(1) - 1;
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

        private void ConfirmEditMeetingsBtn_Click(object sender, EventArgs e)
        {
            int status = (listBoxEditMeetingsStatus.SelectedIndex + 1);
            int user = Tools.ReadComboId(listBoxEditMeetingsUser.Text);
            int building = idBuildings[listBoxBuildingsEditMeetings.SelectedIndex];
            string date = dateTimeEditMeeting.Text;
            int id = Tools.ReadComboId(editMeetingsList.Text);
            if (Tools.ValidateMeeting(building, status) == true)
            {
                try
                {
                    string query = "UPDATE projektbd.meetings SET  status_id ='" + status +
                    "', agent_id='" + User.LoggedUserId + "', user_id='" + user
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


        // dodawanie budnykow
        private void addBuildingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(addBuildingPanel);
            Tools.LockBackTextBox(TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
            Tools.LoadBuildingElements(ListBoxBuildingType, ListBoxBuildingTransaction);

        }

        private void ListBoxBuildingTransaction_SelectedIndexChanged(object sender, EventArgs e)
        {
            Tools.LockBackTextBox(TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
            Tools.UnlockTextBox(ListBoxBuildingTransaction.SelectedIndex,
            TextBoxBuildingsSellPrice, TextBoxBuildingsTimeRent, TextBoxBuildingsRentPrice);
        }

        private void ConfirmAddBuildingBtn_Click(object sender, EventArgs e)
        {
            int addedId = 0;
            int type = (ListBoxBuildingType.SelectedIndex + 1);
            string size = TextBoxBuildingSize.Text;
            int basement = ListBoxBuildingBasement.SelectedIndex;
            string landSize = TextBoxBuildingSizeLand.Text;
            string street = TextBoxBuildingStreet.Text;
            string noBuilding = TextBoxBuildingNo_building.Text;
            string noApartament = TextBoxBuildingNo_apartament.Text;
            string zip = Tools.ZipCodeValidate(TextBoxBuildingZipCode.Text);
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

        // edytownaie budynkow
        private void editBuildingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editBuildingPanel);
            Tools.LoadBuildings(LbSelectBuildingEdit, idBuildings, buildingTransactions);
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
                    "t.id, s.sell_price, r.rent_price, r.rent_min_time, type_id FROM projektbd.buildings b\r\n" +
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
                    if (reader.IsDBNull(5) == false) TextBoxEditBuildingNoA.Text = reader.GetString(5);
                    TextBoxEditBuildingCity.Text = reader.GetString(6);
                    TextBoxEditBuildingZipCode.Text = Tools.ZipCodeToString(reader.GetString(7));
                    ListBoxTypeTransactionEdit.SelectedIndex = reader.GetInt32(8) - 1;
                    if (reader.IsDBNull(9) == false) TextBoxSellEdit.Text = reader.GetInt32(9).ToString();
                    if (reader.IsDBNull(10) == false) TextBoxRentEdit.Text = reader.GetInt32(10).ToString();
                    if (reader.IsDBNull(11) == false) TextBoxRentTimeEdit.Text = reader.GetInt32(11).ToString();
                    ListBoxTypeBuildingEdit.SelectedIndex = reader.GetInt32(12) - 1;
                }
                con.Close();
                Tools.LoadTransactionBuildings(ListBoxTypeTransactionEdit.SelectedIndex, TextBoxSellEdit, TextBoxRentEdit, TextBoxRentTimeEdit);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        //logout
        private void LogOutBtn_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
