using MmsEngine.Browser.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MmsEngine.NowPlaying.Support
{
    /*
      <Title guid="32362e59-014d-b8f9-53e6-c0ec57ed4244" name="Say My Name (#1's Edit)" dna="name" artGuid="408ed461-21cd-f82a-ccd7-53c3ca283213"
             isSearchable="false" npIndex="1" button="2" duration="00:04:00" track="8" albumGuid="c4b1265e-e74a-e0a8-6f78-06a2519a7abd"
             album="#1's" albumUnique="#1's (Destiny's Child)" artist="Destiny's Child" host="" />
    */
    [XmlRoot("Title")]
    public class QueueLine
    {
        /// <summary>
        /// Provides the item’s ID for use in any action
        /// </summary>
        [JsonProperty("guid")][XmlAttribute("guid")] public string Guid { get; set; }
        /// <summary>
        /// Provides the title label of the item
        /// </summary>
        [JsonProperty("title")][XmlAttribute("name")] public string Title { get; set; }
        /// <summary>
        /// Provides the name of the attribute containing the value to use for display
        /// </summary>
        [JsonProperty("dna")][XmlAttribute("dna")] public string Dna { get; set; }
        /// <summary>
        /// Index location of the Now Playing list
        /// </summary>
        [JsonProperty("npIndex")][XmlAttribute("npIndex")] public int NowPlayingIndex { get; set; }
        /// <summary>
        /// Indicates whether that menu item is a branch ( true ) or a leaf ( false )
        /// </summary>        
        [JsonProperty("isSearchable")][XmlAttribute("isSearchable")] public bool IsSearchable { get; set; }
        /// <summary>
        /// Duration of the song
        /// </summary>
        [JsonProperty("duration")][XmlAttribute("duration")] public string Duration { get; set; }
        /// <summary>
        /// Track index on the album
        /// </summary>
        [JsonProperty("track")][XmlAttribute("track")] public int Track { get; set; }
        /// <summary>
        /// Provides an integer value that indicates which secondary action to offer on that item.
        /// 0 Off, 1 Add, 2 Delete, 3 Play, 4 Power, 5 PowerOn, 6 Edit, 7 AllTracks, 8 ShuffleAll
        /// </summary>
        [JsonProperty("buttonId")][XmlAttribute("button")] public int ButtonId { get; set; }
        private string _artGuid;
        /// <summary>
        /// Provides the guid to use if displaying art in the browse
        /// </summary>
        [JsonProperty("artGuid")]
        [XmlAttribute("artGuid")]
        public string ArtGuid
        {
            get => string.IsNullOrEmpty(_artGuid) ? Guid : _artGuid;
            set => _artGuid = value;
        }
        /// <summary>
        /// Provides the album’s ID for use in any action
        /// </summary>
        [JsonProperty("albumGuid")][XmlAttribute("albumGuid")] public string AlbumGuid { get; set; }
        /// <summary>
        /// Album's title
        /// </summary>
        [JsonProperty("album")][XmlAttribute("album")] public string Album { get; set; }
        /// <summary>
        /// Album's title combined with Artist info
        /// </summary>
        [JsonProperty("albumUnique")][XmlAttribute("albumUnique")] public string AlbumUnique { get; set; }
        /// <summary>
        /// Artist's name
        /// </summary>
        [JsonProperty("artist")][XmlAttribute("artist")] public string Artist { get; set; }
        /// <summary>
        /// Host information...not sure, its undocumented
        /// </summary>
        [JsonProperty("host")][XmlAttribute("host")] public string Host { get; set; }
        private string _action;
        [JsonProperty("action")]
        [XmlAttribute("action")]
        public string Action
        {
            get => _action;
            set => _action = value.Equals("action", System.StringComparison.OrdinalIgnoreCase) ? $"action {Guid}" : value;
        }
        [JsonProperty("imageUrl")]
        public string ImageUrl => $"/getart?c=1&guid={ArtGuid}&fmt=png&.png";

        public override string ToString() => $"{Title} - {Guid}";
    }
}
