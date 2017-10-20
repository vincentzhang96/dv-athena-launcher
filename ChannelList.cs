using System;
using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    [Serializable]
    public class ChannelList
    {
        [XmlAttribute("channel_name")]
        public string ChannelName { get; set; }

        [XmlElement("Local")]
        public ServerLocal[] Locals { get; set; }
    }
}
