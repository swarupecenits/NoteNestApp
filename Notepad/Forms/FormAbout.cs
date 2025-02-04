using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Notepad.Forms
{
    public partial class FormAbout : Form
    {
        public FormAbout()
        {
            InitializeComponent();
        }

        private void FormAbout_Load(object sender, EventArgs e)
        {
            lblYear.Text = "2025-" + DateTime.Now.Year.ToString().Substring(2) + " All Rights Reserved";
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://swarup-chanda.vercel.app/",
                UseShellExecute = true
            });
        }
    }
}