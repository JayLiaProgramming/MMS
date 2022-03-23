using MmsEngine.Communications;
using MmsEngine.Extensions;
using MmsEngine.NowPlaying;
using MmsEngine.Browser;
using MmsEngine.Browser.Support;
using MmsEngine.Support;

using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Reflection;
using MmsEngine.Popups;
using MmsEngine.Popups.Support;

namespace MmsEngine
{
    public class MmsPlayer : BindableBase
    {
        private TelnetClient _client;
        private bool _initialized;
        private string _receivedData;
        private const string ReportState = "ReportState";
        private const string StateChanged = "StateChanged";
        private const string XmlFlag = "<";
        private Dictionary<string, Action<string, object[]>> _frameworkActions;

        #region Public Properties
        /// <summary>
        /// Report to Autonomics the name of this control system
        /// </summary>
        [JsonProperty("clientName")]
        public string ClientName { get; set; } = "C5";
        /// <summary>
        /// IP Address of the MMS Server
        /// </summary>
        [JsonProperty("playerIpAddress")]
        public string MmsIPAddress { get; set; }
        /// <summary>
        /// IP Address of the control system.
        /// </summary>
        [JsonIgnore]
        public string HostIPAddress { get; set; }
        private MmsInstance _instanceId = MmsInstance.Main;
        /// <summary>
        /// Selects the Output of the MMS for this instance
        /// </summary>
        [JsonProperty("instanceId")]
        public MmsInstance InstanceId
        {
            get => _instanceId;
            set
            {
                if (_instanceId == value) return;
                _instanceId = value;
                Send($"SetInstance {InstanceId}");
            }
        }
        private readonly Guid _guid = System.Guid.NewGuid();
        [JsonProperty("guid")]
        public string Guid
        {
            get => _guid.ToString();
        }
        [JsonProperty("browserData")]
        public BrowserData Browser { get; set; }
        [JsonProperty("playbackData")]
        public Playback Playback { get; set; }
        [JsonProperty("playerName")]
        public string PlayerName { get => Playback?.InstanceName ?? string.Empty; }
        private string _serverName;
        [JsonProperty("serverName")]
        public string ServerName
        {
            get => _serverName;
            private set
            {
                if (!string.IsNullOrEmpty(value) && value.Contains("|"))
                    _serverName = value.Substring("|");
                else
                    _serverName = value;
            }
        }
        private bool _back;
        [JsonProperty("back")]
        public bool Back
        {
            get => _back;
            set
            {
                SetProperty(ref _back, value);
                var updateArgs = new UpdateEventArgs { Guid = Guid, Type = UpdateEventType.PropertyUpdate, PropertyName = "back", Params = new object[] { value } };
                UpdateEvent?.Invoke(updateArgs);
            }
        }
        #endregion
        public MmsPlayer()
        {
            Playback = new Playback(this);
            Playback.PropertyChanged += Playback_PropertyChanged;
            SetupFrameworkActions();

            _client = new TelnetClient();
            _client.DataReceivedEvent += Client_ReceivedData;
            _client.StatusChangedEvent += Client_StatusChanged;

        }
        public MmsPlayer(string hostIpAddress, string mmsIpAddress, string clientName, int port = 5004) : this()
        {
            HostIPAddress = hostIpAddress;
            MmsIPAddress = mmsIpAddress;
            ClientName = clientName;
            _client.IPAddress = MmsIPAddress;
            _client.Port = port;
            _client.Connect();
        }
        public void Connect()
        {
            if (string.IsNullOrEmpty(ClientName) || string.IsNullOrEmpty(HostIPAddress) || string.IsNullOrEmpty(MmsIPAddress)) return;
            _client.Connect();
        }

        private void Playback_PropertyChanged(string propertyName, string jsonPropertyName, object value)
        {
            var updateArgs = new UpdateEventArgs { Guid = Guid, Type = UpdateEventType.PropertyUpdate, PropertyName = $"playbackData.{jsonPropertyName}", Params = new object[] { value } };
            UpdateEvent?.Invoke(updateArgs);
        }
        private void Client_StatusChanged(object sender, StatusArgs e)
        {
            if (e.State == ConnectionState.Disconnected)
            {
                _initialized = false;
                _receivedData = string.Empty;
            }

            StatusChangedEvent?.Invoke(this, e);
        }        
        private void Client_ReceivedData(object sender, StringArgs e)
        {
            if (!_initialized)
            {
                var serverId = e.Value.Substring("\r\nServer=", "\n");
                Send($"SetXmlMode Lists");
                Send($"SetInstance {InstanceId}");
                Send($"SubscribeEvents True");
                Send($"SetClientType {ClientName}");
                Send($"SetClientVersion 1.0.0.0");
                Send($"SetEncoding 65001");                
                Send($"SetHost {HostIPAddress}");
                Send("SetOption supports_playnow=true");
                Send("GetStatus");
                Send("BrowseTopMenu");
                _initialized = true;
                return;
            }
            _receivedData += e.Value;
            //need to parse data
            DataEvent?.Invoke(this, e);
            while (_receivedData.Contains("\r\n"))
            {                
                var message = _receivedData.Substring(0, _receivedData.IndexOf("\r\n"));
                _receivedData = _receivedData.Remove(0, _receivedData.IndexOf("\r\n") + 2);
                var jsonPacket = string.Empty;
                if (message.StartsWith(XmlFlag)) //Check to see if its XML
                {
                    //System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
                    //_stopwatch.Start();
                    //convert xml to json and pass to ui
                    var ser = new Serializer();
                    var listType = message.Substring("<", " ");
                    UpdateEventArgs updateArgs = null;
                    try
                    {
                        switch (listType)
                        {
                            case "PickList":
                                Browser = ser.Deserialize<BrowserData>(message);
                                updateArgs = new UpdateEventArgs { Guid = Guid, Type = UpdateEventType.PropertyUpdate, PropertyName = $"browserData", Params = new object[] { Browser } };
                                Browser.Player = this;
                                break;
                            case "NowPlaying":
                                Playback.Queue = ser.Deserialize<QueueData>(message);
                                Playback.Queue.Player = this;
                                updateArgs = new UpdateEventArgs { Guid = Guid, Type = UpdateEventType.PropertyUpdate, PropertyName = $"playbackData.queueData", Params = new object[] { Playback.Queue } };
                                break;
                        }
                    }
                    catch (Exception ex)
                    {
                        var stop = true;
                        return ;
                    }
                    UpdateEvent?.Invoke(updateArgs);
                    //_stopwatch.Stop();
                    //var time = GetMilliSeconds(_stopwatch.ElapsedTicks);
                }
                else if (message.StartsWith("Instance") && Enum.TryParse(message.Substring(" ").RemoveAll("\""), out MmsInstance instance))
                    InstanceId = instance;
                else if (message.StartsWith(ReportState))
                    ParseStateMessage(ReportState, message);
                else if (message.StartsWith(StateChanged))
                    ParseStateMessage(StateChanged, message);
            }
        }
        private void ParseStateMessage(string stateType, string message)
        {
            message = message.Substring(stateType.Length + 1);
            if (!message.StartsWith(InstanceId.ToString())) return;
            var attribute = message.Substring(InstanceId.ToString().Length + 1, "=");
            var value = message.Substring("=");

            if (attribute.Equals("ui", StringComparison.OrdinalIgnoreCase) && value.StartsWith(XmlFlag))
            {
                var ser = new Serializer();
                var popupType = value.Substring("<", " ");
                object popup = null;
                try
                {
                    switch (popupType)
                    {
                        case "MessageBox":
                            popup = ser.Deserialize<MessageBox>(value);
                            break;
                        case "InputBox":
                            popup = ser.Deserialize<InputBox>(value);
                            (popup as InputBox).GenerateButtons();
                            break;
                        case "Clear":
                            popup = value.Substring("guid=\"", "\"");
                            break;
                    }
                }
                catch (Exception) {
                    return;
                }
                var updateArgs = new UpdateEventArgs { Guid = Guid, Type = UpdateEventType.MessageWindow, PropertyName = popupType, Params = new object[] { popup } };
                UpdateEvent?.Invoke(updateArgs);
            }
            else if (GetType().GetProperty(attribute) is PropertyInfo propInfo)
                propInfo.SetValue(this, value);
            else
                Playback.ParsePlayerData(attribute, value);
        }
        

        #region Framework Requirements 
        [JsonIgnore] public Action<UpdateEventArgs> UpdateEvent { get; set; }
        public bool HandleUpdate(UpdateEventArgs e)
        {
            switch (e.Type)
            {
                case UpdateEventType.Control:
                    var command = e.PropertyName;
                    if (_frameworkActions.TryGetValue(command, out var action))
                        action?.Invoke(command, e.Params);
                    else
                    {
                        if (e.Params == null)
                            Send(e.PropertyName);
                        else
                        {
                            command = $"{e.PropertyName} {string.Join(" ", e.Params)}";
                            Send(command);
                        }
                    }
                    break;
            }
            return true;
        }
        private void SetupFrameworkActions()
        {
            _frameworkActions = new Dictionary<string, Action<string, object[]>>
            {
                { "BrowserList.Back", BrowserListControl },
                { "BrowserList.Select", BrowserListControl },
                { "Popup.ButtonAction", PopupButtonAction }
            };
        }        
        private void BrowserListControl(string data, object[] p)
        {
            var command = data.Substring(".");
            if (command.Equals("Select"))
            {
                Send($"AckPickItem {p[0]}");
                _browserHistoryCount++;
            }
            else if (command.Equals("Back"))
            {
                _browserHistoryCount--;
                if (_browserHistoryCount <= 0)
                    Send("BrowseTopMenu");
                else
                    Send("Back 1");
            }
            if (_browserHistoryCount <= 0)
            {
                _browserHistoryCount = 0;
                Back = false;
            }
            else
                Back = true;
        }
        private void PopupButtonAction(string data, object[] p)
        {
            var action = p[0].ToString().Substring(" ");
            Send(action);
        }
        #endregion


        #region Debug Logic
        [JsonIgnore]        
        public EventHandler<StringArgs> DataEvent;
        [JsonIgnore]
        public EventHandler<StatusArgs> StatusChangedEvent;
        public void Send(string data) => _client.SendData($"{data}\r\n");

        private int _browserHistoryCount = 0;
        public static decimal GetMilliSeconds(long ticks)
        {
            long nanoSecondsPerTick = (1000L * 1000L * 1000L) / System.Diagnostics.Stopwatch.Frequency;
            //bool isHighRes = System.Diagnostics.Stopwatch.IsHighResolution;
            return ticks * nanoSecondsPerTick / 1000000M;
        }
        #endregion
    }
}
