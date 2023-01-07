using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ProjektBD
{
    public partial class LoginPanel : Form
    {
        
        string login, passwd;

        public LoginPanel()
        {
            InitializeComponent();
        }

        private void registerBtn_Click(object sender, EventArgs e)
        {
            RegisterPanel registration = new RegisterPanel();
            registration.Show();
        }

        private void loginBtn_Click(object sender, EventArgs e)
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            
            //komenda do bazy
            string query = "SELECT * FROM projektbd.users WHERE login= '" + textBoxLogin.Text +
            "' AND password= '" + textBoxPasswd.Text + "'";

            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                login = reader["login"].ToString();
                passwd = reader["password"].ToString();
            }

            if (login != null || passwd != null)
            {
                MessageBox.Show("Logowanie Udane!", "Logowanie", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Podane dane są nieprawidłowe.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 
    }
}
