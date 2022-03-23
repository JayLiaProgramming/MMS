using MmsEngine;
using MmsEngine.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;

namespace MMS_UI.Websocket
{
    public class ComServer
    {
        private WebSocketServer? _wsServer;
        private Func<Dictionary<string, MmsPlayer>> _updateCallback;
        public ComServer(Func<Dictionary<string, MmsPlayer>> updateCallback)
        {
            _updateCallback = updateCallback;
        }
        public void Start(int port, bool secure = false)
        {
            try
            {
                _wsServer = new WebSocketServer(IPAddress.Any, port, secure);
                _wsServer.AddWebSocketService("/mms", () => new UiServer(_updateCallback));
                _wsServer.Log.Level = LogLevel.Trace;
                _wsServer.Log.Output = delegate
                {
                    //Debug.Print(DebugLevel.WebSocket, "{1} {0}\rCaller:{2}\rMessage:{3}\rs:{4}", d.Level.ToString(), d.Date.ToString(), d.Caller.ToString(), d.Message, s);
                };
                _wsServer.Start();
            }
            catch (Exception)
            {
                //Debug.Print(DebugLevel.Error, "WebSocket Failed to start {0}", ex.Message);
            }
        }

        public void Stop()
        {
            if (_wsServer != null)
                _wsServer.Stop();

            _wsServer = null;
        }

        public void SendAll(string message)
        {
            //var messageData = JsonConvert.SerializeObject(message, Formatting.None, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            _wsServer?.WebSocketServices["/mms"].Sessions.Broadcast(message);
        }
    }

    public class UiServer : WebSocketBehavior
    {
        private Dictionary<string, MmsPlayer> _players;
        public UiServer(Func<Dictionary<string, MmsPlayer>> updateCallback)
        {
            _players = updateCallback?.Invoke() ?? new Dictionary<string, MmsPlayer>();
        }
        protected override void OnOpen()
        {
            try
            {
                base.OnOpen();
                var updateEvent = new UpdateEventArgs { Type = UpdateEventType.Full, Params = new object[] { _players.Values.ToList() } };
                Send(JsonConvert.SerializeObject(updateEvent));
            }
            catch (Exception)
            {
                //Debug.Print(DebugLevel.Error, "Websocket.OnOpen error {0}", ex.Message);
            }
        }
        protected override void OnMessage(MessageEventArgs e)
        {
            try
            {
                var updateEventArgs = JsonConvert.DeserializeObject<UpdateEventArgs>(e.Data);
                if (!_players.TryGetValue(updateEventArgs.Guid, out var player)) return;
                player.HandleUpdate(updateEventArgs);
            }
            catch (Exception)
            {
                //Debug.Print(DebugLevel.Error, "WebSocket.OnMessage error {0}", ex.Message);
            }
        }
        protected override void OnError(ErrorEventArgs e)
        {
            //Debug.Print(DebugLevel.Error, "WebSocket.OnError message {0}", e.Message);
        }
    }
}