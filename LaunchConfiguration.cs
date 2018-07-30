using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    public class LaunchConfiguration
    {
        public string DragonNestExePath { get; set; }

        [Obsolete]
        public int Lver { get; set; }

        [Obsolete]
        public bool UsePacking { get; set; }

        public bool UnusePacking { get; set; }

        [Obsolete]
        public bool Standalone { get; set; }
        
        public string Token { get; set; }

        public bool UiTest { get; set; }

        public IList<LoginServerInfo> LoginServers { get; set; }

        public string CommandLineParams
        {
            get
            {
                var ips = string.Join(";", this.LoginServers.Select(i => i.Addr));
                var ports = string.Join(";", this.LoginServers.Select(i => i.Port));
                var ret = $"/ip:{ips} /port:{ports}";
#pragma warning disable CS0612 // Type or member is obsolete
                if (this.Lver != 0)
                {
                    ret += $" /Lver:{this.Lver}";
                }
                if (this.UsePacking)
                {
                    ret += " /use_packing";
                }
                if (this.UnusePacking)
                {
                    ret += " /unuse_packing";
                }
                if (this.Standalone)
                {
                    ret += " /stand_alone";
                }
#pragma warning restore CS0612 // Type or member is obsolete
                if (!string.IsNullOrEmpty(this.Token))
                {
                    ret += $" /ttoken:{this.Token}";
                }
                if (this.UiTest)
                {
                    ret += " /uitest";
                }
                return ret;
            }
        }

        public LaunchConfiguration()
        {
            this.LoginServers = new List<LoginServerInfo>();
        }
    }
}
