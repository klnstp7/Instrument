using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Upgrade
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnNext_Click(object sender, EventArgs e)
        {

            if (this.chkAgree.Checked == false)
            {
                MessageBox.Show("必须同意协议才能进行更新操作！","系统提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Error);
                return;
            }
            this.btnNext.Enabled = false;
            DataBaseForm dataBaseFrm = new DataBaseForm();
            dataBaseFrm.Show();
            this.Visible = false;
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认退出更新？", "系统提示", MessageBoxButtons.OKCancel,MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        private void chkAgree_CheckedChanged(object sender, EventArgs e)
        {
            this.btnNext.Enabled = this.chkAgree.Checked;
        }

    }
}
