using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace IPPackageGenerator
{
    public class ResponseObject
    {
        public Socket Socket { get; }
        
        public List<byte> RawResponse { get; }

        public byte[] RawByteResponse { get; }

        public ResponseObject(Socket socket, int bufferSize)
        {
            Socket = socket;
            RawResponse = new List<byte>(bufferSize);
            RawByteResponse = new byte[bufferSize];
        }

        public void ReceiveResponse(AsyncCallback callback)
        {
            Socket.BeginReceive(RawByteResponse, 0, RawByteResponse.Length, 0, callback, this);
        }
    }
    
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private const int bufferSize = 1024;
        private ManualResetEvent connectDone = new ManualResetEvent(false);
        private ManualResetEvent sendDone = new ManualResetEvent(false);
        private ManualResetEvent receiveDone = new ManualResetEvent(false);

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SendPackage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var host = HostNameInput.Text;
                if (int.TryParse(PortInput.Text, out var portValue))
                {
                    var ipAddress = Dns.GetHostEntry(host).AddressList.FirstOrDefault();
                    if (ipAddress == null)
                    {
                        PrintError("Не удалось создать адрес хоста");
                    }

                    var endPoint = new IPEndPoint(ipAddress, portValue);

                    using var socket = new Socket(ipAddress.AddressFamily, SocketType.Raw, ProtocolType.IP);

                    var messageToSend = Encoding.UTF8.GetBytes(MessageInput.Text);

                    socket.BeginConnect(endPoint, ConnectCallback, socket);
                    connectDone.WaitOne();

                    socket.BeginSend(messageToSend, 0, messageToSend.Length, 0, SendCallback, socket);
                    sendDone.WaitOne();

                    try
                    {
                        var responseObject = new ResponseObject(socket, bufferSize);
                        responseObject.ReceiveResponse(ReceiveCallback);
                        receiveDone.WaitOne();

                        TextOutput.Text += Encoding.UTF8.GetString(responseObject.RawResponse.ToArray());
                    }
                    catch (Exception exception)
                    {
                        PrintError(exception.Message);
                    }
                    
                    socket.Shutdown(SocketShutdown.Both);
                    socket.Close();
                }
            }
            catch (Exception exception)
            {
                PrintError(exception.Message);
            }
        }

        private void ConnectCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket socket)
                {
                    socket.EndConnect(asyncResult);
                    connectDone.Set();
                }
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }

        private void SendCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is Socket socket)
                {
                    socket.EndSend(asyncResult);
                    sendDone.Set();
                }
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }

        private void ReceiveCallback(IAsyncResult asyncResult)
        {
            try
            {
                if (asyncResult.AsyncState is ResponseObject responseObject)
                {
                    var numberOfBytes = responseObject.Socket.EndReceive(asyncResult);

                    if (numberOfBytes > 0)
                    {
                        responseObject.RawResponse.AddRange(responseObject.RawByteResponse);
                        responseObject.ReceiveResponse(ReceiveCallback);
                    }
                    else
                    {
                        receiveDone.Set();
                    }
                }
            }
            catch (Exception e)
            {
                PrintError(e.Message);
            }
        }

        private void PortInput_OnPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void PrintError(string error)
        {
            TextOutput.Text += $"{DateTime.Now}: {error}\n";
        }
    }
}