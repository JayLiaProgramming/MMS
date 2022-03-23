using MmsEngine.Popups.Support;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MmsEngine.Popups
{
    public class PopupBox
    {
        [JsonProperty("type")] public string Type { get; set; }
        [JsonProperty("guid")][XmlAttribute("guid")] public string Guid { get; set; }
        [JsonProperty("title")][XmlAttribute("caption")] public string Title { get; set; }
        [JsonProperty("message")][XmlAttribute("message")] public string Message { get; set; }
        [JsonProperty("messageShort")][XmlAttribute("messageShort")] public string MessageShort { get; set; }
        [JsonProperty("timeOut")][XmlAttribute("timeOut")] public int TimeOut { get; set; }
        [JsonProperty("uniqueId")][XmlAttribute("uniqueid")] public int UniqueId { get; set; }
        [JsonProperty("buttons")] public List<PopupButton> Buttons { get; set; }
    }
}
