using MySql.Data.MySqlClient;
using ProjektBD.sidebar_classes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;

namespace ProjektBD
{
    public partial class AdminPanel : Form
    {
        public AdminPanel()
        {
            InitializeComponent();
            HidePannels();
        }

        // metody ogolne

        private void HidePannels()
        {
            foreach (Panel tb in this.Controls.OfType<Panel>())
            {
                tb.Visible = false;
            }
        }

        private void SwitchVisibile(Panel tb)
        {
            HidePannels();
            if (tb.Visible == true)
            {
                tb.Visible = false;
            }
            else
                tb.Visible = true;
                tb.BringToFront();
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
                ", street= '" + streetBox.Text + "', no_building= '" + no_buildingBox.Text + "', no_apartament= '" + no_apartamentBox.Text + "', zip_code=" +
                "'" + int.Parse(zip_codeBox.Text) + "', city= '" + cityBox.Text + "' WHERE id= '" + selectedId + "';";

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
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz usunac wiersz o indeksie: " + selectedId + "?", "Usuwanie", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
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
            SwitchVisibile(editProfilePanel);
            LoadEditPanel();
        }

        private void ConfirmEditUserBtn_Click(object sender, EventArgs e)
        {
            UpdateRow();
        }

        private void confirmDelUserBtn_Click(object sender, EventArgs e)
        {
            DeleteRow();
            Tools.CleanTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
            rankBox, streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox);
            LoadEditPanel();
        }

        private void userListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Tools.ReadComboId(userListBox.Text);
            string query = "SELECT * FROM projektbd.users WHERE id= '" + selectedId + "';";

            Tools.FillTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
            rankBox, streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox, query);
        }

        // metody zwiazane z dodawaniem spotkania jako administrator
        private void LoadAddMeetingsPanelAgentsAndUsers()
        {
            listBoxMeetingsAgent.Items.Clear();
            listBoxMeetingsUser.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT id_rank,id,first_name,last_name,email FROM projektbd.users WHERE id_rank IN (2,1);";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                if(reader.GetInt32(0)==2)
                listBoxMeetingsAgent.Items.Add("id: " + reader.GetInt32(1) + ". Imie i nazwisko:" + 
                reader.GetString(2) + " " + reader.GetString(3) + " mail: " + reader.GetString(4));

                if (reader.GetInt32(0) == 1)
                    listBoxMeetingsUser.Items.Add("id: " + reader.GetInt32(1) + ". Imie i nazwisko:" +
                reader.GetString(2) + " " + reader.GetString(3) + " mail: " + reader.GetString(4));
            }
            con.Close();
        }

        private void LoadStatusofMeeting()
        {
            listBoxMeetingsStatus.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT * from status";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                listBoxMeetingsStatus.Items.Add(reader.GetInt32(0) + ". " + reader.GetString(1));
            }
            con.Close();
        }

        private void LoadBuildings()
        {
            listBoxBuildings.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT b.size, b.basement, b.plot_of_land_size," +
                "\r\nCONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, CONCAT('/', b.no_apartament), ''),' ', b.city, ' ', b.zip_code ),\r\n" +
                "t.type_of, s.sell_price, r.rent_price, r.rent_min_time\r\nFROM projektbd.buildings b \r\n" +
                "INNER JOIN projektbd.transaction_type t ON b.transaction_id = t.id\r\n" +
                "LEFT JOIN projektbd.sell_details s ON b.id = s.id_buildings\r\n" +
                "LEFT JOIN projektbd.rent_details r ON b.id = r.id_buildings;";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();

            while (reader.Read())
            {
                string piwnica;

                if (reader.GetInt32(1) == 1) piwnica = "Tak";
                else piwnica = "Nie";

                listBoxBuildings.Items.Add("Metraż: " + reader.GetInt32(0) + "m^2. piwnica: " + piwnica + " Działka: " + "\n" +
                reader.GetDouble(2) + " Adres: " + reader.GetString(3) + " cena: " + reader.GetInt32(5) + " PLN");
            }
            con.Close();
        }

        private void addMeetingBtn_Click(object sender, EventArgs e)
        {
            SwitchVisibile(addMeetingPanel);
            LoadAddMeetingsPanelAgentsAndUsers();
            LoadStatusofMeeting();
            LoadBuildings();
        }

        private void ConfirmAddMeetingBtn_Click(object sender, EventArgs e)
        {
            DialogResult wynik = MessageBox.Show("Czy na pewno chcesz dodac to spotkanie" + 
            "?", "Dodawanie Spotkań", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

            int status = Tools.ReadComboId(listBoxMeetingsStatus.Text);
            int user = Tools.ReadComboId(listBoxMeetingsUser.Text);
            int agent = Tools.ReadComboId(listBoxMeetingsAgent.Text);

            if (wynik == DialogResult.Yes)
            {
                try
                {


                    string query = "INSERT INTO projektbd.meetings(status_id,agent_id,user_id,date,buildings_id " +
                    "VALUES('" + status + "'" + agent + "'" + user + "' NOW() '" + 1 + "');";

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
            //listBoxMeetings.Items.Clear();
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            string query = "SELECT m.id, m.status_id, m.date, b.id as b_id, b.size, " +
                "\r\nCONCAT('ul. ', b.street, ' ', b.no_building, IF(b.no_apartament, CONCAT('/', b.no_apartament), '')),\r\n " +
                "s.sell_price,u.id as u_id, u.first_name, u.last_name, u.email\r\nFROM projektbd.meetings m " +
                "INNER JOIN projektbd.buildings b \r\nON m.buildings_id = b.id " +
                "INNER JOIN projektbd.sell_details s\r\nON m.buildings_id = s.id_buildings " +
                "INNER JOIN projektbd.users u \r\nON  m.user_id = u.id;";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                //listBoxMeetings.Items.Add("id: " + reader.GetInt32(0) + ". Imie i nazwisko:" + reader.GetString(8) + " " + reader.GetString(9));
            }
            con.Close();
        }
    }
}

