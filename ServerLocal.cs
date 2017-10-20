using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    [Serializable]
    public class ServerLocal
    {
        [XmlAttribute("local_name")]
        public string LocalName { get; set; }

        [XmlElement("version")]
        public AddrElement Version { get; set; }

        [XmlElement("update")]
        public AddrElement Update { get; set; }

        [XmlElement("guidepage")]
        public AddrElement Guidepage { get; set; }

        [XmlElement("homepage")]
        public AddrElement Homepage { get; set; }

        [XmlElement("login")]
        public LoginServerInfo[] Login { get; set; }
    }
}
