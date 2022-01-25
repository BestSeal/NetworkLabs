using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace IPPackGenerator
{
    public partial class MainWindow : Window
    {
        private string _ipv4Pattern = @"^(?:[0-9]{1,3}\.){3}[0-9]{1,3}$";

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void SendPackage(object sender, RoutedEventArgs e)
        {
            try
            {
                ValidateFields();
                var receiverIp = IPAddress.Parse(Ip.Text);
                var endpoint = new IPEndPoint(receiverIp, int.Parse(Port.Text));

                var data = Encoding.UTF8.GetBytes(Message.Text);

                await Task.Run(() =>
                {
                    using var socket = new Socket(SocketType.Raw, ProtocolType.IP);

                    socket.SendTo(data, SocketFlags.None, endpoint);

                    socket.Close();
                });
            }
            catch (Exception exception)
            {
                WriteOutput(exception.Message);
            }
        }

        private async void StartTestReceiver(object sender, RoutedEventArgs e)
        {
            string received = "";
            
            try
            {
                ValidateFields();
                var endpoint = (EndPoint)new IPEndPoint(IPAddress.Any, int.Parse(Port.Text));
                var testDataResult = Encoding.UTF8.GetBytes(Message.Text).Length;
                
                await Task.Run(() =>
                {
                    using var socket = new Socket(SocketType.Raw, ProtocolType.IP);
                    var buffer = new byte[1000];
                    socket.Bind(endpoint);
                    socket.ReceiveFrom(buffer, ref endpoint);
                    received = Encoding.UTF8.GetString(buffer, 20,  testDataResult);
                });
            }

            catch (Exception exception)
            {
                WriteOutput(exception.Message);
            }

            WriteOutput(received);
        }

        private void ValidateFields()
        {
            if (!Regex.IsMatch(Ip.Text, _ipv4Pattern))
            {
                throw new Exception("Invalid IP address");
            }

            if (!int.TryParse(Port.Text, out var port) || port < 0 || port > 65535)
            {
                throw new Exception("Invalid port number");
            }
        }

        private void WriteOutput(string message)
        {
            Output.Text += $"{message}\n";
        }
    }
}