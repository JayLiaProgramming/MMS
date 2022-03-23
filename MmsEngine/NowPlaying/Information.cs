using MmsEngine.Extensions;
using MmsEngine.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace MmsEngine.NowPlaying
{
    public class Information : BindableBase
    {
        private Dictionary<string, string> _transcribeProperty = new Dictionary<string, string>
        {
            { "ArtistName", "Artist" },
            { "MediaName", "Album" },
            { "TrackName", "Track" },
            { "NowPlayingSrce", "Provider" },            
            { "NowPlayingType", "Type" },
            { "MetaData1", string.Empty },
            { "MetaData2", string.Empty },
            { "MetaData3", string.Empty },
            { "MetaData4", string.Empty },
        };
        public Information(MmsPlayer player)
        {
            Player = player;
        }
        private string _provider;
        [JsonProperty("provider")] public string Provider
        {
            get => _provider;
            set
            {
                SetProperty(ref _provider, value);
                ProviderImageUrl = $"http://{Player.MmsIPAddress}/getart?c=1&guid={value}badge&fmt=png&.png";
            }
        }
        private string _station;
        [JsonProperty("station")] public string Station
        {
            get => _station;
            set => SetProperty(ref _station, value);                
        }
        private string _providerImageUrl;
        [JsonProperty("providerImageUrl")] public string ProviderImageUrl
        {
            get => _providerImageUrl;
            set => SetProperty(ref _providerImageUrl, value);
        }
        private string _title;
        [JsonProperty("title")] public string Title
        {
            get => _title;
            private set => SetProperty(ref _title, value);
        }
        private string _artist;
        [JsonProperty("artist")] public string Artist
        {
            get => _artist;
            private set => SetProperty(ref _artist, value);
        }
        private string _album;
        [JsonProperty("album")] public string Album
        {
            get => _album;
            private set => SetProperty(ref _album, value);
        }
        private string _track;
        [JsonProperty("track")] public string Track
        {
            get => _track;
            private set => SetProperty(ref _track, value);
        }
        private string _type;
        [JsonProperty("type")] public string Type
        {
            get => _type;
            private set => SetProperty(ref _type, value);
        }
        private string _artUrl;
        [JsonProperty("artUrl")] public string ArtUrl
        {
            get => _artUrl;
            private set => SetProperty(ref _artUrl, value);
        }
        private string _radioFrequency;
        [JsonProperty("radioFrequency")] public string RadioFrequency
        {
            get => _radioFrequency;
            private set => SetProperty(ref _radioFrequency, value);
        }
        private bool _trackDurationAvailable;
        [JsonProperty("trackDurationAvailable")] public bool TrackDurationAvailable
        {
            get => _trackDurationAvailable;
            private set => SetProperty(ref _trackDurationAvailable, value);
        }

        private int _trackDuration;
        [JsonProperty("trackDuration")] public int TrackDuration
        {
            get => _trackDuration;
            set
            {
                SetProperty(ref _trackDuration, value);
                TrackDurationAvailable = _trackDuration > 0;
            }
        }
        private int _trackProgress;
        [JsonProperty("trackProgress")] public int TrackProgress
        {
            get => _trackProgress;
            set => SetProperty(ref _trackProgress, value);
        }

        internal void ParsePlayerData(string attribute, string value)
        {
            switch (attribute)
            {
                case "NowPlayingGuid":
                    value = value.Substring(1, value.Length - 2);
                    ArtUrl = $"http://{Player.MmsIPAddress}/getart?c=1&guid={value}&rfle=0&rflh=30&rflo=70&rz=10&fmt=png&.png";
                    return;
                case "TrackTime":
                    TrackProgress = int.Parse(value);
                    return;
                case "MediaTime":
                    TrackDuration = int.Parse(value);
                    return;
                default:
                    if (!attribute.StartsWith("MetaLabel")) break;
                    _transcribeProperty[attribute.Replace("Label", "Data")] = value;
                    break;
            }
            if (_transcribeProperty.TryGetValue(attribute, out var property))
            {
                if (property == string.Empty)
                {
                    Title = value;
                    return;
                }
                attribute = property;
            }

            if (GetType().GetProperty(attribute) is PropertyInfo propInfo)
                propInfo.SetValue(this, value);
        }
    }
}
