using Newtonsoft.Json;

using System.Xml.Serialization;

namespace MmsEngine.Browser.Support
{

    [XmlRoot("PickItem")]
    public class BrowserLine
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
        /// Indicates whether that menu item is a branch ( true ) or a leaf ( false )
        /// </summary>
        [JsonProperty("hasChildren")][XmlAttribute("hasChildren")] public bool HasChildren { get; set; }
        [JsonProperty("hasArt")][XmlAttribute("hasArt")] public bool HasArt { get; set; }
        /// <summary>
        /// Provides an integer value that indicates which secondary action to offer on that item.
        /// 0 Off, 1 Add, 2 Delete, 3 Play, 4 Power, 5 PowerOn, 6 Edit, 7 AllTracks, 8 ShuffleAll
        /// </summary>
        [JsonProperty("buttonId")][XmlAttribute("button")] public int ButtonId { get; set; }
        private string _artGuid;
        /// <summary>
        /// Provides the guid to use if displaying art in the browse
        /// </summary>
        [JsonProperty("artGuid")][XmlAttribute("artGuid")]
        public string ArtGuid
        {
            get => string.IsNullOrEmpty(_artGuid) ? Guid : _artGuid;
            set => _artGuid = value;
        }
        [JsonProperty("isInputBox")][XmlAttribute("singleInputBox")] public bool InputBox { get; set; }
        private string _action = string.Empty;
        [JsonProperty("action")][XmlAttribute("action")] public string Action
        {
            get => _action;
            set => _action = value.Equals("action", System.StringComparison.OrdinalIgnoreCase) ? $"action {Guid}" : value;
        }
        [JsonProperty("imageUrl")]
        public string ImageUrl => $"/getart?c=1&guid={ArtGuid}&fmt=png&.png";

        public override string ToString() => $"{Title} - {Guid}";        
    }
}
