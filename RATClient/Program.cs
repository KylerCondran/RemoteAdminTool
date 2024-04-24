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
        static TcpClient sock = new TcpClient();
        static IPAddress ip = IPAddress.Parse("127.0.0.1");
        static int port = 6961;
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
        static void Help()
        {
            Console.WriteLine("CONNECT        Start A Connection");
            Console.WriteLine("MESSAGE        Send A Message To The Server");
            Console.WriteLine("DISCONNECT     End A Connection");
            Console.WriteLine("HELP           List Commands");
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
        }
    }
    [Serializable]
    public class Command
    {
        public string CMD { get; set; }
        public string[] Args { get; set; }
    }
}