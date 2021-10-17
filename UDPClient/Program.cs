using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using var client = new UdpClient(4242);
            
            try
            {
                client.Connect("localhost", 6969);

                var message = "Hi, server!";
                var byteMessage = Encoding.UTF8.GetBytes(message);

                client.Send(byteMessage, byteMessage.Length);

                var endPoint = new IPEndPoint(IPAddress.Any, 4242);

                var response = client.Receive(ref endPoint);
                
                Console.WriteLine($"Server response: {Encoding.UTF8.GetString(response)}");
                
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }
    }
}