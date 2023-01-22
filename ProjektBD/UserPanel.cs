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
    public partial class UserPanel : Form
    {
        public UserPanel()
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
            panelBtn.Visible = true;
        }

        // panel edycji profilu
        private void editProfileBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(editProfilePanel);
            LoadProfile();
        } //guzik inicjujacy

        private void LoadProfile()
        {
            int selectedId = User.LoggedUserId;
            string query = "SELECT first_name, last_name,login,password,email,street,no_building" +
            ",no_apartament,zip_code,city FROM projektbd.users WHERE id= '" + selectedId + "';";

            Tools.FillTextBoxesUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
             streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox, query);
        }

        private void confirmDelUserBtn_Click(object sender, EventArgs e)
        {
            Tools.DeleteRow();
            this.Close();
        }

        private void confirmEditUserBtn_Click(object sender, EventArgs e)
        {
            Tools.UpdateRowUser(first_nameBox, last_nameBox, loginBox, passwordBox, emailBox,
             streetBox, no_buildingBox, no_apartamentBox, zip_codeBox, cityBox);
        }

        // panel spotkań
        private void MeetingBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(MeetingPanel);
            
            Tools.LoadAllMeetings(listBoxMeetings);
        } //guzik inicjujacy

        private void BuildingsBtn_Click(object sender, EventArgs e)
        {
            HidePannels();
            Tools.SwitchVisibile(BuildingsPanel);
            Tools.LoadBuildingsUser(ListBoxBuilding);
        }

        // guzik wylogowania
        private void UserLogout_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void first_nameBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Tools.allowLettersOnly(first_nameBox, e);
        }

        private void last_nameBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Tools.allowLettersOnly(last_nameBox, e);
        }

        private void no_apartamentBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Tools.allowNumbersOnly(no_apartamentBox, e);
        }

        private void zip_codeBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Tools.allowPostalOnly(zip_codeBox, e);
        }

        private void cityBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            Tools.allowLettersOnly(cityBox, e);
        }

        // walidacja edycji profilu
    }
}
