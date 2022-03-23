using MmsEngine.NowPlaying.Support;
using MmsEngine.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;


namespace MmsEngine.NowPlaying
{
    public class Playback : BindableBase
    {
        private Dictionary<string, string> _transcribeProperty = new Dictionary<string, string>
        {
            { "TrackQueueIndex", "QueueActiveIndex" },
            { "TotalTracks", "QueueTotal" },
        };
        private readonly Dictionary<string, Action<string, string>> _specialProperty;
        public Playback(MmsPlayer player)
        {
            Player = player;
            _specialProperty = new Dictionary<string, Action<string, string>>
            {
                { "PlayState", ParsePlayState },
                { "MediaState", ParsePlayState },
                { "SkipNextAvailable", ParsePlayerCommandOption },
                { "SkipPrevAvailable", ParsePlayerCommandOption },
                { "PlayPauseAvailable", ParsePlayerCommandOption },
                { "ShuffleAvailable", ParsePlayerCommandOption },
                { "RepeatAvailable", ParsePlayerCommandOption },
                { "SeekAvailable", ParsePlayerCommandOption },
                { "ThumbsUp", ParseThumbsState },
                { "ThumbsDown", ParseThumbsState },
                { "Stars", ParseStarsState },
                { "TrackQueueIndex", ParseIntProperty },
                { "TotalTracks", ParseIntProperty }
            };
            PlayingInformation = new Information(player);
            PlayingInformation.PropertyChanged += PlayingInformation_PropertyChanged;
        }

        private void PlayingInformation_PropertyChanged(string propertyName, string jsonPropertyName, object value)
        {
            OnPropertyChanged(propertyName, $"playingInformation.{jsonPropertyName}", value);
        }
        private string _bufferingState;
        [JsonProperty("bufferingState")] public string BufferingState
        {
            get => _bufferingState;
            set => SetProperty(ref _bufferingState, value);
        }
        [JsonProperty("playingInformation")] public Information PlayingInformation { get; set; }
        private PlayerCommands _playerCommands;
        [JsonProperty("playerCommands")] public PlayerCommands PlayerCommands
        {
            get => _playerCommands;
            set => SetProperty(ref _playerCommands, value);
        }
        private TriState _thumbsUpState;
        [JsonProperty("thumbsUpState")] public TriState ThumbsUpState
        {
            get => _thumbsUpState;
            set => SetProperty(ref _thumbsUpState, value);
        }
        private TriState _thumbsDownState;
        [JsonProperty("thumbsDownState")] public TriState ThumbsDownState
        {
            get => _thumbsDownState;
            set => SetProperty(ref _thumbsDownState, value);
        }
        private StarState _starState;
        [JsonProperty("starState")] public StarState StarState
        {
            get => _starState;
            set => SetProperty(ref _starState, value);
        }
        private PlayerState _playerState;
        [JsonProperty("playerState")] public PlayerState PlayerState
        {
            get => _playerState;
            set => SetProperty(ref _playerState, value);
        }
        [JsonProperty("queueData")] public QueueData Queue { get; set; }
        private int _queueTotal;
        [JsonProperty("queueTotal")]
        public int QueueTotal
        {
            get => _queueTotal;
            set
            {
                SetProperty(ref _queueTotal, value);
                Player.Send("browsenowplaying");
            }
        }
        private int _queueActiveIndex;
        [JsonProperty("queueActiveIndex")] public int QueueActiveIndex
        {
            get => _queueActiveIndex;
            set => SetProperty(ref _queueActiveIndex, value);
        }
        public void Play() => Player.Send("Play");
        public void Stop() => Player.Send("Stop");
        public void PlayPause() => Player.Send("PlayPause");
        public void Seek(int amount)
        {
            //_player.Send("Play");
        }
        public void SkipNext() => Player.Send("SkipNext");
        public void SkipPrev() => Player.Send("SkipPrevious");
        public void ThumbsUp() => Player.Send("ThumbsUp");
        public void ThumbsDown() => Player.Send("ThumbsDown");
        public void Repeat() => Player.Send("Repeat");
        public void Shuffle() => Player.Send("Shuffle");
        internal void ParsePlayerData(string attribute, string value)
        {
            if (_specialProperty.TryGetValue(attribute, out var action))
            {
                action.Invoke(attribute, value);
                return;
            }
            
            if (_transcribeProperty.TryGetValue(attribute, out var property))
                attribute = property;

            if (GetType().GetProperty(attribute) is PropertyInfo propInfo)
                propInfo.SetValue(this, value);
            else 
                PlayingInformation.ParsePlayerData(attribute, value);
        }
        private void ParsePlayState(string attribute, string value)
        {
            if (!Enum.TryParse(value, out PlayerState playState)) return;
            PlayerState = playState;
        }
        private void ParsePlayerCommandOption(string attribute, string value)
        {
            var available = value == "True";
            if (!Enum.TryParse(attribute.Replace("Available", ""), out PlayerCommands command)) return;
            if (available)
                PlayerCommands |= command;
            else
                PlayerCommands &= ~command;
            return;
        }
        private void ParseThumbsState(string attribute, string value)
        {
            if (!Enum.TryParse(value, out TriState state)) return;
            if (attribute == "ThumbsDown")
                ThumbsDownState = state;
            else
                ThumbsUpState = state;
        }
        private void ParseStarsState(string attribute, string value)
        {
            if (!Enum.TryParse(value, out StarState state)) return;
            StarState = state;
        }
        private void ParseIntProperty(string attribute, string val)
        {
            if (!int.TryParse(val, out var value)) return;
            if (attribute.Equals("TrackQueueIndex"))
                QueueActiveIndex = value;
            else
                QueueTotal = value;
        }
        internal string InstanceName { get; set; }
    }
}
