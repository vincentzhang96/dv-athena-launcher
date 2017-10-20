using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace Divinitor.DN.Athena.Lib.Launcher
{
    public class Launcher
    {
        private readonly HttpClient HttpClient = new HttpClient();

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr FindWindowEx(IntPtr parentHandle, IntPtr childAfter, string className,
            string windowTitle);

        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn,
            IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        static IEnumerable<IntPtr> EnumerateProcessWindowHandles(int processId)
        {
            var handles = new List<IntPtr>();

            foreach (ProcessThread thread in Process.GetProcessById(processId).Threads)
                EnumThreadWindows(thread.Id,
                    (hWnd, lParam) =>
                    {
                        handles.Add(hWnd);
                        return true;
                    }, IntPtr.Zero);

            return handles;
        }

        public Process Launch(LaunchConfiguration launchConfig)
        {
            var proc = new Process
            {
                StartInfo =
                {
                    WorkingDirectory = Directory.GetParent(launchConfig.DragonNestExePath).FullName,
                    FileName = launchConfig.DragonNestExePath,
                    Arguments = launchConfig.CommandLineParams
                }
            };

            proc.Start();
            return proc;
        }

        public async Task<Process> WaitForGameWindowAsync(Process proc)
        {
            while (true)
            {
                try
                {
                    var handles = EnumerateProcessWindowHandles(proc.Id);
                    foreach (var handle in handles)
                    {
                        StringBuilder clsName = new StringBuilder(256);
                        var res = GetClassName(handle, clsName, clsName.Capacity);
                        if (res != 0 && "DRAGONNEST" == clsName.ToString())
                        {
                            // Small delay just in case
                            await Task.Delay(500);
                            return proc;
                        }
                    }
                }
                catch (ArgumentException)
                {
                    // Ignored
                }
                await Task.Delay(1000);
            }
        }

        public LaunchConfiguration GetLaunchConfiguration(ServerLocal server, string dnExecutablePath)
        {
            var launchConfig = new LaunchConfiguration {DragonNestExePath = dnExecutablePath};
            server.Login.ToList().ForEach(launchConfig.LoginServers.Add);
            
            return launchConfig;
        }

        public async Task<ChannelList> GetChannels(Uri patchConfigListUri)
        {
            var cfgList = await this.HttpClient.GetStringAsync(patchConfigListUri);

            var serializer = new XmlSerializer(typeof(PatchConfigList));
            using (var reader = new StringReader(cfgList))
            {
                return ((PatchConfigList) serializer.Deserialize(reader)).ChannelList.First();
            }
        }
    }
}
