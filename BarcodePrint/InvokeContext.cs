using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Channels;
using BarcodePrint;
using System.Xml;
using System.IO;
using System.Configuration;

/// <summary>
/// 动态调用WCF的工具类库
/// </summary>
public class InvokeContext
{

    #region Wcf服务工厂
    public static T CreateWCFService<T>()
    {
        string XmlFile = string.Format(@"{0}\{1}.xml", Directory.GetCurrentDirectory(), ConfigurationManager.AppSettings["WCF_xmlFile"]);
        XmlHelper xml = new XmlHelper();
        XmlAttribute xmlAtt = xml.GetXmlAttribute(XmlFile, "root/IPSettings", "IP");
        string Ip = xmlAtt.Value;
        XmlAttribute xmlAtt_port = xml.GetXmlAttribute(XmlFile, "root/IPSettings", "Port");
        string Port = xmlAtt_port.Value;
        string WCF_Suffix = ConfigurationManager.AppSettings["WCF_Suffix"];
        return CreateWCFServiceByURL<T>(string.Format("http://{0}:{1}/{2}", Ip, Port, WCF_Suffix), "basicHttpBinding");
    }
    public static T CreateWCFServiceByURL<T>(string url)
    {
        return CreateWCFServiceByURL<T>(url, "wsHttpBinding");
    }


    public static T CreateWCFServiceByURL<T>(string url, string bing)
    {
        if (string.IsNullOrEmpty(url)) throw new NotSupportedException("this url isn`t Null or Empty!");
        EndpointAddress address = new EndpointAddress(url);
        Binding binding = CreateBinding(bing);
        ChannelFactory<T> factory = new ChannelFactory<T>(binding, address);
        return factory.CreateChannel();
    }
    #endregion

    #region 创建传输协议
    /// <summary>
    /// 创建传输协议
    /// </summary>
    /// <param name="binding">传输协议名称</param>
    /// <returns></returns>
    private static Binding CreateBinding(string binding)
    {
        Binding bindinginstance = null;
        if (binding.ToLower() == "basichttpbinding")
        {
            BasicHttpBinding ws = new BasicHttpBinding();
            ws.MaxBufferSize = 2147483647;
            ws.MaxBufferPoolSize = 2147483647;
            ws.MaxReceivedMessageSize = 2147483647;
            ws.ReaderQuotas.MaxStringContentLength = 2147483647;
            ws.CloseTimeout = new TimeSpan(0, 10, 0);
            ws.OpenTimeout = new TimeSpan(0, 10, 0);
            ws.ReceiveTimeout = new TimeSpan(0, 10, 0);
            ws.SendTimeout = new TimeSpan(0, 10, 0);

            bindinginstance = ws;
        }
        else if (binding.ToLower() == "netnamedpipebinding")
        {
            NetNamedPipeBinding ws = new NetNamedPipeBinding();
            ws.MaxReceivedMessageSize = 65535000;
            bindinginstance = ws;
        }
        else if (binding.ToLower() == "netpeertcpbinding")
        {
            NetPeerTcpBinding ws = new NetPeerTcpBinding();
            ws.MaxReceivedMessageSize = 65535000;
            bindinginstance = ws;
        }
        else if (binding.ToLower() == "nettcpbinding")
        {
            NetTcpBinding ws = new NetTcpBinding();
            ws.MaxReceivedMessageSize = 65535000;
            ws.Security.Mode = SecurityMode.None;
            bindinginstance = ws;
        }
        else if (binding.ToLower() == "wsdualhttpbinding")
        {
            WSDualHttpBinding ws = new WSDualHttpBinding();
            ws.MaxReceivedMessageSize = 65535000;

            bindinginstance = ws;
        }
        else if (binding.ToLower() == "webhttpbinding")
        {
            //WebHttpBinding ws = new WebHttpBinding();
            //ws.MaxReceivedMessageSize = 65535000;
            //bindinginstance = ws;
        }
        else if (binding.ToLower() == "wsfederationhttpbinding")
        {
            WSFederationHttpBinding ws = new WSFederationHttpBinding();
            ws.MaxReceivedMessageSize = 65535000;
            bindinginstance = ws;
        }
        else if (binding.ToLower() == "wshttpbinding")
        {
            WSHttpBinding ws = new WSHttpBinding(SecurityMode.None);
            ws.MaxReceivedMessageSize = 65535000;
            ws.Security.Message.ClientCredentialType = System.ServiceModel.MessageCredentialType.Windows;
            ws.Security.Transport.ClientCredentialType = System.ServiceModel.HttpClientCredentialType.Windows;
            bindinginstance = ws;
        }
        return bindinginstance;

    }
    #endregion

}