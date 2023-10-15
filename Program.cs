using System.Net.NetworkInformation;
using System.Diagnostics;
using System.Net.Sockets;
using System.Net;
namespace qsign
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new Main());
        }
    }
    public static class Tool
    {
        public static string PortToPid(int port)
        {
            string output;
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd ";
                process.StartInfo.Arguments = $"/c netstat -ano | findstr \"{port}\"";
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
            }
            ///System.Diagnostics.Debug.WriteLine(output);
            string[] lines = output.Split("\n");
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            foreach (string line in lines)
            {
                string[] parts = line.Split(" ", StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length > 4)
                {
                    result.Add(new Dictionary<string, string>()
                    {
                        {
                            "pid", parts[4]
                        },
                        {
                            "address", parts[1]
                        },
                        {
                            "state", parts[3]
                        }
                    });
                }
            }
            string targetPid = "";
            foreach (var item in result)
            {
                if (item["state"] == "LISTENING")
                {
                    targetPid = item["pid"];
                    break;
                }
            }
            return targetPid;


        }
    }
}