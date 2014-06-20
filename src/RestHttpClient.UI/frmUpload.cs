﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace RestHttpClient
{
    public partial class frmUpload : Form
    {
        public frmUpload()
        {
            InitializeComponent();
        }

        private void buttonClose_Click(object sender, EventArgs e)
        {
            try
            {
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            try
            {
                var httpSrv = new HttpService();
                var token = httpSrv.GetToken(textBoxApiKey.Text);
                var login = httpSrv.Login(token, textBoxUserName.Text, textBoxPassword.Text);
                var uploadInfo = httpSrv.GetUploadInf(login);

                var fileName = textBoxFileName.Text;
                httpSrv.UploadFile(fileName, uploadInfo);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void buttonFile_Click(object sender, EventArgs e)
        {
            try
            {
                var form = new OpenFileDialog();

                if (form.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
                {
                    textBoxFileName.Text = form.FileName;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
