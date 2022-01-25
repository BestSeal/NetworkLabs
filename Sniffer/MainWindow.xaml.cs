using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using PacketDotNet;
using ProtocolType = System.Net.Sockets.ProtocolType;

namespace Sniffer
{
    public partial class MainWindow : Window
    {
        private static bool _stopReceiver;

        public MainWindow()
        {
            InitializeComponent();
        }


        private async void StartSniffer(object sender, RoutedEventArgs e)
        {
            try
            {
                WriteOutput("Sniffer starts");
                await Sniff();
            }
            catch (Exception exception)
            {
                WriteOutput(exception.Message);
            }
        }

        private async Task Sniff()
        {
            _stopReceiver = false;

            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Raw, ProtocolType.IP);
            socket.Bind(new IPEndPoint(IPAddress.Any, 80));

            var buffer = new byte[1024 * 1024 * 100];

            while (!_stopReceiver)
            {
                var bytesCount = await socket.ReceiveAsync(buffer, SocketFlags.None);
                GetPack(buffer);
                ClearBuffer(ref buffer);
            }
        }

        private void StopSniffer(object sender, RoutedEventArgs e)
        {
            WriteOutput("Sniffer stops");
            _stopReceiver = true;
        }

        private void ClearBuffer(ref byte[] buffer)
        {
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = 0;
            }
        }

        private void GetPack(byte[] rawPack)
        {
            var packet = Packet.ParsePacket(LinkLayers.Raw, rawPack);

            var tcp = packet.Extract<IPPacket>();
            if (tcp != null)
            {
                WriteOutput(
                    $"##########\n Protocol: TCP \n Version: {tcp.Version} \n Dest. addr: " +
                    $"{tcp.DestinationAddress} \n Header length: {tcp.HeaderLength} \n Source addr: {tcp.SourceAddress} " +
                    $"\n Total length: {tcp.TotalLength}, TTL: {tcp.TimeToLive} \n ##########");
                return;
            }

            var udp = packet.Extract<UdpPacket>();
            if (udp != null)
            {
                WriteOutput(
                    $"##########\n Protocol: UDP \n Checksum: {udp.Checksum}\n Length: {udp.Length}\n Dest. port: " +
                    $"{udp.DestinationPort}\nSource port: {udp.SourcePort}\n##########");
                return;
            }

            var icmp = packet.Extract<IcmpV4Packet>();
            if (icmp != null)
            {
                WriteOutput(
                    $"##########\n Protocol: ICMP \n Checksum: {icmp.Checksum}\n Data length: {icmp.Data.Length}\n\n##########");
                return;
            }

            var ip = packet.Extract<IPPacket>();
            if (ip != null)
            {
                WriteOutput(
                    $"##########\n Protocol: TCP \n Version: {ip.Version} \n Dest. addr: " +
                    $"{ip.DestinationAddress} \n Header length: {ip.HeaderLength} \n Source addr: {ip.SourceAddress} " +
                    $"\n Total length: {ip.TotalLength}, TTL: {ip.TimeToLive} \n\n##########");
            }
        }

        private void WriteOutput(string message)
        {
            Output.Text += $"{message}\n";
        }
    }
}