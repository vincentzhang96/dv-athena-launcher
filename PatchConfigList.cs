using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    [XmlRoot(ElementName = "document", Namespace = "")]
    public class PatchConfigList
    {
        [XmlElement]
        public ChannelList[] ChannelList { get; set; }
    }
}
