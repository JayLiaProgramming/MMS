using MmsEngine.Extensions;
using System.Collections.Generic;
using System.Xml.Serialization;


namespace MmsEngine.Browser.Support
{
    /*
     <Clients total="3" start="1" more="false" art="false" alpha="false" displayAs="List">
        <Client guid="41a7c710-d014-cb9d-d46b-532a1d2fadb0" name="C5UI@192.168.68.101:23098" you="1" />
        ...
    </Clients>
    */
    [XmlRoot("Clients")]
    public class Clients
    {
        [XmlAttribute("total")] public int Total { get; set; }
        [XmlAttribute("start")] public int Start { get; set; }
        [XmlAttribute("more")] public bool More { get; set; }
        [XmlAttribute("art")] public bool Art { get; set; }
        [XmlAttribute("alpha")] public bool Alpha { get; set; }
        [XmlAttribute("displayAs")] public string DisplayAs { get; set; }
        [XmlElement("Client")] public List<Client> ClientList { get; set; }
    }

    [XmlRoot("Client")]
    public class Client
    {
        [XmlAttribute("guid")] public string Guid { get; set; }
        [XmlAttribute("name")] public string Title { get; set; }
        [XmlAttribute("you")] public bool You { get; set; }
        public string Name => Title?.Substring(0, "@");
        public string Address => Title?.Substring("@", ":");
        public string Port => Title?.Substring(":");
    }
}
