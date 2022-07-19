using DesktopClient.Helpers;
using DesktopClient.Models;
using System;
using System.Windows.Forms;

namespace DesktopClient
{
    public partial class Register : Form
    {
        public Register()
        {
            InitializeComponent();
        }

        private async void registerButton_Click(object sender, EventArgs e)
        {
            var model = new RegisterModel
            {
                UserName = this.userNameTextBox.Text,
                Email = this.emailTextBox.Text,
                Password = this.passwordTextBox.Text,
            };

            bool isRegistered = await UserHelper.Instance.RegisterAsync(model);

            if (isRegistered)
            {
                MessageBox.Show("User Registered Successfully");
                this.Hide();
                Form1 form1 = new Form1();
                form1.ShowDialog();
            }

            else
                MessageBox.Show("Registration Errror");
        }
    }
}
