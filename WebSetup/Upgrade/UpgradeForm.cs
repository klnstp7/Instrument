using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Upgrade.Models;
using Upgrade.Bussiness;
using Upgrade.Common;

namespace Upgrade
{
    public partial class UpgradeForm : Form
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(UpgradeForm));

        protected ProgressForm progressFrm = new ProgressForm();

        private bool updSuccess = false;

        /// <summary>
        /// 更新窗体
        /// </summary>
        public UpgradeForm()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 更新事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnUpgrade_Click(object sender, EventArgs e)
        {
            if(this.tbxFolderLocation.Text.Trim().Length==0)
            {
                MessageBox.Show("网站根目录不能为空！", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                return;
            }

            if (MessageBox.Show("确定更新程序？", "系统提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.Cancel)
            {
                return;
            }

            //判断版本号
            ParamModel paramModel = UpdScriptService.GetParamByCode("VN");
            if (paramModel == null)
            {
                //初始化版本号
                paramModel = new ParamModel();
                paramModel.ParamName = "V1.0.1";
                paramModel.ParamCode = "VN";
                UpdScriptService.InsertParam(paramModel);
            }
            else
            {
                string upgradeVersion=String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["UpgradeVersion"]);
                if (!(String.Compare(upgradeVersion, paramModel.ParamName) > 0))
                {
                    MessageBox.Show("升级版本不正确！","系统提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
            }

            //更新操作
            bgWorker.WorkerReportsProgress = true;
            bgWorker.RunWorkerAsync();
            progressFrm.ShowDialog();
        }

        /// <summary>
        /// 选择站点目录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBrowserFolder_Click(object sender, EventArgs e)
        {
            if (this.folderBowser.ShowDialog() == DialogResult.OK)
            {
                this.tbxFolderLocation.Text = this.folderBowser.SelectedPath;
            }
        }

        /// <summary>
        /// 退出更新
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
        /// 上一步
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnPrevStep_Click(object sender, EventArgs e)
        {
            this.Visible = false;

            DataBaseForm updScriptFrm = new DataBaseForm();
            updScriptFrm.Show();
        }

        #region 更新脚本与文件
        /// <summary>
        /// 更新脚本
        /// </summary>
        private bool UpgradeScript()
        {
            try
            {
                string scriptPath = string.Format("{0}{1}", Application.StartupPath, System.Configuration.ConfigurationManager.AppSettings["ScriptsPath"]);
                UpdScriptService.ExcuteScript(scriptPath);
                bgWorker.ReportProgress(50);
                return true; 
            }
            catch (Exception ex)
            {
                log.Error("更新脚本失败，错误信息：" + ex.Message);

                string rollBackScript = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["RollBackScript"]);
                if (rollBackScript == "true")
                {
                    string rollBackScriptPath = string.Format("{0}{1}", Application.StartupPath, System.Configuration.ConfigurationManager.AppSettings["RollBackScriptPath"]);
                    UpdScriptService.ExcuteScript(rollBackScriptPath);
                }
                return false;
            }
        }

        /// <summary>
        /// 更新站点文件
        /// </summary>
        private bool UpgradeFiles()
        {
            //是否备份站点文件
            string webFilesBackup = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["WebFilesBackup"]);
            //备份根目录命名
            string randFolderName = String.Format("{0:yyyyMMddHHmmss}", DateTime.Now);
            string backupPath = string.Format("{0}\\{1}\\{2}", Application.StartupPath, "backup", randFolderName);
            //排除的文件类型
            string exceptBKFileType = string.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["ExceptBKFileType"]);

            //获取更新包路径
            string updFilePath = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["UpdFilesPath"]);
            string updFileFolder = string.Format("{0}{1}", Application.StartupPath, updFilePath);

             //网站根目录
            string webSiteRootPath = this.tbxFolderLocation.Text.Trim();

            try
            {
                //备份文件
                if (webFilesBackup == "true") this.BackupWebFiles(webSiteRootPath, backupPath, exceptBKFileType);
                bgWorker.ReportProgress(70);

                //更新文件
                this.UpdateWebFiles(webSiteRootPath, updFileFolder);
                bgWorker.ReportProgress(100);
               // throw new Exception("WebFiles RollBack");
                return true;
            }
            catch (Exception ex)
            {
                log.Error("更新文件失败，错误信息："+ex.Message);
                string rollBackWebFiles = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["RollBackWebFiles"]);
                if (rollBackWebFiles == "true")
                {
                    this.RollBackWebFiles(backupPath, updFileFolder, webSiteRootPath, exceptBKFileType);
                }
                return false;
            }
        }

        /// <summary>
        /// 备份目录以及所有的文件
        /// </summary>
        /// <param name="folderPath"></param>
        private void BackupWebFiles(string folderPath,string backupPath,string exceptBKFileType)
        {
            //创建备份根目录
            if (Directory.Exists(backupPath) == false) Directory.CreateDirectory(backupPath);

            //复制目录
            IList<string> webDirectoryList = new List<string>();
            FileHelper.GetAllDirectorys(folderPath, ref webDirectoryList);
            foreach (string webDirectory in webDirectoryList)
            {
                string backupDirectory = string.Format("{0}", webDirectory.Replace(folderPath, backupPath));
                Directory.CreateDirectory(backupDirectory);
            }

            //复制文件
            IList<string> webFileList = new List<string>();
            FileHelper.GetAllFiles(folderPath, ref webFileList);
            foreach (string webFile in webFileList)
            {
                string backupFileName = string.Format("{0}", webFile.Replace(folderPath, backupPath));
                if (exceptBKFileType.Contains(Path.GetExtension(backupFileName).ToLower()) == true) continue;
                File.Copy(webFile, backupFileName, true);
            }
        }

        /// <summary>
        /// 回滚目录以及所有的文件
        /// </summary>
        /// <param name="folderPath"></param>
        private void RollBackWebFiles(string backupPath,string updFileFolder, string folderPath, string exceptBKFileType )
        {
            //删除新增文件
            IList<string> updFileList = new List<string>();
            FileHelper.GetAllFiles(updFileFolder, ref updFileList); ;
            foreach (string updFile in updFileList)
            {
                string backupFileName = string.Format("{0}", updFile.Replace(updFileFolder, backupPath));
                string webSiteFileName = string.Format("{0}", updFile.Replace(updFileFolder, folderPath));
                if (File.Exists(backupFileName) == false && File.Exists(webSiteFileName))
                {
                    File.Delete(webSiteFileName);
                }
            }

            //删除新增目录
            IList<string> updDirectoryList = new List<string>();
            FileHelper.GetAllDirectorys(updFileFolder, ref updDirectoryList);
            foreach (string updDirectory in updDirectoryList)
            {
                string backupDirectoryName = string.Format("{0}", updDirectory.Replace(updFileFolder, backupPath));
                string webSiteDirectoryName = string.Format("{0}", updDirectory.Replace(updFileFolder, folderPath));
                if (Directory.Exists(backupDirectoryName) == false && Directory.Exists(webSiteDirectoryName) == true)
                {
                    Directory.Delete(webSiteDirectoryName);
                }
            }

            //还原备份文件
            IList<string> webFileList = new List<string>();
            FileHelper.GetAllFiles(backupPath, ref webFileList);
            foreach (string backupFileName in webFileList)
            {
                string webFile = string.Format("{0}", backupFileName.Replace(backupPath, folderPath));
                if (exceptBKFileType.Contains(Path.GetExtension(webFile).ToLower()) == true) continue;
                File.Copy(backupFileName, webFile, true);
            }
        }

        /// <summary>
        /// 更新目录以及所有的文件
        /// </summary>
        /// <param name="srcFolderPath"></param>
        /// <param name="updFileFolder"></param>
        private void UpdateWebFiles(string srcFolderPath, string updFileFolder)
        {
            //遍历目录
            IList<string> updDirectoryList = new List<string>();
            FileHelper.GetAllDirectorys(updFileFolder, ref updDirectoryList);
            foreach (string updDirectory in updDirectoryList)
            {
                string webSiteDirectoryName = string.Format("{0}", updDirectory.Replace(updFileFolder, srcFolderPath));
                if (Directory.Exists(webSiteDirectoryName) == false)
                {
                    Directory.CreateDirectory(webSiteDirectoryName);
                }
            }

            //遍历文件
            IList<string> updFileList = new List<string>();
            FileHelper.GetAllFiles(updFileFolder, ref updFileList); ;
            foreach (string updFile in updFileList)
            {
                string webSiteFileName = string.Format("{0}", updFile.Replace(updFileFolder, srcFolderPath));
                File.Copy(updFile, webSiteFileName, true);
            }
        }

       
        #endregion

        #region 更新操作
        private void bgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
           //更新脚本
            bool updScriptResult = false, updWebFilesResult = false;
            updScriptResult=this.UpgradeScript();
            Thread.Sleep(1000);

            //更新文件
            if (updScriptResult == true)
            {
                updWebFilesResult = this.UpgradeFiles();
                Thread.Sleep(1000);
            }
            if(updScriptResult==true && updWebFilesResult==true) updSuccess = true;
        }
       

        private void bgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressFrm.ProgressValue = e.ProgressPercentage;
        }

        private void bgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            progressFrm.Close();
            if (updSuccess == true)
            {
                //更新版本号
                ParamModel paramModel = new ParamModel();
                paramModel.ParamName = String.Format("{0}", System.Configuration.ConfigurationManager.AppSettings["UpgradeVersion"]);
                paramModel.ParamCode = "VN";
                UpdScriptService.UpdateParam(paramModel);

                //提示成功
                this.Visible = false;
                if (MessageBox.Show("更新成功！", "系统提示", MessageBoxButtons.OK,MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
            else
            {
                //提示失败
                MessageBox.Show("更新失败，请查看日志文件！", "系统提示",MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion
    }
}
