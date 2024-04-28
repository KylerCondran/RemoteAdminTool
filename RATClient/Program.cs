﻿using System;
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
                            Console.WriteLine("RAT Client: Not Connected");
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
                    Console.WriteLine("RAT Client: Connected To: " + ip.ToString() + ":" + port.ToString() + ".");
                }
                catch
                {
                    Console.WriteLine("Error: The Server RAT Is Unreachable.");
                }
            } else
            {
                Console.WriteLine("Already Connected - Disconnect First.");
            }
        }
        static void SendCommand(string[] CMDS)
        {
            try
            {
                Command c = new Command();
                c.CMD = CMDS[0];
                int argcount = CMDS.Length - 1;
                if (argcount != 0)
                {
                    string[] args = new string[argcount];
                    for (int i = 0; i < argcount; i++) args[i] = CMDS[i + 1];
                    c.Args = args;
                }
                NetworkStream nstream = sock.GetStream();
                byte[] sendBytes = Encoding.ASCII.GetBytes(SerializeToXml(c));
                nstream.Write(sendBytes, 0, sendBytes.Length);
                nstream.Flush();
                byte[] message = new byte[sock.ReceiveBufferSize + 1];
                int bytesRead = nstream.Read(message, 0, Convert.ToInt32(sock.ReceiveBufferSize));
                if (bytesRead == 0) return;
                Response r = DeserializeFromXml<Response>(Encoding.ASCII.GetString(message, 0, bytesRead));
                if (r.Type == "Message") { Console.WriteLine(r.Msg); }
                else if (r.Type == "Data")
                {
                    try
                    {
                        using (var w = new BinaryWriter(File.OpenWrite(r.Msg))) w.Write(r.Data);
                        Console.WriteLine("File Received.");
                    }
                    catch (Exception e) { Console.WriteLine(e.Message); }   
                }
            } catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
        #endregion
        #region "Helper Methods"
        static bool ValidCommand(string CMD)
        {
            Match m = Regex.Match(CMD, "^disconnect$|^info$|^processes$|^screenshot$|^logoff$|^shutdown$|^restart$|^clipboard$|^services$|^software$|^clear$|^help$|^exit$|^connect (?:(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\.){3}(?:25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9]):([1-9][0-9]{0,3}|[1-5][0-9]{4}|6[0-4][0-9]{3}|65[0-4][0-9]{2}|655[0-2][0-9]|6553[0-5])$|^message .+$|^run .+$|^taskkill .+$|^delete (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^sendkeys .+$|^retrieve (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^search (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$|^download (https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|www\\.[a-zA-Z0-9][a-zA-Z0-9-]+[a-zA-Z0-9]\\.[^\\s]{2,}|https?:\\/\\/(?:www\\.|(?!www))[a-zA-Z0-9]+\\.[^\\s]{2,}|www\\.[a-zA-Z0-9]+\\.[^\\s]{2,}) (?<ParentPath>(?:[a-zA-Z]\\:|\\\\\\\\[\\w\\s\\.]+\\\\[\\w\\s\\.$]+)\\\\(?:[\\w\\s\\.]+\\\\)*)(?<BaseName>[\\w\\s\\.]*?)$", RegexOptions.IgnoreCase);
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
            Console.WriteLine("CLEAR          Clear The Screen");
            Console.WriteLine("CLIPBOARD      Return ClipBoard Text If Present");
            Console.WriteLine("CONNECT        Start A Connection");
            Console.WriteLine("DELETE         Delete A File On The Server");
            Console.WriteLine("DISCONNECT     End A Connection");
            Console.WriteLine("DOWNLOAD       Download A File To The Server");
            Console.WriteLine("EXIT           Quit The Program");
            Console.WriteLine("HELP           List Commands");
            Console.WriteLine("INFO           List Server Information");
            Console.WriteLine("LOGOFF         Log Off The Server");
            Console.WriteLine("MESSAGE        Send A Message To The Server");
            Console.WriteLine("PROCESSES      List Running Processes");
            Console.WriteLine("RESTART        Restart The Server");
            Console.WriteLine("RETRIEVE       Download A File From The Server");
            Console.WriteLine("RUN            Run A Program");
            Console.WriteLine("SCREENSHOT     Return A Screenshot From The Server");
            Console.WriteLine("SEARCH         List The Contents Of A Directory");
            Console.WriteLine("SENDKEYS       Send Key Presses To The Server");
            Console.WriteLine("SERVICES       List Running Services");
            Console.WriteLine("SHUTDOWN       Shutdown The Server");
            Console.WriteLine("SOFTWARE       List Installed Software");
            Console.WriteLine("TASKKILL       Kill A Running Process");
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