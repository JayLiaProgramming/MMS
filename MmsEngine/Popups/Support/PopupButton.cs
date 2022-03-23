using Newtonsoft.Json;
using System.Xml.Serialization;

namespace MmsEngine.Popups.Support
{
    [XmlRoot("Button")]
    public class PopupButton
    {
        [JsonProperty("text")][XmlAttribute("text")] public string Context { get; set; }
        [JsonProperty("action")][XmlAttribute("action")] public string Action { get; set; }
        [JsonProperty("default")][XmlAttribute("default")] public bool Default { get; set; }
    }
}
