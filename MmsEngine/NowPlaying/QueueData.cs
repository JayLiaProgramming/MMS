using MmsEngine.Browser.Support;
using MmsEngine.NowPlaying.Support;
using MmsEngine.Support;
using Newtonsoft.Json;

using System.Collections.Generic;
using System.Xml.Serialization;

namespace MmsEngine.NowPlaying
{
    /*
    <NowPlaying total="24" start="1" more="false" art="false" alpha="false" displayAs="List" caption="Queue not currently in use" np="1" queueInUse="0">
	    <Title guid="32362e59-014d-b8f9-53e6-c0ec57ed4244" name="Say My Name (#1's Edit)" dna="name" artGuid="408ed461-21cd-f82a-ccd7-53c3ca283213"
               isSearchable="false" npIndex="1" button="2" duration="00:04:00" track="8" albumGuid="c4b1265e-e74a-e0a8-6f78-06a2519a7abd"
               album="#1's" albumUnique="#1's (Destiny's Child)" artist="Destiny's Child" host="" />
        ...
    </NowPlaying>
     */
    [XmlRoot("NowPlaying")]
    public class QueueData : BaseList
    {
        private int _nowPlaying;
        [JsonProperty("np")]
        [XmlAttribute("np")]
        public int NowPlaying
        {
            get => _nowPlaying;
            set => SetProperty(ref _nowPlaying, value);
        }
        private int _inUse;
        [JsonProperty("inUse")]
        [XmlAttribute("queueInUse")]
        public int InUse
        {
            get => _inUse;
            set => SetProperty(ref _inUse, value);
        }
        private List<QueueLine> _items;
        [JsonProperty("listItems")]
        [XmlElement("Title")]
        public List<QueueLine> Items
        {
            get => _items;
            set => SetProperty(ref _items, value);
        }
    }
}
