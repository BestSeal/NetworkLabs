using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FtpViewer.Views;

namespace FtpViewer
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

        private async void OnPathOpen(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Path.Text))
            {
                ArchiverViewer.ItemsSource = await LoadFiles(Path.Text);
            }
        }

        private async Task<List<FileView>> LoadFiles(string path)
        {
            var request = WebRequest.Create(path);
            request.Method = WebRequestMethods.Ftp.ListDirectory;
            var response = (FtpWebResponse)await request.GetResponseAsync();
            var stream = new StreamReader(response.GetResponseStream());

            var views = new List<FileView>();
            var outString = await stream.ReadLineAsync();
            while (!string.IsNullOrEmpty(outString))
            {
                views.Add(new FileView(outString));
                outString = await stream.ReadLineAsync();
            }

            return views;
        }

        private async void OpenFolder(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 2)
            {
                if (!string.IsNullOrEmpty(Path.Text))
                {
                    foreach (FileView item in ArchiverViewer.ItemsSource)
                    {
                        if (item != null && item.FileName.Equals(((TextBlock)sender).Text))
                        {
                            var files = await LoadFiles(Path.Text + "/" + ((TextBlock)sender).Text);

                            if (files?.Any() != true) return;

                            foreach (var file in files)
                            {
                                item.FileViews.Add(file);
                            }

                            break;
                        }
                    }
                }
            }
            else if (e.LeftButton == MouseButtonState.Pressed && e.ClickCount == 1)
            {
                FileToLoad.Text = ((TextBlock)sender).Text;
            }
        }

        private async void LoadFile(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(Path.Text) && !string.IsNullOrEmpty(FileToLoad.Text))
            {
                await DownloadFile(Path.Text + "/" + FileToLoad.Text);
                ResetProgressBar();
            }
        }

        private async Task DownloadFile(string path)
        {
            try
            {
                var request = WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.GetFileSize;
                var length = (await request.GetResponseAsync()).ContentLength;

                request = WebRequest.Create(path);
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                var response = (FtpWebResponse)await request.GetResponseAsync();

                using var stream = new StreamReader(response.GetResponseStream());

                await using var fileStream = File.Create(DownloadPath.Text + System.IO.Path.DirectorySeparatorChar + FileToLoad.Text);
                var buffer = new byte[1024];
                for (long i = 0; i < length / buffer.Length + 1; i++)
                {
                    await LoadFile(buffer, i, length, stream.BaseStream, fileStream);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private void ResetProgressBar()
        {
            DownloadProgress.Value = 0;
        }

        private void UpdateProgressBar(double delta)
        {
            DownloadProgress.Value += delta;
        }

        private async Task LoadFile(byte[] buffer,long currentProgress, long max, Stream inStream, FileStream outStream)
        {
            await inStream.ReadAsync(buffer); 
            await outStream.WriteAsync(buffer);
            UpdateProgressBar( currentProgress / ( (double) max / buffer.Length));
        }
    }
}