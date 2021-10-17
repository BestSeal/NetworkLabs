using System;
using System.Net.Sockets;
using System.Text;
using TCPBasicClientServer;

namespace TCPClient
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                using var client = new TcpClient("26.197.5.194", 27015);
                
                using var netStream = client.GetStream();

                var byteMessage = Encoding.UTF8.GetBytes(TestSettings.TEST_MESSAGE);

                netStream.Write(byteMessage, 0, byteMessage.Length);

                Console.WriteLine("Message sent");

                var response = new byte[1024];

                netStream.Read(response, 0, response.Length);

                var stringResponse = Encoding.UTF8.GetString(response);
                Console.WriteLine($"Server response: {stringResponse}");
                
                netStream.Close();
                client.Close();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}