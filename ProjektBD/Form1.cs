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

        /*private void loginBtn_Click(object sender, EventArgs e)
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

        }*/

        string login, haslo;

        private void loginBtn_Click(object sender, EventArgs e) // sprawdza czy użytkownik jest w bazie danych
        {
            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = connstring;
            con.Open();

            //ścieżka dostępu do bazy musisz dac swoja tutaj
            string query = "SELECT * FROM projektbd.users WHERE login= '" + textBoxLogin.Text +
            "'AND password = '" + textBoxPasswd.Text + " ' ";
            MySqlCommand sda = new MySqlCommand(query, con);
            MySqlDataReader reader = sda.ExecuteReader();
            while (reader.Read())
            {
                login = reader["login"].ToString();
                haslo = reader["password"].ToString();
            }
            if (login != "") // logowanie udane
            {

                    MessageBox.Show("Logowanie Udane!", "Logowanie", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //MainPannel panel = new MainPannel();
                    //panel.Show();

            }
            else // logowanie nieudane
            {

                    MessageBox.Show("Podane dane są nieprawidłowe.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }

        }
        
    }
}
