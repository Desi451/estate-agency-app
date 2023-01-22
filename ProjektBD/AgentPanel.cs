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
        public AgentPanel()
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
    }
}
