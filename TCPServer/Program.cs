using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TCPBasicClientServer;

namespace TCPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                var listener = new TcpListener(IPAddress.Any, TestSettings.PORT_NUMBER);
            
                listener.Start();

                while (true)
                {
                    using var client = listener.AcceptTcpClient();
                    using var netStream = client.GetStream();

                    var byteMessage = new byte[1024];

                    netStream.Read(byteMessage, 0, byteMessage.Length);
                    
                    var encodedMessage = Encoding.UTF8.GetString(byteMessage);

                    Console.WriteLine(encodedMessage.Length > 0
                        ? $"Message received: {encodedMessage}"
                        : "Empty message received");

                    byteMessage = Encoding.UTF8.GetBytes("Hi!");
                    
                    netStream.Write(byteMessage);
                    
                    client.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}