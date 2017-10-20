using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    [Serializable]
    public class LoginServerInfo
    {
        [XmlAttribute("addr")]
        public string Addr { get; set; }

        [XmlAttribute("port")]
        public int Port { get; set; }
    }
}
