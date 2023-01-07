﻿using MySql.Data.MySqlClient;
using Org.BouncyCastle.Math.EC;
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

namespace ProjektBD
{
    public partial class RegisterPanel : Form{

        private string temp;
        private int id_rank;

        public RegisterPanel(){
            InitializeComponent();
            rankBox.Items.Add("Klient");
            rankBox.Items.Add("Agent Nieruchomości");
        }

        private bool CheckLogin()
        {
            string query = "SELECT COUNT(*) AS liczba FROM projektbd.users WHERE login= '" + loginBox.Text + "' ";

            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                temp = reader["liczba"].ToString();
            }
            if (temp == "1")
            {
                MessageBox.Show("login juz wykorzstywany", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
                return false;
            }
            con.Close();
            return true;
        }
        private bool CheckEmail()
        {
            string query = "SELECT COUNT(*) AS liczba FROM projektbd.users WHERE email= '" + emailBox.Text + "' ";

            MySqlConnection con = new MySqlConnection();
            con.ConnectionString = DataBase.Connstring;
            con.Open();
            MySqlCommand cmd = new MySqlCommand(query, con);
            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                temp = reader["liczba"].ToString();
            }
            if (temp == "1")
            {
                MessageBox.Show("email juz wykorzstywany", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                con.Close();
                return false;
            }
            con.Close();
            return true;
        }

        private bool VerifyData()
        {
            foreach (TextBox tb in this.Controls.OfType<TextBox>())
            {
                if (tb.Text == "" && tb != no_apartamentBox)
                {
                    MessageBox.Show("Uzupełnij wszystkie pola!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return false;
                }
            }
            if(rankBox.Text != "Klient" && rankBox.Text != "Agent Nieruchomości")
            {
                MessageBox.Show("Wybierz kategorie!", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            if(!emailBox.Text.Contains("@"))
            {
                MessageBox.Show("Wpisz poprawnie adres mailowy", "Błąd", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }
            CheckLogin();
            CheckEmail();
            return true;
        }

        private void InsertUser()
        {
            if (rankBox.Text == "Klient")
            {
                id_rank = 1;
            }
            else if (rankBox.Text == "Agent Nieruchomości")
            {
                id_rank = 2;
            }
            try
            {
                string query = "INSERT INTO projektbd.users(first_name,last_name,login,password,email,id_rank,street,no_building,no_apartament,zip_code,city) " +
                "VALUES('" + first_nameBox.Text + "','" + last_nameBox.Text + "','" + loginBox.Text + "','" + passwordBox.Text + "','" + emailBox.Text + "','" +
                id_rank.ToString() + "','" + streetBox.Text + "','" + no_buildingBox.Text + "','" + no_apartamentBox.Text + "','" + zip_codeTxt.Text + "','" + cityBox.Text + "');";

                MySqlConnection con = new MySqlConnection();
                con.ConnectionString = DataBase.Connstring;
                con.Open();
                MySqlCommand cmd = new MySqlCommand(query, con);
                cmd.ExecuteReader();
                con.Close();
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            MessageBox.Show("Pomyślnie Zarejestrowano!", "Rejestracja", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void registerBtn_Click(object sender, EventArgs e){
            if (VerifyData()) {
                InsertUser();
                this.Close();
            }
        }
    }
}
