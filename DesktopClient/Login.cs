using DesktopClient.Helpers;
using DesktopClient.Models;
using System;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Login : Form
    {
        public Login()
        {
            InitializeComponent();
        }

        private async void loginButton_Click(object sender, EventArgs e)
        {
            var model = new LoginModel
            {
                Email = this.emailTextBox.Text,
                Password = this.passwordTextBox.Text
            };

            bool isLogged = await UserHelper.Instance.LogIn(model);

            if (isLogged)
            {
                MessageBox.Show("User Logged Successfully");
                this.Hide();
                Form1 form1 = new Form1();
                form1.ShowDialog();
            }
            else
                MessageBox.Show("Login Errros");
        }
    }
}
