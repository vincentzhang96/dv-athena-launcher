using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

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

        public async Task<LaunchConfiguration> GetLaunchConfiguration(Uri patchConfigListUri, string dnExecutablePath)
        {
            var launchConfig = new LaunchConfiguration {DragonNestExePath = dnExecutablePath};

            var cfgList = await HttpClient.GetStringAsync(patchConfigListUri);
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(cfgList);
            // TODO This only supports NA's single server
            XmlNodeList loginServers = doc.GetElementsByTagName("login");

            for (var i = 0; i < loginServers.Count; ++i)
            {
                var server = loginServers[i];
                if (server.Attributes == null)
                {
                    continue;
                }

                launchConfig.LoginServers.Add(new LoginServerInfo()
                {
                    Addr = server.Attributes["addr"].Value,
                    Port = Convert.ToInt32(server.Attributes["port"].Value)
                });
            }

            return launchConfig;
        }
    }
}