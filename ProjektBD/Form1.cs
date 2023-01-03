using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace ProjektBD
{
    public partial class LoginPanel : Form
    {
        string connstring = "server=localhost;uid=root;pwd=Kutas123;database=projektbd";

        public LoginPanel()
        {
            InitializeComponent();
        }

        private void LoginPanel_Load(object sender, EventArgs e)
        {
            

        }

        private void button1_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = connstring;
            con.Open();
            string sql = "select city FROM projektbd.users";
            MySqlCommand cmd = new MySqlCommand(sql, con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                MessageBox.Show("Miasto" + reader);
            }

        }
    }
}
