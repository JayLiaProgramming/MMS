using MmsEngine.Popups.Support;

using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace MmsEngine.Popups
{
    [XmlRoot("MessageBox")]
    public class MessageBox : PopupBox {
        public MessageBox() => Type = "MessageBox";
    }
}
