﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.1022
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

// 
// 此源代码由 wsdl 自动生成, Version=4.0.30319.1。
// 
namespace GRGTCommonUtils.WS {
    using System;
    using System.Web.Services;
    using System.Diagnostics;
    using System.Web.Services.Protocols;
    using System.ComponentModel;
    using System.Xml.Serialization;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Web.Services.WebServiceBindingAttribute(Name="BasicHttpBinding_KnowledgeService", Namespace="http://tempuri.org/")]
    public partial class KnowledgeService : System.Web.Services.Protocols.SoapHttpClientProtocol {
        
        private System.Threading.SendOrPostCallback GetKnowledgeListForPagingOperationCompleted;
        
        private System.Threading.SendOrPostCallback GetKnowledgeDetailInfoOperationCompleted;
        
        /// <remarks/>
        public KnowledgeService() {
            this.Url = "http://localhost:8003/Ebusiness/KnowledgeService";
        }
        
        /// <remarks/>
        public event GetKnowledgeListForPagingCompletedEventHandler GetKnowledgeListForPagingCompleted;
        
        /// <remarks/>
        public event GetKnowledgeDetailInfoCompletedEventHandler GetKnowledgeDetailInfoCompleted;
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/KnowledgeService/GetKnowledgeListForPaging", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string GetKnowledgeListForPaging([System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] PagingModel paging, [System.Xml.Serialization.XmlArrayAttribute(IsNullable=true)] [System.Xml.Serialization.XmlArrayItemAttribute(Namespace="http://schemas.microsoft.com/2003/10/Serialization/Arrays")] string[] fieldCondition, int type, [System.Xml.Serialization.XmlIgnoreAttribute()] bool typeSpecified, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string accessToken) {
            object[] results = this.Invoke("GetKnowledgeListForPaging", new object[] {
                        paging,
                        fieldCondition,
                        type,
                        typeSpecified,
                        accessToken});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetKnowledgeListForPaging(PagingModel paging, string[] fieldCondition, int type, bool typeSpecified, string accessToken, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetKnowledgeListForPaging", new object[] {
                        paging,
                        fieldCondition,
                        type,
                        typeSpecified,
                        accessToken}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndGetKnowledgeListForPaging(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetKnowledgeListForPagingAsync(PagingModel paging, string[] fieldCondition, int type, bool typeSpecified, string accessToken) {
            this.GetKnowledgeListForPagingAsync(paging, fieldCondition, type, typeSpecified, accessToken, null);
        }
        
        /// <remarks/>
        public void GetKnowledgeListForPagingAsync(PagingModel paging, string[] fieldCondition, int type, bool typeSpecified, string accessToken, object userState) {
            if ((this.GetKnowledgeListForPagingOperationCompleted == null)) {
                this.GetKnowledgeListForPagingOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetKnowledgeListForPagingOperationCompleted);
            }
            this.InvokeAsync("GetKnowledgeListForPaging", new object[] {
                        paging,
                        fieldCondition,
                        type,
                        typeSpecified,
                        accessToken}, this.GetKnowledgeListForPagingOperationCompleted, userState);
        }
        
        private void OnGetKnowledgeListForPagingOperationCompleted(object arg) {
            if ((this.GetKnowledgeListForPagingCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetKnowledgeListForPagingCompleted(this, new GetKnowledgeListForPagingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        [System.Web.Services.Protocols.SoapDocumentMethodAttribute("http://tempuri.org/KnowledgeService/GetKnowledgeDetailInfo", RequestNamespace="http://tempuri.org/", ResponseNamespace="http://tempuri.org/", Use=System.Web.Services.Description.SoapBindingUse.Literal, ParameterStyle=System.Web.Services.Protocols.SoapParameterStyle.Wrapped)]
        [return: System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string GetKnowledgeDetailInfo(int knowledgeId, [System.Xml.Serialization.XmlIgnoreAttribute()] bool knowledgeIdSpecified, [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)] string accessToken) {
            object[] results = this.Invoke("GetKnowledgeDetailInfo", new object[] {
                        knowledgeId,
                        knowledgeIdSpecified,
                        accessToken});
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public System.IAsyncResult BeginGetKnowledgeDetailInfo(int knowledgeId, bool knowledgeIdSpecified, string accessToken, System.AsyncCallback callback, object asyncState) {
            return this.BeginInvoke("GetKnowledgeDetailInfo", new object[] {
                        knowledgeId,
                        knowledgeIdSpecified,
                        accessToken}, callback, asyncState);
        }
        
        /// <remarks/>
        public string EndGetKnowledgeDetailInfo(System.IAsyncResult asyncResult) {
            object[] results = this.EndInvoke(asyncResult);
            return ((string)(results[0]));
        }
        
        /// <remarks/>
        public void GetKnowledgeDetailInfoAsync(int knowledgeId, bool knowledgeIdSpecified, string accessToken) {
            this.GetKnowledgeDetailInfoAsync(knowledgeId, knowledgeIdSpecified, accessToken, null);
        }
        
        /// <remarks/>
        public void GetKnowledgeDetailInfoAsync(int knowledgeId, bool knowledgeIdSpecified, string accessToken, object userState) {
            if ((this.GetKnowledgeDetailInfoOperationCompleted == null)) {
                this.GetKnowledgeDetailInfoOperationCompleted = new System.Threading.SendOrPostCallback(this.OnGetKnowledgeDetailInfoOperationCompleted);
            }
            this.InvokeAsync("GetKnowledgeDetailInfo", new object[] {
                        knowledgeId,
                        knowledgeIdSpecified,
                        accessToken}, this.GetKnowledgeDetailInfoOperationCompleted, userState);
        }
        
        private void OnGetKnowledgeDetailInfoOperationCompleted(object arg) {
            if ((this.GetKnowledgeDetailInfoCompleted != null)) {
                System.Web.Services.Protocols.InvokeCompletedEventArgs invokeArgs = ((System.Web.Services.Protocols.InvokeCompletedEventArgs)(arg));
                this.GetKnowledgeDetailInfoCompleted(this, new GetKnowledgeDetailInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }
        
        /// <remarks/>
        public new void CancelAsync(object userState) {
            base.CancelAsync(userState);
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.SerializableAttribute()]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://schemas.datacontract.org/2004/07/Global.Common.Models.WCF")]
    public partial class PagingModel {
        
        private string fieldOrderField;
        
        private int pageCurrentField;
        
        private int pageSizeField;
        
        private int recordCountField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute(IsNullable=true)]
        public string FieldOrder {
            get {
                return this.fieldOrderField;
            }
            set {
                this.fieldOrderField = value;
            }
        }
        
        /// <remarks/>
        public int PageCurrent {
            get {
                return this.pageCurrentField;
            }
            set {
                this.pageCurrentField = value;
            }
        }
        
        /// <remarks/>
        public int PageSize {
            get {
                return this.pageSizeField;
            }
            set {
                this.pageSizeField = value;
            }
        }
        
        /// <remarks/>
        public int RecordCount {
            get {
                return this.recordCountField;
            }
            set {
                this.recordCountField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetKnowledgeListForPagingCompletedEventHandler(object sender, GetKnowledgeListForPagingCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetKnowledgeListForPagingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetKnowledgeListForPagingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    public delegate void GetKnowledgeDetailInfoCompletedEventHandler(object sender, GetKnowledgeDetailInfoCompletedEventArgs e);
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("wsdl", "4.0.30319.1")]
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    public partial class GetKnowledgeDetailInfoCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        internal GetKnowledgeDetailInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        /// <remarks/>
        public string Result {
            get {
                this.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
}
