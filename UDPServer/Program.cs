using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace UDPServer
{
    class Program
    {
        static void Main(string[] args)
        {
            var client = new UdpClient(6969);
            var endPoint = new IPEndPoint(IPAddress.Any, 6969);

            try
            {
                while (true)
                {
                    Console.WriteLine("Waiting for message");
                    var bytes = client.Receive(ref endPoint);

                    Console.WriteLine($"Received broadcast from {endPoint} :");
                    client.Connect(endPoint.Address, endPoint.Port);

                    var respone = Encoding.UTF8.GetBytes($"Hi, client({endPoint})");

                    client.Send(respone, respone.Length);
                }
            }
            catch (SocketException e)
            {
                Console.WriteLine(e);
            }
            finally
            {
                client.Close();
            }
        }
    }
}