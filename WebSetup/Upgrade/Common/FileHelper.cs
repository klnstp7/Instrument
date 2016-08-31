using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Upgrade.Common
{
    public class FileHelper
    {
        /// <summary>
        /// 创建目录
        /// </summary>
        /// <param name="path"></param>
        public static void CreateDirectory(string path)
        {
            if (Directory.Exists(path) == false) Directory.CreateDirectory(path);
        }


        /// <summary>
        /// 获取指定路径下的所有目录
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="directoryList"></param>
        public static void GetAllDirectorys(string directoryPath, ref IList<string> directoryList)
        {
            string[] subDirectoryArray = System.IO.Directory.GetDirectories(directoryPath); ;
            foreach (string subDirectory in subDirectoryArray)
            {
                directoryList.Add(subDirectory);
                if (Directory.GetDirectories(subDirectory).Length > 0)
                {
                    GetAllDirectorys(subDirectory, ref directoryList);
                }
            }
        }

        /// <summary>
        /// 获取指定目录下的所有文件
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <param name="fileList"></param>
        public static void GetAllFiles(string directoryPath, ref IList<string> fileList)
        {
            string[] fileSystemInfos = Directory.GetFileSystemEntries(directoryPath);
            foreach (string fileSystemInfo in fileSystemInfos)
            {
                if (System.IO.Directory.Exists(fileSystemInfo) == true)
                {
                    GetAllFiles(fileSystemInfo, ref fileList);
                }
                if (System.IO.File.Exists(fileSystemInfo) == true)
                {
                    fileList.Add(fileSystemInfo);
                }
            }
        }

    }
}
