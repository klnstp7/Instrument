using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Web.Administration;
using System.DirectoryServices;
using System.Diagnostics;
using Microsoft.Win32;

namespace InstallerHelper
{
    public class NewWebSiteInfo
    {
        private string hostIP; // 主机IP  
        private string portNum; // 网站端口号  
        //private string descOfWebSite; // 网站表示。一般为网站的网站名。如"www.myweb.com.cn"  
        private string nameOfWebSite;// 网站名称。如"我的网站"，此处即为在IIS管理器中的网站名称  
        private string webPath; // 网站的物理地址 
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hostIP">网站主机，绑定网站的时候使用，默认为“”，全部未分配</param>
        /// <param name="portNum">网站端口号</param>
        /// <param name="nameOfWebSite">网站名称</param>
        /// <param name="webPath">网站安装地址</param>
        public NewWebSiteInfo(string hostIP, string portNum, string nameOfWebSite, string webPath)  
        {  
            this.hostIP = hostIP;
            this.portNum = portNum;  
            //this.descOfWebSite = descOfWebSite;  
            this.nameOfWebSite = nameOfWebSite;  
            this.webPath = webPath;  
        }

        #region 属性
        public string BindString  
        {  
            get  
            {  
                return String.Format("{0}:{1}:", hostIP, portNum); //网站标识（IP,端口，主机头值）  
            }  
        }  
  
        public string PortNum  
        {  
            get  
            {  
                return portNum;  
            }  
        }  
  
        public string NameOfWebSite  
        {  
            get  
            {  
                return nameOfWebSite;  
            }  
        }  
  
        public string WebPath  
        {  
            get  
            {  
                return webPath;  
            }
        }

        #endregion

    }  
}
