using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace RATClient
{
    public class Program
    {
        #region "Declarations"
        static TcpClient sock = new TcpClient();
        static IPAddress ip = IPAddress.Parse("127.0.0.1");
        static int port = 6961;
        #endregion
        #region "Main"
        static void Main(string[] args)
        {
            string CMD = "";          
            Console.WriteLine("Remote Administration Tool");
            Console.WriteLine("Type HELP For A List Of Commands");
            Console.WriteLine("");
            while (true)
            {
                Console.Write(">");
                CMD = Console.ReadLine();
                string[] CMDS = CMD.Split(' ');
                switch (CMDS[0].ToLower())
                {
                    case "connect":
                        Connect();
                        break;
                    case "disconnect":
                        EndConnection();
                        break;
                    case "message":
                        SendCommand(CMDS);
                        break;
                    case "run":
                        SendCommand(CMDS);
                        break;
                    case "help":
                        Help();
                        break;
                    default:
                        Console.WriteLine("Invalid Command.");
                        break;
                }
                Console.WriteLine("");
            }
        }
        #endregion       
        #region "Connection Methods"
        static void EndConnection()
        {
            sock.Close();
            Console.WriteLine("RAT Client: Not Connected");
        }
        static void Connect()
        {
            ip = IPAddress.Parse("127.0.0.1");
            port = 6961;
            try
            {
                sock = new TcpClient();
                sock.Connect(ip, port);
                Console.WriteLine("RAT Client: Connected To: " + ip.ToString() + ":" + port.ToString());
            } catch (Exception e)
            {
                //if (My.Computer.Network.Ping(""))
                //{
                Console.WriteLine("Error: The server RAT is not opened.");
                Console.WriteLine("Error: An internet connection cannot be established.");
                //}
            }
        }
        static void SendCommand(string[] CMDS)
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
            Byte[] sendBytes = Encoding.ASCII.GetBytes(SerializeToXml(c));
            nstream.Write(sendBytes, 0, sendBytes.Length);
            nstream.Flush();
            byte[] message = new byte[sock.ReceiveBufferSize + 1];
            int bytesRead = 0;           
            bytesRead = nstream.Read(message, 0, Convert.ToInt32(sock.ReceiveBufferSize));
            if (bytesRead == 0) return;
            Response r = DeserializeFromXml<Response>(Encoding.ASCII.GetString(message, 0, bytesRead));
            if (r.Type == "Message") { Console.WriteLine(r.Msg); }
        }
        #endregion
        #region "Helper Methods"
        static void Help()
        {
            Console.WriteLine("CONNECT        Start A Connection");
            Console.WriteLine("MESSAGE        Send A Message To The Server");
            Console.WriteLine("DISCONNECT     End A Connection");
            Console.WriteLine("RUN            Run A Program");
            Console.WriteLine("HELP           List Commands");
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
                using (XmlTextWriter w = new XmlTextWriter(z) { Formatting = Formatting.Indented })
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