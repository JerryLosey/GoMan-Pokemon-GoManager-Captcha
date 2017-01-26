﻿using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using GoMan.Model;

namespace GoMan.View
{
    public partial class LoginForm : Form
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam,[MarshalAs(UnmanagedType.LPWStr)] string lParam);
        public LoginForm()
        {
            InitializeComponent();
        }

        private async void btnSubmit_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(ApplicationModel.Settings.Username))
            {
                MessageBox.Show(this, "Enter a username",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (string.IsNullOrEmpty(ApplicationModel.Settings.Password))
            {
                MessageBox.Show(this, "Enter a password",
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }


            pbRefresh.Visible = true;
            btnSubmit.Enabled = false;

            var methodResult = await LogonOn.TryPing();

            pbRefresh.Visible = false;
            btnSubmit.Enabled = true;

            if (methodResult.Error != null)
                MessageBox.Show(this, methodResult.Error.Message + "\n" + methodResult.Error.StackTrace,
                    "ERROR", MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                this.DialogResult = DialogResult.OK;
        }

        private void txtUsername_TextChanged(object sender, EventArgs e)
        {
                ApplicationModel.Settings.Username = txtUsername.Text;
                ApplicationModel.Settings.SaveSetting();
        }

        private void txtPassword_TextChanged(object sender, EventArgs e)
        {
                ApplicationModel.Settings.Password = txtPassword.Text;
                ApplicationModel.Settings.SaveSetting();
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            txtPassword.Text = ApplicationModel.Settings.Password;
            txtUsername.Text = ApplicationModel.Settings.Username;

            SendMessage(this.txtUsername.Handle, 5377, 0, "username");
            SendMessage(this.txtPassword.Handle, 5377, 0, "password");
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://goman.io/my-account/");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }
    }
}