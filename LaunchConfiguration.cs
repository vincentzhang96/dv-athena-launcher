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

        public int Lver { get; set; } = 2;

        public bool UsePacking { get; set; } = true;

        public bool Standalone { get; set; } = true;
        
        public IList<LoginServerInfo> LoginServers { get; set; }

        public string CommandLineParams
        {
            get
            {
                var ips = string.Join(";", LoginServers.Select(i => i.Addr));
                var ports = string.Join(";", LoginServers.Select(i => i.Port));
                var ret = $"/ip:{ips} /port:{ports} /Lver:{this.Lver}";
                if (this.UsePacking)
                {
                    ret += " /use_packing";
                }
                if (this.Standalone)
                {
                    ret += " /stand_alone";
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
