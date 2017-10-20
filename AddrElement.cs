using System;
using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    [Serializable]
    public class AddrElement
    {
        [XmlAttribute("addr")]
        public string Addr { get; set; }
    }
}
