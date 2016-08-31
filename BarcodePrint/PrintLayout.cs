using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BarcodePrint.Models;
using System.Resources;

namespace BarcodePrint
{
    public partial class PrintLayout : Form
    {
        //声明一个委托
        public delegate void DisplayPrint(PrintLayoutEventArgs layout);
        //声明事件
        public event DisplayPrint PrintEvent;
        public PrintLayout()
        {

            InitializeComponent();
            //switch (type)
            //{
            //    case Constants.BarCodeType.条形码:
            //        pictureBox1.Image = Image.FromFile(@"IMG\BarCode_AB.PNG");
            //        pictureBox2.Image = Image.FromFile(@"IMG\BarCode_BA.PNG");
            //        break;
            //    case Constants.BarCodeType.二维码:
            //        pictureBox1.Image = Image.FromFile(@"IMG\QRCode_AB.PNG");
            //        pictureBox2.Image = Image.FromFile(@"IMG\QRCode_BA.PNG");
            //        break;
            //    case Constants.BarCodeType.混合:
            //        pictureBox1.Image = Image.FromFile(@"IMG\Mix_AB.PNG");
            //        pictureBox2.Image = Image.FromFile(@"IMG\Mix_BA.PNG");
            //        break;

            //}
        }

        private void button1_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.DialogResult result =
                  System.Windows.Forms.MessageBox.Show(
                          "确认打印记录吗？",
                          "确定",
                          MessageBoxButtons.OKCancel,
                          MessageBoxIcon.Question);
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                Constants.BarCodeLayout layout;
                if (RB_Barcode_AB.Checked) layout = Constants.BarCodeLayout.条码上;
                else if (RB_Barcode_BA.Checked) layout = Constants.BarCodeLayout.条码下;
                else if (RB_QRcode_AB.Checked) layout = Constants.BarCodeLayout.二维码左;
                else if (RB_QRcode_BA.Checked) layout = Constants.BarCodeLayout.二维码右;
                else if (RB_Mixcode_AB.Checked) layout = Constants.BarCodeLayout.混合条码上;
                else layout = Constants.BarCodeLayout.混合条码下;
                Print(layout);
            }
        }

        public void OnPrintEvent(PrintLayoutEventArgs e)
        {
            if (PrintEvent != null)
            {
                PrintEvent(e);
            }
        }

        public void Print(Constants.BarCodeLayout Layout)
        {
            PrintLayoutEventArgs e = new PrintLayoutEventArgs();
            e.Layout = Layout;
            OnPrintEvent(e);
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
