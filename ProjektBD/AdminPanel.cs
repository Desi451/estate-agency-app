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
    public partial class AdminPanel : Form
    {
        public AdminPanel()
        {
            InitializeComponent();
            HidePannels();
        }

        private void HidePannels()
        {
            foreach (Panel tb in this.Controls.OfType<Panel>())
            {
                tb.Visible = false;
            }
        }

        private void SwitchVisibile(Panel tb)
        {
            if(tb.Visible == true)
            {
                tb.Visible = false;
            }
            else
                tb.Visible = true;
        }

        private void updateRow()
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

        private void editProfileBtn_Click(object sender, EventArgs e)
        {
            userListBox.Items.Clear();
            SwitchVisibile(editProfilePanel);
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();

            string query = "SELECT id, first_name, last_name, login FROM projektbd.users";

            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                userListBox.Items.Add("id: " + reader.GetInt32(0) + ". Imie i nazwisko:" + reader.GetString(1) + " " +reader.GetString(2) + " login: " + reader.GetString(3));
            }
            con.Close();
        }

        private void confirmEditUserBtn_Click(object sender, EventArgs e)
        {
            updateRow();
        }

        private void confirmDelUserBtn_Click(object sender, EventArgs e)
        {

        }

        private void userListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedId = Tools.ReadComboId(userListBox.Text);
            string query = "SELECT * FROM projektbd.users WHERE id= '" + selectedId + "';";

            Tools.FillTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
                rankBox, streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox, query);
        }
    }
}

