﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18046
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Server.ReadModel.Endpoint.CqrsServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="KeyValuePairOfguidstring", Namespace="http://schemas.datacontract.org/2004/07/System.Collections.Generic")]
    [System.SerializableAttribute()]
    public partial struct KeyValuePairOfguidstring : System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        private System.Guid keyField;
        
        private string valueField;
        
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public System.Guid key {
            get {
                return this.keyField;
            }
            set {
                if ((this.keyField.Equals(value) != true)) {
                    this.keyField = value;
                    this.RaisePropertyChanged("key");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute(IsRequired=true)]
        public string value {
            get {
                return this.valueField;
            }
            set {
                if ((object.ReferenceEquals(this.valueField, value) != true)) {
                    this.valueField = value;
                    this.RaisePropertyChanged("value");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="Pong", Namespace="http://schemas.datacontract.org/2004/07/Server.Contracts")]
    [System.SerializableAttribute()]
    public partial class Pong : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="CqrsServiceReference.ICqrsService")]
    public interface ICqrsService {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICqrsService/SetName", ReplyAction="http://tempuri.org/ICqrsService/SetNameResponse")]
        void SetName(System.Guid id, string name);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ICqrsService/SetName", ReplyAction="http://tempuri.org/ICqrsService/SetNameResponse")]
        System.IAsyncResult BeginSetName(System.Guid id, string name, System.AsyncCallback callback, object asyncState);
        
        void EndSetName(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICqrsService/GetName", ReplyAction="http://tempuri.org/ICqrsService/GetNameResponse")]
        string GetName(System.Guid id);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ICqrsService/GetName", ReplyAction="http://tempuri.org/ICqrsService/GetNameResponse")]
        System.IAsyncResult BeginGetName(System.Guid id, System.AsyncCallback callback, object asyncState);
        
        string EndGetName(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICqrsService/GetList", ReplyAction="http://tempuri.org/ICqrsService/GetListResponse")]
        Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] GetList();
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ICqrsService/GetList", ReplyAction="http://tempuri.org/ICqrsService/GetListResponse")]
        System.IAsyncResult BeginGetList(System.AsyncCallback callback, object asyncState);
        
        Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] EndGetList(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICqrsService/ReloadFromEvents", ReplyAction="http://tempuri.org/ICqrsService/ReloadFromEventsResponse")]
        void ReloadFromEvents(System.Uri uri, System.DateTime lastEvent);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ICqrsService/ReloadFromEvents", ReplyAction="http://tempuri.org/ICqrsService/ReloadFromEventsResponse")]
        System.IAsyncResult BeginReloadFromEvents(System.Uri uri, System.DateTime lastEvent, System.AsyncCallback callback, object asyncState);
        
        void EndReloadFromEvents(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ICqrsService/Ping", ReplyAction="http://tempuri.org/ICqrsService/PingResponse")]
        Server.ReadModel.Endpoint.CqrsServiceReference.Pong Ping(System.Uri sender);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ICqrsService/Ping", ReplyAction="http://tempuri.org/ICqrsService/PingResponse")]
        System.IAsyncResult BeginPing(System.Uri sender, System.AsyncCallback callback, object asyncState);
        
        Server.ReadModel.Endpoint.CqrsServiceReference.Pong EndPing(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ICqrsServiceChannel : Server.ReadModel.Endpoint.CqrsServiceReference.ICqrsService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetNameCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetNameCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public string Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((string)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetListCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[])(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class PingCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public PingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public Server.ReadModel.Endpoint.CqrsServiceReference.Pong Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((Server.ReadModel.Endpoint.CqrsServiceReference.Pong)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CqrsServiceClient : System.ServiceModel.ClientBase<Server.ReadModel.Endpoint.CqrsServiceReference.ICqrsService>, Server.ReadModel.Endpoint.CqrsServiceReference.ICqrsService {
        
        private BeginOperationDelegate onBeginSetNameDelegate;
        
        private EndOperationDelegate onEndSetNameDelegate;
        
        private System.Threading.SendOrPostCallback onSetNameCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetNameDelegate;
        
        private EndOperationDelegate onEndGetNameDelegate;
        
        private System.Threading.SendOrPostCallback onGetNameCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetListDelegate;
        
        private EndOperationDelegate onEndGetListDelegate;
        
        private System.Threading.SendOrPostCallback onGetListCompletedDelegate;
        
        private BeginOperationDelegate onBeginReloadFromEventsDelegate;
        
        private EndOperationDelegate onEndReloadFromEventsDelegate;
        
        private System.Threading.SendOrPostCallback onReloadFromEventsCompletedDelegate;
        
        private BeginOperationDelegate onBeginPingDelegate;
        
        private EndOperationDelegate onEndPingDelegate;
        
        private System.Threading.SendOrPostCallback onPingCompletedDelegate;
        
        public CqrsServiceClient() {
        }
        
        public CqrsServiceClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public CqrsServiceClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CqrsServiceClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public CqrsServiceClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> SetNameCompleted;
        
        public event System.EventHandler<GetNameCompletedEventArgs> GetNameCompleted;
        
        public event System.EventHandler<GetListCompletedEventArgs> GetListCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> ReloadFromEventsCompleted;
        
        public event System.EventHandler<PingCompletedEventArgs> PingCompleted;
        
        public void SetName(System.Guid id, string name) {
            base.Channel.SetName(id, name);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginSetName(System.Guid id, string name, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginSetName(id, name, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndSetName(System.IAsyncResult result) {
            base.Channel.EndSetName(result);
        }
        
        private System.IAsyncResult OnBeginSetName(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            string name = ((string)(inValues[1]));
            return this.BeginSetName(id, name, callback, asyncState);
        }
        
        private object[] OnEndSetName(System.IAsyncResult result) {
            this.EndSetName(result);
            return null;
        }
        
        private void OnSetNameCompleted(object state) {
            if ((this.SetNameCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.SetNameCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void SetNameAsync(System.Guid id, string name) {
            this.SetNameAsync(id, name, null);
        }
        
        public void SetNameAsync(System.Guid id, string name, object userState) {
            if ((this.onBeginSetNameDelegate == null)) {
                this.onBeginSetNameDelegate = new BeginOperationDelegate(this.OnBeginSetName);
            }
            if ((this.onEndSetNameDelegate == null)) {
                this.onEndSetNameDelegate = new EndOperationDelegate(this.OnEndSetName);
            }
            if ((this.onSetNameCompletedDelegate == null)) {
                this.onSetNameCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnSetNameCompleted);
            }
            base.InvokeAsync(this.onBeginSetNameDelegate, new object[] {
                        id,
                        name}, this.onEndSetNameDelegate, this.onSetNameCompletedDelegate, userState);
        }
        
        public string GetName(System.Guid id) {
            return base.Channel.GetName(id);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetName(System.Guid id, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetName(id, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public string EndGetName(System.IAsyncResult result) {
            return base.Channel.EndGetName(result);
        }
        
        private System.IAsyncResult OnBeginGetName(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Guid id = ((System.Guid)(inValues[0]));
            return this.BeginGetName(id, callback, asyncState);
        }
        
        private object[] OnEndGetName(System.IAsyncResult result) {
            string retVal = this.EndGetName(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetNameCompleted(object state) {
            if ((this.GetNameCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetNameCompleted(this, new GetNameCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetNameAsync(System.Guid id) {
            this.GetNameAsync(id, null);
        }
        
        public void GetNameAsync(System.Guid id, object userState) {
            if ((this.onBeginGetNameDelegate == null)) {
                this.onBeginGetNameDelegate = new BeginOperationDelegate(this.OnBeginGetName);
            }
            if ((this.onEndGetNameDelegate == null)) {
                this.onEndGetNameDelegate = new EndOperationDelegate(this.OnEndGetName);
            }
            if ((this.onGetNameCompletedDelegate == null)) {
                this.onGetNameCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetNameCompleted);
            }
            base.InvokeAsync(this.onBeginGetNameDelegate, new object[] {
                        id}, this.onEndGetNameDelegate, this.onGetNameCompletedDelegate, userState);
        }
        
        public Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] GetList() {
            return base.Channel.GetList();
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginGetList(System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetList(callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] EndGetList(System.IAsyncResult result) {
            return base.Channel.EndGetList(result);
        }
        
        private System.IAsyncResult OnBeginGetList(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return this.BeginGetList(callback, asyncState);
        }
        
        private object[] OnEndGetList(System.IAsyncResult result) {
            Server.ReadModel.Endpoint.CqrsServiceReference.KeyValuePairOfguidstring[] retVal = this.EndGetList(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetListCompleted(object state) {
            if ((this.GetListCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetListCompleted(this, new GetListCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetListAsync() {
            this.GetListAsync(null);
        }
        
        public void GetListAsync(object userState) {
            if ((this.onBeginGetListDelegate == null)) {
                this.onBeginGetListDelegate = new BeginOperationDelegate(this.OnBeginGetList);
            }
            if ((this.onEndGetListDelegate == null)) {
                this.onEndGetListDelegate = new EndOperationDelegate(this.OnEndGetList);
            }
            if ((this.onGetListCompletedDelegate == null)) {
                this.onGetListCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetListCompleted);
            }
            base.InvokeAsync(this.onBeginGetListDelegate, null, this.onEndGetListDelegate, this.onGetListCompletedDelegate, userState);
        }
        
        public void ReloadFromEvents(System.Uri uri, System.DateTime lastEvent) {
            base.Channel.ReloadFromEvents(uri, lastEvent);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginReloadFromEvents(System.Uri uri, System.DateTime lastEvent, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginReloadFromEvents(uri, lastEvent, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public void EndReloadFromEvents(System.IAsyncResult result) {
            base.Channel.EndReloadFromEvents(result);
        }
        
        private System.IAsyncResult OnBeginReloadFromEvents(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Uri uri = ((System.Uri)(inValues[0]));
            System.DateTime lastEvent = ((System.DateTime)(inValues[1]));
            return this.BeginReloadFromEvents(uri, lastEvent, callback, asyncState);
        }
        
        private object[] OnEndReloadFromEvents(System.IAsyncResult result) {
            this.EndReloadFromEvents(result);
            return null;
        }
        
        private void OnReloadFromEventsCompleted(object state) {
            if ((this.ReloadFromEventsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.ReloadFromEventsCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void ReloadFromEventsAsync(System.Uri uri, System.DateTime lastEvent) {
            this.ReloadFromEventsAsync(uri, lastEvent, null);
        }
        
        public void ReloadFromEventsAsync(System.Uri uri, System.DateTime lastEvent, object userState) {
            if ((this.onBeginReloadFromEventsDelegate == null)) {
                this.onBeginReloadFromEventsDelegate = new BeginOperationDelegate(this.OnBeginReloadFromEvents);
            }
            if ((this.onEndReloadFromEventsDelegate == null)) {
                this.onEndReloadFromEventsDelegate = new EndOperationDelegate(this.OnEndReloadFromEvents);
            }
            if ((this.onReloadFromEventsCompletedDelegate == null)) {
                this.onReloadFromEventsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnReloadFromEventsCompleted);
            }
            base.InvokeAsync(this.onBeginReloadFromEventsDelegate, new object[] {
                        uri,
                        lastEvent}, this.onEndReloadFromEventsDelegate, this.onReloadFromEventsCompletedDelegate, userState);
        }
        
        public Server.ReadModel.Endpoint.CqrsServiceReference.Pong Ping(System.Uri sender) {
            return base.Channel.Ping(sender);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public System.IAsyncResult BeginPing(System.Uri sender, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginPing(sender, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        public Server.ReadModel.Endpoint.CqrsServiceReference.Pong EndPing(System.IAsyncResult result) {
            return base.Channel.EndPing(result);
        }
        
        private System.IAsyncResult OnBeginPing(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.Uri sender = ((System.Uri)(inValues[0]));
            return this.BeginPing(sender, callback, asyncState);
        }
        
        private object[] OnEndPing(System.IAsyncResult result) {
            Server.ReadModel.Endpoint.CqrsServiceReference.Pong retVal = this.EndPing(result);
            return new object[] {
                    retVal};
        }
        
        private void OnPingCompleted(object state) {
            if ((this.PingCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.PingCompleted(this, new PingCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void PingAsync(System.Uri sender) {
            this.PingAsync(sender, null);
        }
        
        public void PingAsync(System.Uri sender, object userState) {
            if ((this.onBeginPingDelegate == null)) {
                this.onBeginPingDelegate = new BeginOperationDelegate(this.OnBeginPing);
            }
            if ((this.onEndPingDelegate == null)) {
                this.onEndPingDelegate = new EndOperationDelegate(this.OnEndPing);
            }
            if ((this.onPingCompletedDelegate == null)) {
                this.onPingCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnPingCompleted);
            }
            base.InvokeAsync(this.onBeginPingDelegate, new object[] {
                        sender}, this.onEndPingDelegate, this.onPingCompletedDelegate, userState);
        }
    }
}
