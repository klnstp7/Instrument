using System;
using System.Configuration;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.Serialization.Json;
using BarcodePrint.Models;
using System.Drawing.Printing;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;


namespace BarcodePrint
{
    public partial class MainForm : Form
    {
        IList<InstrumentModel> Assetslist = new List<InstrumentModel>();
        IList<InstrumentModel> Instrumentlist = new List<InstrumentModel>();
        public MainForm()
        {
            InitializeComponent();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            PrintDocument print = new PrintDocument();
            string sDefault = print.PrinterSettings.PrinterName;//默认打印机名
            comboBox1.Items.Add("");
            foreach (string sPrint in PrinterSettings.InstalledPrinters)//获取所有打印机名称
            {
                comboBox1.Items.Add(sPrint);
                if (sPrint == sDefault)
                    comboBox1.SelectedIndex = comboBox1.Items.IndexOf(sPrint);
            }
            BindAssetsData();
            BindInstrumentData();
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            string s = "^[1-9]*$";//正则表达式
            Regex reg = new Regex(s);
            if (!reg.IsMatch(this.txtinstrumentNum.Text))
            {
                MessageBox.Show("请输入大于0的整数");
                txtinstrumentNum.Text = "1";
                return;
            }
            DataGridView gv = tabControl1.SelectedTab.Text == "仪器标准" ? dataGridView1:gridStockItems ;
            string cb_chck = tabControl1.SelectedTab.Text == "仪器标准" ?  "cb_check1":"cb_check" ;
            bool R = false;
            int count = Convert.ToInt32(gv.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)gv.Rows[i].Cells[cb_chck];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (!flag)
                    continue;
                else
                {
                    R = true;
                    break;
                }

            }
            if (!R)
            {
                MessageBox.Show("未勾选任何记录");
                return;
            }
            PrintLayout PL = new PrintLayout();
            PL.PrintEvent += new BarcodePrint.PrintLayout.DisplayPrint(Print);
            PL.ShowDialog();


        }

        private IList<InstrumentModel> BindData(int InstrumentForm)
        {
            string result = "";
            try { 
            ServiceReference1.InstrumentWCFServices wcf = InvokeContext.CreateWCFService<ServiceReference1.InstrumentWCFServices>();
            result = wcf.GetAllInstrument(InstrumentForm, LoginInfo.CurrentUser.JobNo.ToString(), LoginInfo.CurrentUser.AccessToKen);
            }
            catch { MessageBox.Show("连接服务失败，请检查IP及端口设置是否正确！"); return null; }
            using (MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(result)))
            {
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(RsultModel));
                RsultModel model = (RsultModel)serializer.ReadObject(ms);
                if (model.Msg != "OK")
                {
                    MessageBox.Show(model.Msg);
                    return null;
                }
                return model.Data;
            }
        }

        private void BindAssetsData()
        {
            Assetslist = BindData(Constants.InstrumentForm.固定资产.GetHashCode());
            if (Assetslist == null)
                Assetslist = new List<InstrumentModel>();
            gridStockItems.DataSource = Assetslist;
        }
        private void BindInstrumentData()
        {
            Instrumentlist = BindData(Constants.InstrumentForm.仪器.GetHashCode());
            if (Instrumentlist == null)
                Instrumentlist = new List<InstrumentModel>();
            dataGridView1.DataSource = Instrumentlist;
        }
        /// <summary>
        /// 刷新
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BindAssetsData();
        }
        private void BtnInstrumentRefresh_Click(object sender, EventArgs e)
        {
            BindInstrumentData();
        }
        /// <summary>
        /// 退出
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        #region ===打印===

        public void Print(PrintLayoutEventArgs e)
        {
            DataGridView gv = tabControl1.SelectedTab.Text == "仪器标准" ? dataGridView1 : gridStockItems;
            string cb_chck = tabControl1.SelectedTab.Text == "仪器标准" ? "cb_check1" : "cb_check";
            Print(gv, cb_chck, e.Layout);
        }
        private void Print(DataGridView gv, string cb_chck, Constants.BarCodeLayout layout)
        {
            bool R = false;
            int count = Convert.ToInt32(gv.Rows.Count.ToString());
            for (int i = 0; i < count; i++)
            {
                DataGridViewCheckBoxCell checkCell = (DataGridViewCheckBoxCell)gv.Rows[i].Cells[cb_chck];
                Boolean flag = Convert.ToBoolean(checkCell.Value);
                if (!flag)
                    continue;
                R = true;
                DataGridViewRow row = gv.Rows[i];
                InstrumentModel item = (InstrumentModel)row.DataBoundItem;
                for (int j = 0; j < int.Parse(txtinstrumentNum.Text); j++)
                    Print(item, layout);
            }
            if (!R)
                MessageBox.Show("未勾选任何记录");
        }
        private void Print(InstrumentModel m, Constants.BarCodeLayout layout)
        {
            // 打开 打印机 端口.
            try
            {
                if (string.IsNullOrEmpty(comboBox1.SelectedItem.ToString()))
                {
                    MessageBox.Show("请选择打印机。");
                    return;
                }
                TSCLIB_DLL.openport(comboBox1.SelectedItem.ToString());                                                //Open 
            }
            catch (Exception ex)
            {
                MessageBox.Show(string.Format("{0}打印机安装出错或没有安装驱动。", comboBox1.SelectedItem.ToString()));
                return;
            }
            switch (layout)
            {
                case Constants.BarCodeLayout.二维码左:
                    QRCode_ABPrint(m);
                    break;
                case Constants.BarCodeLayout.二维码右:
                    QRCode_BAPrint(m);
                    break;
                case Constants.BarCodeLayout.条码上:
                    BarCode_ABPrint(m);
                    break;
                case Constants.BarCodeLayout.条码下:
                    BarCode_BAPrint(m);
                    break;
                case Constants.BarCodeLayout.混合条码上:
                    MixCode_ABPrint(m);
                    break;
                case Constants.BarCodeLayout.混合条码下:
                    MixCode_BAPrint(m);
                    break;
            }

            //// if (radioButton1.Checked)//二维码
            //     QRCodePrint(m, layout);//二维码
            // //else if (radioButton2.Checked)//条形码
            //     BarCodePrint(m, layout);//条形码
            // //else 
            // MixCodePrint(m, layout);//混合
        }

        #region=====二维码打印=======
        /// <summary>
        /// 调用TSC打印机打印
        /// </summary>
        private void QRCode_ABPrint(InstrumentModel m)
        {
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            TSCLIB_DLL.sendcommand("DIRECTION 0");
            string strP = "QRCODE 200,60,H,6,A,0,M2,S1,\"" + m.BarCode + "\"";
            TSCLIB_DLL.sendcommand(strP);
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            //TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            //TSCLIB_DLL.windowsfont(340, 120, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            //TSCLIB_DLL.windowsfont(340, 160, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            //TSCLIB_DLL.windowsfont(340, 200, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            //TSCLIB_DLL.windowsfont(340, 240, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            //TSCLIB_DLL.windowsfont(340, 280, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            int Y = 0;
            string name = m.InstrumentName;
            if (m.InstrumentName.Length > 10)
            {
                Y = 40;
                name = m.InstrumentName.Substring(0, 10);
            }
            TSCLIB_DLL.windowsfont(340, 50, 23, 0, 2, 0, "宋体", "名称:" + name);
            if (m.InstrumentName.Length > 10)
                TSCLIB_DLL.windowsfont(340, 50 + Y, 23, 0, 2, 0, "宋体", m.InstrumentName.Substring(10, m.InstrumentName.Length - 10));
            TSCLIB_DLL.windowsfont(340, 90 + Y, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(340, 130 + Y, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(340, 170 + Y, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(340, 210 + Y, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode
            //TSCLIB_DLL.barcode("176", "66", "39", "130", "1", "0", "7", "12", barCode);
            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }

        private void QRCode_BAPrint(InstrumentModel m)
        {

            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            TSCLIB_DLL.sendcommand("DIRECTION 0");
            string strP = "QRCODE 530,60,H,6,A,0,M2,S1,\"" + m.BarCode + "\"";
            TSCLIB_DLL.sendcommand(strP);
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            //TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            //TSCLIB_DLL.windowsfont(340, 120, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            //TSCLIB_DLL.windowsfont(340, 160, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            //TSCLIB_DLL.windowsfont(340, 200, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            //TSCLIB_DLL.windowsfont(340, 240, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            //TSCLIB_DLL.windowsfont(340, 280, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            int Y = 0;
            string name = m.InstrumentName;
            if (m.InstrumentName.Length > 10)
            {
                Y = 40;
                name = m.InstrumentName.Substring(0, 10);
            }
            TSCLIB_DLL.windowsfont(200, 50, 23, 0, 2, 0, "宋体", "名称:" + name);
            if (m.InstrumentName.Length > 10)
                TSCLIB_DLL.windowsfont(200, 50 + Y, 23, 0, 2, 0, "宋体", m.InstrumentName.Substring(10, m.InstrumentName.Length - 10));
            TSCLIB_DLL.windowsfont(200, 90 + Y, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(200, 130 + Y, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(200, 170 + Y, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(200, 210 + Y, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode
            //TSCLIB_DLL.barcode("176", "66", "39", "130", "1", "0", "7", "12", barCode);
            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        #endregion

        #region ======条形码打印========

        /// <summary>
        /// 调用TSC打印机打印条码布局在上方
        /// </summary>
        /// <param name="m"></param>
        /// <param name="layout"></param>
        private void BarCode_ABPrint(InstrumentModel m)
        {
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode "200", "5", "128", "65", "1", "0", "2", "2"
            TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            TSCLIB_DLL.windowsfont(200, 120, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            TSCLIB_DLL.windowsfont(200, 160, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(200, 200, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(200, 240, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(200, 280, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);


            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        /// <summary>
        /// 条码布局在下方
        /// </summary>
        /// <param name="m"></param>
        /// <param name="layout"></param>
        private void BarCode_BAPrint(InstrumentModel m)
        {
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode "200", "5", "128", "65", "1", "0", "2", "2"
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title

            TSCLIB_DLL.windowsfont(200, 10, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);

            TSCLIB_DLL.windowsfont(200, 50, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(200, 90, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(200, 130, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(200, 170, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            TSCLIB_DLL.barcode("280", "220", "128", "65", "1", "0", "2", "2", m.BarCode);


            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        #endregion

        #region ======混合打印=======
        /// <summary>
        /// 条码布局在上方
        /// </summary>
        /// <param name="m"></param>
        /// <param name="layout"></param>
        private void MixCode_BAPrint(InstrumentModel m)
        {
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            TSCLIB_DLL.sendcommand("DIRECTION 0");
            string strP = "QRCODE 530,30,H,6,A,0,M2,S1,\"" + m.BarCode + "\"";
            TSCLIB_DLL.sendcommand(strP);
            TSCLIB_DLL.barcode("280", "220", "128", "65", "1", "0", "2", "2", m.BarCode);
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            //TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            //TSCLIB_DLL.windowsfont(340, 120, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            //TSCLIB_DLL.windowsfont(340, 160, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            //TSCLIB_DLL.windowsfont(340, 200, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            //TSCLIB_DLL.windowsfont(340, 240, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            //TSCLIB_DLL.windowsfont(340, 280, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            int Y = 0;
            string name = m.InstrumentName;
            if (m.InstrumentName.Length > 10)
            {
                Y = 40;
                name = m.InstrumentName.Substring(0, 10);
            }
            TSCLIB_DLL.windowsfont(200, 10, 23, 0, 2, 0, "宋体", "名称:" + name);
            if (m.InstrumentName.Length > 10)
                TSCLIB_DLL.windowsfont(200, 10 + Y, 23, 0, 2, 0, "宋体", m.InstrumentName.Substring(10, m.InstrumentName.Length - 10));
            TSCLIB_DLL.windowsfont(200, 40 + Y, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(200, 70 + Y, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(200, 100 + Y, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(200, 130 + Y, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode
            //TSCLIB_DLL.barcode("176", "66", "39", "130", "1", "0", "7", "12", barCode);
            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        /// <summary>
        /// 条码布局在下方
        /// </summary>
        /// <param name="m"></param>
        /// <param name="layout"></param>
        private void MixCode_ABPrint(InstrumentModel m)
        {
            // 设置标签 宽度、高度 等信息.
            // 宽 94mm  高 25mm
            // 速度为4
            // 字体浓度为8
            // 使用垂直間距感測器(gap sensor)
            // 两个标签之间的  间距为 3.5mm
            //TSCLIB_DLL.setup("35", "35", "1", "8", "1", "3.5", "0");
            // 清除缓冲信息
            TSCLIB_DLL.clearbuffer();
            // 发送 TSPL 指令.
            // 设置 打印的方向.
            TSCLIB_DLL.sendcommand("DIRECTION 0");
            string strP = "QRCODE 530,140,H,6,A,0,M2,S1,\"" + m.BarCode + "\"";
            TSCLIB_DLL.sendcommand(strP);
            TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            // 打印文本信息.
            // 在 (176, 16) 的坐标上
            // 字体高度为34
            // 旋转的角度为 0 度
            // 2 表示 粗体.
            // 文字没有下划线.
            // 字体为 黑体.
            // 打印的内容为：title
            //TSCLIB_DLL.barcode("280", "7", "128", "65", "1", "0", "2", "2", m.BarCode);
            //TSCLIB_DLL.windowsfont(340, 120, 23, 0, 2, 0, "宋体", "名称:" + m.InstrumentName);
            //TSCLIB_DLL.windowsfont(340, 160, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            //TSCLIB_DLL.windowsfont(340, 200, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            //TSCLIB_DLL.windowsfont(340, 240, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            //TSCLIB_DLL.windowsfont(340, 280, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            int Y = 0;
            string name = m.InstrumentName;
            if (m.InstrumentName.Length > 10)
            {
                Y = 40;
                name = m.InstrumentName.Substring(0, 10);
            }
            TSCLIB_DLL.windowsfont(200, 110, 23, 0, 2, 0, "宋体", "名称:" + name);
            if (m.InstrumentName.Length > 10)
                TSCLIB_DLL.windowsfont(200, 110 + Y, 23, 0, 2, 0, "宋体", m.InstrumentName.Substring(10, m.InstrumentName.Length - 10));
            TSCLIB_DLL.windowsfont(200, 140 + Y, 23, 0, 2, 0, "宋体", "仪器型号:" + m.Specification);
            TSCLIB_DLL.windowsfont(200, 170 + Y, 23, 0, 2, 0, "宋体", "资产编号:" + m.AssetsNo);
            TSCLIB_DLL.windowsfont(200, 200 + Y, 23, 0, 2, 0, "宋体", "出厂编号:" + m.SerialNo);
            TSCLIB_DLL.windowsfont(200, 230 + Y, 23, 0, 2, 0, "宋体", "管理编号:" + m.ManageNo);
            // 打印条码.
            // 在 (176, 66) 的坐标上
            // 以 Code39 的条码方式
            // 条码高度 130
            // 打印条码的同时，还打印条码的文本信息.
            // 旋转的角度为 0 度
            // 条码 宽 窄 比例因子为 7:12
            // 条码内容为:barCode
            //TSCLIB_DLL.barcode("176", "66", "39", "130", "1", "0", "7", "12", barCode);
            // 打印.
            TSCLIB_DLL.printlabel("1", "1");
            // 关闭 打印机 端口
            TSCLIB_DLL.closeport();
        }
        #endregion

        #endregion

        #region ===全选===
        private void gridStockItems_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridviewCheckboxHeaderCell cell = new DataGridviewCheckboxHeaderCell();
            cell.OnCheckBoxClicked += new DataGridviewCheckboxHeaderCellEventHander(cell_OnCheckBoxClicked);
            DataGridViewCheckBoxColumn checkboxCol = this.gridStockItems.Columns[0] as DataGridViewCheckBoxColumn;
            if (checkboxCol != null)
            {
                checkboxCol.HeaderCell = cell;
                checkboxCol.HeaderCell.Value = string.Empty;
            }
        }
        void cell_OnCheckBoxClicked(object sender, DataGridviewCheckboxHeaderEventHander e)
        {
            foreach (DataGridViewRow row in gridStockItems.Rows)
            {
                if (e.CheckedState)
                    row.Cells[0].Value = true;
                else
                    row.Cells[0].Value = false;
            }

        }
        private void dataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            DataGridviewCheckboxHeaderCell cell = new DataGridviewCheckboxHeaderCell();
            cell.OnCheckBoxClicked += new DataGridviewCheckboxHeaderCellEventHander(cell_OnCheckBox2Clicked);
            DataGridViewCheckBoxColumn checkboxCol = this.dataGridView1.Columns[0] as DataGridViewCheckBoxColumn;
            if (checkboxCol != null)
            {
                checkboxCol.HeaderCell = cell;
                checkboxCol.HeaderCell.Value = string.Empty;
            }
        }
        void cell_OnCheckBox2Clicked(object sender, DataGridviewCheckboxHeaderEventHander e)
        {
            foreach (DataGridViewRow row in this.dataGridView1.Rows)
            {
                if (e.CheckedState)
                    row.Cells[0].Value = true;
                else
                    row.Cells[0].Value = false;
            }

        }
        #endregion

        #region===重置===
        /// <summary>
        /// 仪器重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnInstrumentReSet_Click(object sender, EventArgs e)
        {

            dataGridView1.DataSource = Instrumentlist;
            txtInstrumentName1.Text = "";
            txtAssetsNo1.Text = "";
            txtBelongDepart1.Text = "";
            txtManageNo1.Text = "";
            txtSerialNo1.Text = "";
            txtSpecification1.Text = "";
        }
        /// <summary>
        /// 固定资产重置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            gridStockItems.DataSource = Assetslist;
            txtInstrumentName.Text = "";
            txtAssetsNo.Text = "";
            txtBelongDepart.Text = "";
            txtManageNo.Text = "";
            txtSerialNo.Text = "";
            txtSpecification.Text = "";
        }
        #endregion

        #region===查询===
        /// <summary>
        /// 固定资产查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInstrumentName_KeyUp(object sender, KeyEventArgs e)
        {
            IList<InstrumentModel> item = Assetslist;
            if (!string.IsNullOrWhiteSpace(txtInstrumentName.Text))
                item = item.Where(p => p.InstrumentName != null && p.InstrumentName.ToLower().IndexOf(txtInstrumentName.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtAssetsNo.Text))
                item = item.Where(p => p.AssetsNo != null && p.AssetsNo.ToLower().IndexOf(txtAssetsNo.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtBelongDepart.Text))
                item = item.Where(p => p.DepartName != null && p.DepartName.ToLower().IndexOf(txtBelongDepart.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtManageNo.Text))
                item = item.Where(p => p.ManageNo != null && p.ManageNo.ToLower().IndexOf(txtManageNo.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtSerialNo.Text))
                item = item.Where(p => p.SerialNo != null && p.SerialNo.ToLower().IndexOf(txtSerialNo.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtSpecification.Text))
                item = item.Where(p => p.Specification != null && p.Specification.ToLower().IndexOf(txtSpecification.Text.Trim().ToLower()) >= 0).ToList();
            gridStockItems.DataSource = item;
        }
        /// <summary>
        /// 仪器查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtInstrumentName1_KeyUp(object sender, KeyEventArgs e)
        {
            IList<InstrumentModel> item = Instrumentlist;
            if (!string.IsNullOrWhiteSpace(txtInstrumentName1.Text))
                item = item.Where(p => p.InstrumentName != null && p.InstrumentName.ToLower().IndexOf(txtInstrumentName1.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtAssetsNo1.Text))
                item = item.Where(p => p.AssetsNo != null && p.AssetsNo.ToLower().IndexOf(txtAssetsNo1.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtBelongDepart1.Text))
                item = item.Where(p => p.DepartName != null && p.DepartName.ToLower().IndexOf(txtBelongDepart1.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtManageNo1.Text))
                item = item.Where(p => p.ManageNo != null && p.ManageNo.ToLower().IndexOf(txtManageNo1.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtSerialNo1.Text))
                item = item.Where(p => p.SerialNo != null && p.SerialNo.ToLower().IndexOf(txtSerialNo1.Text.Trim().ToLower()) >= 0).ToList();
            if (!string.IsNullOrWhiteSpace(txtSpecification1.Text))
                item = item.Where(p => p.Specification != null && p.Specification.ToLower().IndexOf(txtSpecification1.Text.Trim().ToLower()) >= 0).ToList();
            dataGridView1.DataSource = item;
        }
        #endregion




    }
}
