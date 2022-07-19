using DesktopClient.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            Init();

            UserHelper.Instance.LoginExpiredEvent += LoginExpired;

        }

        private void Init()
        {
            bool userLogged = UserHelper.Instance.User != null;

            this.loginButton.Visible = !userLogged;
            this.registerButton.Visible = !userLogged;
            this.getAllButton.Visible = userLogged;
            this.logoutButton.Visible = userLogged;
            this.infoFromTokenButton.Visible = userLogged;
        }

        private void LoginExpired(object sender, EventArgs e)
        {
            this.Init();
        }

        private void loginButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Login login = new Login();
            login.ShowDialog();
        }

        private void registerButton_Click(object sender, EventArgs e)
        {
            this.Hide();
            Register register = new Register();
            register.ShowDialog();
        }

        private async void getAllButton_Click(object sender, EventArgs e)
        {
            var users = await UserHelper.Instance.GetAll();

            this.richTextBox1.Text = string.Empty;

            if (users == null || users.Count == 0)
                return;

            foreach (var item in users)
            {
                this.richTextBox1.Text += $"Id: {item.Id}, name: {item.UserName}, email: {item.Email} {Environment.NewLine}";
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            UserHelper.Instance.LogOut();
            Init();
        }

        private void infoFromTokenButton_Click(object sender, EventArgs e)
        {
            this.richTextBox1.Text = string.Empty;

            var user = UserHelper.Instance.User;

            if (user == null) return;

            var payload = user.AccesssToken.Split('.')[1];

            if (payload.Length % 4 != 0)
            {
                for (int i = 0; i < payload.Length % 4; i++)
                    payload += "=";
            }

            byte[] data = Convert.FromBase64String(payload);
            string decodedString = Encoding.UTF8.GetString(data);

            this.richTextBox1.Text = decodedString;

        }
    }
}
