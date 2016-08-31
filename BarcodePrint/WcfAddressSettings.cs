using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using System.IO;
using System.Collections;
using System.Configuration;

namespace BarcodePrint
{
    public partial class WcfAddressSettings : Form
    {
        string XmlFile = string.Format(@"{0}\{1}.xml", Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WCF_xmlFile"]);
        public WcfAddressSettings()
        {
            InitializeComponent();
            if (!File.Exists(XmlFile))
                return;
            XmlHelper xml = new XmlHelper();
            XmlAttribute xmlAtt = xml.GetXmlAttribute(XmlFile, "root/IPSettings", "IP");
            textBox1.Text = xmlAtt.Value;
            XmlAttribute xmlAtt_port = xml.GetXmlAttribute(XmlFile, "root/IPSettings", "Port");
            textBox2.Text = xmlAtt_port.Value;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            XmlHelper xml = new XmlHelper();
            Hashtable ht = new Hashtable();
            ht["IP"] = textBox1.Text.Trim();
            ht["Port"] = textBox2.Text.Trim();
            if (!File.Exists(XmlFile))
            {
                xml.CreateXmlDocument(XmlFile, "root", "utf-8");
                xml.InsertNode(XmlFile, "IPSettings", true, "root", ht, null);
            }
            else
                xml.UpdateNode(XmlFile, "root", ht, null);

            // Console.WriteLine("已保存Xml文档");
            Login login = new Login();
            login.Visible = true;
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Login login = new Login();
            login.Visible = true;
            this.Close();
        }
    }
}
