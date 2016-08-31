using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Data.SqlClient;
using System.ServiceProcess;
using Microsoft.Win32;
using System.DirectoryServices;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using Microsoft.Web.Administration;
using System.Security.AccessControl;
using System.Configuration;


namespace InstallerHelper
{
    [RunInstaller(true)]
    public partial class DBInstaller : System.Configuration.Install.Installer
    {
        private const string dbName = "GrgtInstrument";//默认SQL Server数据库名称
        string entPath = String.Format("IIS://{0}/w3svc", "localhost");//IIS根目录
        private string dataBaseType = "SQL";//数据库类型SQL/SQLite...
        //private string dataBaseType = ConfigurationManager.AppSettings["DataBaseType"];//数据库类型
        
        public DBInstaller()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 程序安装前方法
        /// </summary>
        /// <param name="savedState"></param>
        protected override void OnBeforeInstall(IDictionary savedState)
        {
            try
            {
                //if (string.IsNullOrEmpty(GetIISVerstion()))
                //{
                //    MessageBox.Show("检测到您的电脑没有安装IIS，无法继续安装此产品", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                //    this.Rollback(stateSaver);
                //} 
                if (ExistSqlServerService() || dataBaseType.Equals("SQLite"))
                {
                    if (IsExistSitePort())
                    {
                        MessageBox.Show("站点端口号重复", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                        //this.Rollback(savedState);
                    }
                    if (IsExistSiteName(Context.Parameters["websitename"].ToString()))
                    {
                        MessageBox.Show("站点名称重复", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                        //this.Rollback(stateSaver);
                    }
                }
                else
                {
                    MessageBox.Show("检测到您的电脑没有安装SQL SERVER，无法继续安装此产品", "信息提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    this.Rollback(savedState);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            string databaseServer = Context.Parameters["server"].ToString();
            string userName = Context.Parameters["user"].ToString();
            string userPass = Context.Parameters["pwd"].ToString();
            string targetdir = Context.Parameters["targetdir"].ToString();
            string port = Context.Parameters["port"].ToString();
            string websitename = Context.Parameters["websitename"].ToString();

            NewWebSiteInfo siteinfo = new NewWebSiteInfo("", port, websitename, @targetdir);
            CreateNewWebSite(siteinfo);
            SetFileRole();
            //WriteToReg("WebSiteID");
            //if (this.Context.Parameters["deskcut"] == "1")    //创建桌面快捷方式  
            //{
            //    CreateDeskTopCut();
            //}
            //if (this.Context.Parameters["pmenu"] == "1")    //创建应用程序菜单项  
            //{
            //    CreateProCut();
            //}  
            if (dataBaseType.Equals("SQL"))
            {
                ReWriteConfig();
                string connStr = string.Format("data source={0};user id={1};password={2};persist security info=false;packet size=4096", databaseServer, userName, userPass);
                string strSql = "EXEC sp_attach_db  @dbname  =  N'" + dbName + "',"
                                                + "@filename1  =  N'" + targetdir + "DataBase\\GrgtInstrument.mdf',"
                                                + "@filename2  =  N'" + targetdir + "DataBase\\GrgtInstrument_Log.ldf'";
                ExecuteSql(connStr, "master", strSql);
            }
        }

        private void ExecuteSql(string connStr, string DatabaseName, string Sql)
        {
            SqlConnection conn = new SqlConnection(connStr);
            SqlCommand cmd = new SqlCommand(Sql, conn);

            conn.Open();
            conn.ChangeDatabase(DatabaseName);
            try
            {
                cmd.ExecuteNonQuery();
            }
            finally
            {
                conn.Close();
            }
        }

        #region 创建站点
        public void CreateNewWebSite(NewWebSiteInfo siteInfo)
        {

            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);

            string newSiteNum = GetNewWebSiteID();
            DirectoryEntry newSiteEntry = rootEntry.Children.Add(newSiteNum, "IIsWebServer");
            newSiteEntry.CommitChanges();

            newSiteEntry.Properties["ServerBindings"].Value = siteInfo.BindString;
            newSiteEntry.Properties["ServerComment"].Value = siteInfo.NameOfWebSite;
            newSiteEntry.CommitChanges();
            DirectoryEntry vdEntry = newSiteEntry.Children.Add("root", "IIsWebVirtualDir");
            vdEntry.CommitChanges();
            string ChangWebPath = siteInfo.WebPath.Trim().Remove(siteInfo.WebPath.Trim().LastIndexOf('\\'), 1);
            vdEntry.Properties["Path"].Value = ChangWebPath;


            vdEntry.Invoke("AppCreate", true);//创建应用程序  

            vdEntry.Properties["AccessRead"][0] = true; //设置读取权限  
            vdEntry.Properties["AccessWrite"][0] = true;
            vdEntry.Properties["AccessScript"][0] = true;//执行权限  
            vdEntry.Properties["AccessExecute"][0] = false;
            vdEntry.Properties["AppFriendlyName"][0] = siteInfo.NameOfWebSite; //应用程序名称   
            vdEntry.Properties["AuthFlags"][0] = 1;//0表示不允许匿名访问，1表示可以匿名访问，3为基本身份验证，7为windows继承身份验证  
            vdEntry.CommitChanges();

            #region 创建应用程序池
            string appPoolName = "GrgtInstrumentPool";
            if (!IsAppPoolName(appPoolName))
            {
                DirectoryEntry newpool;
                DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
                newpool = appPools.Children.Add(appPoolName, "IIsApplicationPool");
                newpool.CommitChanges();
            }
            #endregion

            #region 针对IIS7

            int Version = int.Parse(GetIISVerstion());
            if (Version > 6)
            {
                #region 修改应用程序的配置(包含托管模式及其NET运行版本)
                ServerManager sm = new ServerManager();
                sm.ApplicationPools[appPoolName].ManagedRuntimeVersion = "v4.0";
                sm.ApplicationPools[appPoolName].Enable32BitAppOnWin64 = true;
                sm.ApplicationPools[appPoolName].ManagedPipelineMode = ManagedPipelineMode.Integrated; //托管模式:Integrated为集成 Classic为经典  
                sm.CommitChanges();
                #endregion

            }
            vdEntry.Properties["AppPoolId"].Value = appPoolName;
            vdEntry.CommitChanges();
            #endregion

            #region 更新脚本映射 网站.net framwork版本 （2.0/4.0）
            //启动aspnet_regiis.exe程序   
            string fileName = Environment.GetEnvironmentVariable("windir") + @"\Microsoft.NET\Framework\v4.0.30319\aspnet_regiis.exe";
            ProcessStartInfo startInfo = new ProcessStartInfo(fileName);
            //处理目录路径   
            string path = vdEntry.Path.ToUpper();
            int index = path.IndexOf("W3SVC");
            path = path.Remove(0, index);
            //启动ASPnet_iis.exe程序,刷新脚本映射   
            startInfo.Arguments = "-i -enable ";
            startInfo.WindowStyle = ProcessWindowStyle.Hidden;
            startInfo.UseShellExecute = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;
            Process process = new Process();
            process.StartInfo = startInfo;
            process.Start();
            process.WaitForExit();
            string errors = process.StandardError.ReadToEnd();
            if (errors != string.Empty)
            {
                throw new Exception(errors);
            }
            #endregion
        }
        #endregion  

        #region 设置SQL Server数据库连接字符串,SQLite不需要设置
        /// <summary>
        /// 设置数据库连接字符串，将字符串里的关键字替换成用户输入的数据库信息
        /// </summary>
        private void ReWriteConfig()
        {
            string configpath = Path.Combine(this.Context.Parameters["targetdir"].ToString() + "/App_Data", "properties.config");
            string configstring = File.ReadAllText(configpath).Replace("#ServerName#", Context.Parameters["server"].ToString());
            configstring = configstring.Replace("#UserId#", Context.Parameters["user"].ToString());
            configstring = configstring.Replace("#Pwd#", Context.Parameters["pwd"].ToString());
            File.WriteAllText(configpath, configstring);
        }
        #endregion

        #region 判断是否安装了SQL SERVER
        /// <summary>
        /// 判断是否安装了SQL SERVER
        /// </summary>
        /// <returns></returns>
        private bool ExistSqlServerService()
        {
            bool Exist = false;
            ServiceController[] service = ServiceController.GetServices();
            for (int i = 0; i < service.Length; i++)
            {
                if (service[i].ServiceName.Length > 5 && service[i].ServiceName.Substring(0, 5) == "MSSQL") //if (service[i].ServiceName == "MSSQLSERVER")
                {
                    Exist = true;
                    break;
                }
            }
            return Exist;
        }
        #endregion

        #region 检测IIS及版本号
        /// <summary>
        /// 检测IIS及版本号
        /// </summary>
        /// <returns></returns>
        private string GetIISVerstion()
        {
            //IIS注册表
            //SYSTEM\CurrentControlSet\Services\W3SVC\Parameters
            //SOFTWARE\Microsoft\InetStp
            //错误信息
            //此安装程序需要IIS(Internet Information Server) 6.0 或更高版本，以及Windows XP 或更高版本。请安装IIS(Internet Information Server) 或更新的操作系统，然后重新运行此安装程序。
            RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\INetStp");
            if (key == null)
                return string.Empty;
            else
                return Convert.ToString(key.GetValue("MajorVersion"));// +"." + Convert.ToString(key.GetValue("MinorVersion"));

        }
        #endregion

        #region 判断应用程序池是否存在
        /// <summary>  
        /// 返回true即应用程序池存在  
        /// </summary>  
        /// <param name="AppPoolName">应用程序池名</param>  
        private bool IsAppPoolName(string AppPoolName)
        {
            bool result = false;
            DirectoryEntry appPools = new DirectoryEntry("IIS://localhost/W3SVC/AppPools");
            foreach (DirectoryEntry getdir in appPools.Children)
            {
                if (getdir.Name.Equals(AppPoolName))
                {
                    result = true;
                }
            }
            return result;
        }
        #endregion  
        
        #region 端口号是否重复
        /// <summary>
        /// 端口号是否重复
        /// </summary>
        /// <returns></returns>
        private bool IsExistSitePort()
        {
            bool exist = false;
            try
            {
                DirectoryEntry rootfolder = GetDirectoryEntry(entPath);
                foreach (DirectoryEntry child in rootfolder.Children)
                {
                    if (child.SchemaClassName.Equals("IIsWebServer", StringComparison.OrdinalIgnoreCase))//IIS网站
                    {
                        if (child.Properties["ServerBindings"].Value != null && child.Properties["ServerBindings"].Value.ToString().Split(':').Length > 1)
                        {
                            if (child.Properties["ServerBindings"].Value.ToString().Split(':')[1] == Context.Parameters["port"].ToString())
                            {
                                exist = true;
                                break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return exist;
        }
        #endregion

        #region 站点名称是否存在

        /// <summary>
        /// 站点名称是否存在
        /// </summary>
        /// <returns></returns>
        private bool IsExistSiteName(string sitename)
        {
            bool exist = false;
            using (DirectoryEntry root = GetDirectoryEntry(entPath))
            {
                foreach (DirectoryEntry Child in root.Children)
                {
                    if (Child.SchemaClassName == "IIsWebServer")
                    {
                        string WName = Child.Properties["ServerComment"].Value.ToString();
                        if (sitename == WName)
                        {
                            exist = true;
                            break;
                        }
                    }
                }
                root.Close();
            }
            return exist;
        }
        #endregion
        
        #region 获得站点新ID
        /// <summary>  
        /// 取得现有最大站点ID，在其上加1即为新站点ID  
        /// </summary>  
        private string GetNewWebSiteID()
        {
            int siteID = 1;
            DirectoryEntry rootEntry = GetDirectoryEntry(entPath);
            foreach (DirectoryEntry de in rootEntry.Children)
            {
                if (de.SchemaClassName == "IIsWebServer")
                {
                    int ID = Convert.ToInt32(de.Name);
                    if (ID >= siteID)
                    {
                        siteID = ID + 1;
                    }
                }
            }
            return siteID.ToString().Trim();
        }


        public DirectoryEntry GetDirectoryEntry(string entPath)
        {
            DirectoryEntry ent = new DirectoryEntry(entPath);
            return ent;
        }  
        #endregion  

        #region 赋予指定用户文件夹权限
        /// <summary>
        /// 赋予指定用户文件夹权限
        /// IIS7: IIS_IUSRS
        /// IIS6: Users
        /// </summary>
        public void SetFileRole()
        {
            int iVersion = int.Parse(GetIISVerstion());
            string fileAddr = this.Context.Parameters["targetdir"].ToString();
            fileAddr = fileAddr.Remove(fileAddr.LastIndexOf('\\'), 1);
            DirectorySecurity fSec = new DirectorySecurity();
            string idendity = string.Empty;
            if (iVersion > 6)
                idendity = "IIS_IUSRS";
            else
                idendity = "Users";
            fSec.AddAccessRule(new FileSystemAccessRule(idendity, FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            //为安装用户设置安装目录的完全控制权限，防止附加数据库时权限不足而附加失败
            fSec.AddAccessRule(new FileSystemAccessRule("Everyone", FileSystemRights.FullControl, InheritanceFlags.ContainerInherit | InheritanceFlags.ObjectInherit, PropagationFlags.None, AccessControlType.Allow));
            System.IO.Directory.SetAccessControl(fileAddr, fSec);
        }
        #endregion
    }
}
