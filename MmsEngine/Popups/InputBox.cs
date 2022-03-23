using MmsEngine.Popups.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MmsEngine.Popups
{
    [XmlRoot("InputBox")]
    public class InputBox : PopupBox
    {
        public InputBox() => Type = "InputBox";
        [JsonProperty("protected")][XmlAttribute("protected")] public bool Protected { get; set; }
        [JsonProperty("value")][XmlAttribute("value")] public string Value { get; set; }
        [JsonProperty("action")][XmlAttribute("action")] public string Action { get; set; }        
        internal void GenerateButtons()
        {
            Buttons = new List<PopupButton>
            {
                new PopupButton { Action = $"{Action}OK", Context = "OK", Default = true },
                new PopupButton { Action = $"{Action}CANCEL", Context = "Cancel", Default = false },
            };
        }
    }
}
