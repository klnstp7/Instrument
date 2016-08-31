using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Configuration ;
using System.IO;
using Upgrade.Bussiness;
using Upgrade.Common;

namespace Upgrade
{
    public partial class DataBaseForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UpdScriptService));

        public DataBaseForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 加载窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UplScriptForm_Load(object sender, EventArgs e)
        {
           // this.tbxServerIPAddress.Text = "172.18.0.50";
            //this.tbxDataBase.Text = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["DataBaseName"]);
           // this.tbxUserName.Text = "sa";
           // this.tbxPassword.Text = "sasa";
        }

        /// <summary>
        /// 更新脚本
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNext_Click(object sender, EventArgs e)
        {
            if (this.tbxServerIPAddress.Text.Trim().Length == 0)
            {
                MessageBox.Show("服务器地址不能为空！","系统提示",MessageBoxButtons.OKCancel,MessageBoxIcon.Information);
                return;
            }

            if (this.tbxDataBase.Text.Trim().Length == 0)
            {
                MessageBox.Show("数据库名称不能为空！", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            if (this.tbxUserName.Text.Trim().Length == 0)
            {
                MessageBox.Show("用户名不能为空！", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }
            if (this.tbxPassword.Text.Trim().Length == 0)
            {
                MessageBox.Show("密码不能为空！", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            SqlHelper.ConnectionString = GetDataBaseConnectionString();
           
            //检查数据库连接
            try
            {
                UpdScriptService.ChkConnectSetting();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
                MessageBox.Show("尝试连接数据库失败，请重新输入！", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Error);
                return;
            }

            this.Visible = false;
            //更新
            UpgradeForm updFileFrm = new UpgradeForm();
            updFileFrm.Show();
           
        }

        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认退出更新？", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// 获取数据库字符串
        /// </summary>
        /// <returns></returns>
        private string GetDataBaseConnectionString()
        {
            string serverIPAddress=this.tbxServerIPAddress.Text.Trim();
            string dbName = String.Format("{0}", this.tbxDataBase.Text.Trim());
            string userName=this.tbxUserName.Text.Trim();
            string password = this.tbxPassword.Text.Trim();
            return string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Persist Security Info=True;", serverIPAddress, dbName, userName, password);
        }

    }
}
