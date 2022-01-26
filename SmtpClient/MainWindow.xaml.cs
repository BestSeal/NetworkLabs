using System;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Accessibility;
using MailKit.Net.Pop3;
using SmtpClient.MailHandler;


namespace SmtpClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                var loginWindow = new LoginWindow();

                if (loginWindow.ShowDialog() == true)
                {
                    FromAddr.Text = LocalStorage.Email;
                }
                else
                {
                    this.Close();
                }
            };

            Closed += (sender, args) =>
            {
                if (LocalStorage.Pop3Client.IsAuthenticated && LocalStorage.Pop3Client.IsConnected)
                {
                    LocalStorage.Pop3Client.Disconnect(true);
                    LocalStorage.Pop3Client.Dispose();
                }
            };
        }

        private async void SendMessage(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(FromAddr.Text) && !string.IsNullOrEmpty(ToAddr.Text))
            {
                var from = new MailAddress(LocalStorage.Email);
                var to = new MailAddress(ToAddr.Text);
                var message = new MailMessage(from, to);

                message.Subject = Title.Text;
                message.Body = Content.Text;
                message.IsBodyHtml = true;

                LocalStorage.SmtpClient.Credentials = new NetworkCredential(LocalStorage.Email, LocalStorage.Password);
                LocalStorage.SmtpClient.EnableSsl = true;
                await LocalStorage.SmtpClient.SendMailAsync(message);

                LocalStorage.SmtpClient.SendCompleted += (o, args) =>
                {
                    ToAddr.Text = "";
                    Content.Text = "";
                    Title.Text = "";
                };
            }
        }

        private async Task UpdateMessagesList()
        {
            if (!LocalStorage.Pop3Client.IsConnected)
            {
                await LocalStorage.Pop3Client.ConnectAsync(LocalStorage.PopAdress, LocalStorage.PopPortNum, true);
            }

            if (!LocalStorage.Pop3Client.IsAuthenticated)
            {
                await LocalStorage.Pop3Client.AuthenticateAsync(LocalStorage.Email, LocalStorage.Password);
            }


            var views = new ObservableCollection<MessageView>();

            MessagesView.ItemsSource = views;
            var max = LocalStorage.Pop3Client.Count > 10 ? 10 : LocalStorage.Pop3Client.Count;
            for (int i = 0; i < max; i++)
            {
                var message = new MessageView(await LocalStorage.Pop3Client.GetMessageAsync(i), i);
                views.Add(message);
            }
        }

        private async void UpdateMailsList(object sender, RoutedEventArgs routedEventArgs) =>
            await UpdateMessagesList();

        private void ViewItemSelected(object sender, MouseButtonEventArgs e)
        {
            var item = MessagesView.SelectedItem as MessageView;
            MessageViewer.NavigateToString(item?.HtmlBody ?? item?.Content ?? "");
        }

        private async void DeleteSelected(object sender, RoutedEventArgs e)
        {
            try
            {
                if (MessagesView.SelectedItem is MessageView messageView)
                {
                    await LocalStorage.Pop3Client.DeleteMessageAsync(messageView.Index);
                    ((ObservableCollection<MessageView>)MessagesView.ItemsSource).Remove(messageView);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}