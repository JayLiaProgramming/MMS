using MmsEngine.Browser.Support;
using MmsEngine.Support;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MmsEngine.Browser
{
    /*
    <PickList total="15" start="1" more="false" art="false" alpha="false" displayAs="List" caption="Home Menu">
        <PickItem guid="6e6f7770-0000-0000-0000-6c6179696e67" name="Now Playing Queue" dna="name" hasChildren="1" hasArt="0" button="0" />
        ...
    </PickList> 
    */
    [XmlRoot("PickList")]
    public class BrowserData : BaseList
    {        
        private List<BrowserLine> _items;
        [JsonProperty("listItems")]
        [XmlElement("PickItem")]
        public List<BrowserLine> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
        public void SelectItem(int index) => Player.Send($"AckPickItem ${Items[index].Guid}");
    }
}
