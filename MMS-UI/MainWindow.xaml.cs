using MMS_UI.Extensions;
using MMS_UI.Websocket;
using MmsEngine;
using MmsEngine.Support;
using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace MMS_UI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly ComServer _comServer;
        private Dictionary<string, MmsPlayer> _players = new Dictionary<string, MmsPlayer>();
        private readonly MainWindowViewModel _vm = new MainWindowViewModel();        
        public MainWindow()
        {
            InitializeComponent();
            DataContext = _vm;
            _comServer = new ComServer(new Func<Dictionary<string, MmsPlayer>>(() => _players));
            //{
                //var jsonPacket = string.Empty;
                //try
                //{
                //    var updateEvent = new UpdateEventArgs { Type = UpdateEventType.Full, Params = new object[] { _players } };
                //    jsonPacket = JsonConvert.SerializeObject(updateEvent);
                //}
                //catch (Exception ex)
                //{
                //    var stop = true;
                //}
                //return jsonPacket;
            //}));
            _comServer.Start(10244);
            //34 ritz cove test
            //var mms = new MmsPlayer("192.168.68.101", "192.168.1.210", "C5-UI");
            //durfee home test
            var mms = new MmsPlayer("0.0.0.0", "192.168.68.163", "C5-UI");
            _players.Add(mms.Guid, mms);
            mms.StatusChangedEvent += (snd, e) =>
            {
                _vm.Status = e.State;
            };
            mms.DataEvent += (snd, e) =>
            {
                if (e.Value.Contains(" TrackTime=")) return;
                _vm.ConsoleData += e.Value;
                Dispatcher.Invoke(() => console.ScrollToEnd());
            };
            mms.UpdateEvent += (e) =>
            {
                var jsonPacket  = JsonConvert.SerializeObject(e);
                _comServer.SendAll(jsonPacket);
            };
            //_mms.UiMessage += _comServer.SendAll;

            //var message = "<PickList total=\"15\" start=\"1\" more=\"false\" art=\"false\" alpha=\"false\" displayAs=\"List\" caption=\"Home Menu\"><PickItem guid=\"6e6f7770-0000-0000-0000-6c6179696e67\" name=\"Now Playing Queue\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"6d796d75-0000-0000-0000-736963000000\" name=\"My Music\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"72656365-0000-0000-0000-74756e656400\" name=\"Recently Tuned\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000040000\" name=\"Amazon Music\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000002000\" name=\"Deezer\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000010000\" name=\"iHeartRadio\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000008000\" name=\"Murfie\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000000010\" name=\"Pandora Internet Radio\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000080000\" name=\"RADIO.COM\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000000008\" name=\"SiriusXM Internet Radio\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000001000\" name=\"Slacker Radio\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000100000\" name=\"SoundMachine\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000000100\" name=\"Spotify\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000004000\" name=\"TIDAL\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /><PickItem guid=\"fbbcedb1-af64-4c3f-bfe5-000000000020\" name=\"TuneIn Radio\" dna=\"name\" hasChildren=\"1\" hasArt=\"0\" button=\"0\" /></PickList>";
            //System.Diagnostics.Stopwatch _stopwatch = new System.Diagnostics.Stopwatch();
            //_stopwatch.Start();
            ////convert xml to json and pass to ui
            //var ser = new Serializer();
            //var pickList = ser.Deserialize<PickList>(message);
            //var jsonPacket = $"{{\"PickList\": {JsonConvert.SerializeObject(pickList)}}}";
            //_stopwatch.Stop();
            //var time = Mms.GetMilliSeconds(_stopwatch.ElapsedTicks);
            //Deserialize->Serialize takes ~233ms

        }

        private void TextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (!(sender is TextBox textBox)) return;
            if (e.Key == Key.Enter)
            {
                _players.Values.ToList()[0].Send(textBox.Text);
                textBox.Text = string.Empty;
            }
        }

        private void PauseProgram_Click(object sender, RoutedEventArgs e)
        {
            //_vm.ConsoleData = String.Empty;
            var stop = true;
        }
        public Image? DownloadImageFromUrl(string imageUrl)
        {
            Image? image = null;
            try
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(imageUrl);
                webRequest.AllowWriteStreamBuffering = true;
                webRequest.Timeout = 30000;

                var webResponse = webRequest.GetResponse();
                var stream = webResponse.GetResponseStream();

                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.StreamSource = stream;
                bitmap.EndInit();

                image = new Image();
                image.Source = bitmap;

                webResponse.Close();
            }
            catch (Exception)
            {
                return image;
            }

            return image;
        }
    }
}
