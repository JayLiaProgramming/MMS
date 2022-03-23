using MmsEngine.Communications.Pattern;

using System;
using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace MmsEngine.Communications
{
    internal class TelnetClient : IDisposable
    {
        private TcpClient _tcpClient;
        private BlockingCollection<string> _messages;
        private Producer _producer;
        private Consumer _consumer;
        private bool _bypassReconnect;

        public EventHandler<StringArgs> DataReceivedEvent;
        public EventHandler<StatusArgs> StatusChangedEvent;

        private ConnectionState _state = ConnectionState.Disconnected;
        public ConnectionState State
        {
            get => _state;
            private set
            {
                if (_state == value) return;
                _state = value;
                StatusChangedEvent?.Invoke(this, new StatusArgs() { State = value });
                if (_state == ConnectionState.Disconnected)
                {
                    Dispose();
                    if (!_bypassReconnect && AutoReconnect)
                        Connect();
                }                
            }
        }
        public int Port { get; set; }
        public string IPAddress { get; set; }
        public bool AutoReconnect { get; set; }
        public TelnetClient() { }
        public TelnetClient(string ipAddress, int port)
        {
            IPAddress = ipAddress;
            Port = port;
            Connect();
        }

        public void Connect()
        {
            if (State != ConnectionState.Connecting)
            {
                Task.Factory.StartNew(() =>
                {
                    try
                    {
                        _messages = new BlockingCollection<string>();
                        _producer = new Producer(_messages);
                        _consumer = new Consumer(_messages);
                        _consumer.ParseMessage += MessageConsume;
                        _consumer.Process();
                        _tcpClient = new TcpClient();
                        State = ConnectionState.Connecting;
                        _tcpClient.Connect(IPAddress, Port);

                        Task.Factory.StartNew(() => { tcpStream_DataReceived(); });
                        State = _tcpClient.Client.Connected ? ConnectionState.Connected : ConnectionState.Disconnected;
                    }
                    catch (AuthenticationException)
                    {
                        _bypassReconnect = true;
                        State = ConnectionState.Disconnected;
                        _tcpClient?.Close();
                        _tcpClient?.Dispose();
                        _tcpClient = null;
                    }
                    catch (Exception)
                    {
                        _bypassReconnect = true;
                        State = ConnectionState.Disconnected;
                        _tcpClient?.Close();
                        _tcpClient?.Dispose();
                        _tcpClient = null;
                    }
                });
            }
        }
        public void Disconnect()
        {
            _bypassReconnect = true;
            if (State != ConnectionState.Disconnected)
                Dispose();
        }
        private void tcpStream_DataReceived()
        {
            if (_tcpClient == null) return;
            var stream = _tcpClient.GetStream();
            byte[] receivedBytes = new byte[1024];

            while (_tcpClient == null ? false : _tcpClient.Connected)
            {
                try
                {
                    var byte_count = stream.Read(receivedBytes, 0, receivedBytes.Length);
                    if (byte_count > 0)
                    {
                        string data = Encoding.UTF8.GetString(receivedBytes, 0, byte_count);
                        _producer.AddData(data);                        
                    }
                }
                catch (Exception)
                {
                    State = _tcpClient == null ? ConnectionState.Disconnected : _tcpClient.Connected ? ConnectionState.Connected : ConnectionState.Disconnected;
                    if (_tcpClient != null)
                        if (!_tcpClient.Connected) break;
                }
            }
        }
        private void MessageConsume(string data) => DataReceivedEvent?.Invoke(this, new StringArgs() { Value = data });
        public void SendData(string data)
        {
            try
            {
                if (State == ConnectionState.Connected)
                {
                    if (_tcpClient != null)
                    {
                        byte[] payload = Encoding.UTF8.GetBytes(data);
                        _tcpClient.GetStream().Write(payload, 0, payload.Length);
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                State = ConnectionState.Disconnected;
                Dispose();
            }
            catch { }
        }
        public void Dispose()
        {
            if (_tcpClient != null)
            {
                if (_tcpClient.Connected)
                    _tcpClient.Client.Shutdown(SocketShutdown.Both);
                _tcpClient.Close();
                State = ConnectionState.Disconnected;
                _tcpClient.Dispose();
                _tcpClient = null;
            }
            _producer = null;
            _consumer.ParseMessage -= MessageConsume;
            _consumer.Closed += () =>
            {
                if (_messages != null)
                {
                    _messages.Dispose();
                    _messages = null;
                }
                _consumer = null;
            };
            _consumer.Close();
        }
    }
}
