﻿using System;
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
                User.Id = Int32.Parse(reader["id"].ToString());
                User.FirstName = reader["first_name"].ToString();
                User.LastName = reader["last_name"].ToString();
                User.Login = reader["login"].ToString();
                User.Password = reader["password"].ToString();
                User.Email = reader["email"].ToString();
                User.Id_rank = Int32.Parse(reader["id_rank"].ToString());
                User.Street = reader["street"].ToString();
                User.No_Building = reader["no_building"].ToString();
                User.No_apartament = reader["no_apartament"].ToString();
                User.ZipCode = reader["zip_code"].ToString();
                User.City = reader["city"].ToString();
            }
            con.Close();

            if (User.Login != null || User.Password != null)
            {
                MessageBox.Show("Logowanie Udane!", "Logowanie", MessageBoxButtons.OK, MessageBoxIcon.Information); 
                AdminPanel admin = new AdminPanel();
                admin.Show();
                this.Hide();

            }
            else
            {
                MessageBox.Show("Podane dane są nieprawidłowe.", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        } 
    }
}
