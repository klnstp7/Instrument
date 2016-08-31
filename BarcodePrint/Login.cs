using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Json;
using BarcodePrint.Models;
using System.Configuration;

namespace BarcodePrint
{
    public partial class Login : Form
    {
        public Login()
        {
            this.StartPosition = FormStartPosition.CenterScreen;
            InitializeComponent();
            try
            {
                if (!File.Exists(@"C:\WINDOWS\system\TSCLIB.dll"))
                    File.Copy(Directory.GetCurrentDirectory() + @"\lib\TSCLIB.dll", @"C:\WINDOWS\system\TSCLIB.dll", true);
            }
            catch (System.Exception ex)
            {

            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtLoginName.Text))
            {
                MessageBox.Show("请输入登录账号！");
                txtLoginName.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("请输入密码！"); txtPassword.Focus();
                return;
            }
            string XmlFile = string.Format(@"{0}\{1}.xml", Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WCF_xmlFile"]);
            if (!File.Exists(XmlFile))
            {
                MessageBox.Show("请先设置访问的IP！");

                WcfAddressSettings seetings = new WcfAddressSettings();
                seetings.StartPosition = FormStartPosition.CenterScreen;
                seetings.Show();
                this.Visible = false; ;
                return;
            }
            string result = "";
            try
            {
                ServiceReference1.InstrumentWCFServices wcf = InvokeContext.CreateWCFService<ServiceReference1.InstrumentWCFServices>();
                result = wcf.AppLogin(txtLoginName.Text, txtPassword.Text, "");
            }
            catch
            {
                MessageBox.Show("连接服务失败，请检查IP及端口设置是否正确");
                return;
            }
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(LoginResult));
                LoginResult model = (LoginResult)serializer.ReadObject(ms);
                if (model.Msg != "OK")
                {
                    MessageBox.Show(model.Msg);
                    txtPassword.Text = "";
                    return;
                }
                LoginInfo.CurrentUser.JobNo = txtLoginName.Text;
                LoginInfo.CurrentUser.AccessToKen = model.Data.AccessToKen;

                MainForm mainform = new MainForm();

                mainform.StartPosition = FormStartPosition.CenterScreen;
                mainform.Show();
                this.Visible = false; ;

            }
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                button1_Click(null, null);
            }
        }

        private void txtLoginName_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (txtLoginName.Text != "" && e.KeyChar == '\r')
            {
                txtPassword.Focus();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            WcfAddressSettings seetings = new WcfAddressSettings();

            seetings.StartPosition = FormStartPosition.CenterScreen;
            seetings.Show();
            this.Visible = false; ;
        }
    }
}
