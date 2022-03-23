using MmsEngine.Support;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MmsEngine.Browser.Support
{
    public class BaseList : BindableBase
    {
        private int _total;
        [JsonProperty("total")]
        [XmlAttribute("total")]
        public int Total
        {
            get => _total;
            set => SetProperty(ref _total, value);
        }
        private int _start;
        [JsonProperty("start")]
        [XmlAttribute("start")]
        public int Start
        {
            get => _start;
            set => SetProperty(ref _start, value);
        }
        private bool _more;
        [JsonProperty("more")]
        [XmlAttribute("more")]
        public bool More
        {
            get => _more;
            set => SetProperty(ref _more, value);
        }
        private bool _art;
        [JsonProperty("art")]
        [XmlAttribute("art")]
        public bool Art
        {
            get => _art;
            set => SetProperty(ref _art, value);
        }
        private bool _alpha;
        [JsonProperty("alpha")]
        [XmlAttribute("alpha")]
        public bool Alpha
        {
            get => _alpha;
            set => SetProperty(ref _alpha, value);
        }
        private string _displayAs;
        [JsonProperty("displayAs")]
        [XmlAttribute("displayAs")]
        public string DisplayAs
        {
            get => _displayAs;
            set => SetProperty(ref _displayAs, value);
        }
        private string _title;
        [JsonProperty("title")]
        [XmlAttribute("caption")]
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
