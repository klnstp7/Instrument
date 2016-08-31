using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Interop.Excel;
using ToolsLib.Utility;

namespace GRGTCommonUtils
{
    public class ExcelHelper
    {
        /**************************************************************************************************************
         * Excel转PDF环境要求:
         * 先安装office 2007
         * 然后安装office 2007 PDF转换插件SaveAsPDFandXPS.exe（去官网下去）
         * 然后新建项目，引用Microsoft Excel 12.0 Object Library
         * 
         * 一、配置Microsoft Excel Application
         * 二、网站的AppPool的标识要设置为LocalSystem或者NetworkService
         * 
         * 因并发导致打开Excel文件异常时处理方法：
         * 方法一：开始运行DCOMCNFG.EXE或comexp.msc -32 ---组件服务---计算机---我的电脑----DCOM配置--- Microsoft Excel Application属性 ----标识选择-“交互式用户或者启动用户”
         * ************************************************************************************************************/
        /// <summary>
        ///  Excel转PDF
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="pdfPath"></param>
        public static bool ExcelConvertToPDF(string excelPath, string pdfPath)
        {
            return ExcelConvertToPDF(excelPath, pdfPath, -1, -1);
        }
        /// <summary>
        ///  Excel转PDF
        /// </summary>
        /// <param name="excelPath"></param>
        /// <param name="pdfPath"></param>
        public static bool ExcelConvertToPDF(string excelPath, string pdfPath, int FromIndex, int ToIndex)
        {
            excelPath = CommonUtils.GetPhysicsPath(excelPath);
            pdfPath = CommonUtils.GetPhysicsPath(pdfPath);
            bool result = false;
            XlFixedFormatType targetType = XlFixedFormatType.xlTypePDF;
            object missing = Type.Missing;
            Application application = null;
            Microsoft.Office.Interop.Excel.Workbook workBook = null;
            try
            {
                application = new Application();
                object type = targetType;
                workBook = application.Workbooks.Open(excelPath, missing, missing, missing, missing, missing,
                missing, missing, missing, missing, missing, missing, missing, missing, missing);

                object From = FromIndex == -1 ? missing : FromIndex + 1;
                object To = ToIndex == -1 ? missing : ToIndex + 1;
                workBook.ExportAsFixedFormat(targetType, pdfPath, XlFixedFormatQuality.xlQualityStandard, false, false, From, To, missing, missing);
                result = true;
            }
            catch(Exception ex)
            {
                result = false;
                throw ex;
            }
            finally
            {
                if (workBook != null)
                {
                    workBook.Close(false, missing, missing);
                    workBook = null;
                }
                if (workBook != null)
                {
                    workBook.Close(false, missing, missing);
                    workBook = null;
                }
                if (application != null)
                {
                    application.Quit();
                    application = null;
                }
            }
            return result;
        }


        public static void PrintPdf(string excelFile,string pdfFile)
        {
            ToolsLib.Utility.CmdUtils.RunExeFileSync(@"D:\FlashPaper2.2\FlashPrinter.exe", string.Format("{0} -o {1}", excelFile, pdfFile));
        }
    }
}
