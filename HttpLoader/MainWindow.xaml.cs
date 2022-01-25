using System;
using System.Net;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows;

namespace HttpLoader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Download(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Url.Text))
            {
                Output.Text = await DownloadPage(Url.Text);
            }
        }

        private async Task<string> DownloadPage(string url)
        {
            var client = new HttpClient();

            return await client.GetStringAsync(url);
        }
    }
}