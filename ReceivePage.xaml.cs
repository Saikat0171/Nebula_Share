using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class ReceivePage : ContentPage
    {
        private readonly UdpClient _udpClient;
        private const int ListeningPort = 8889; // Listening port for incoming file requests
        private TcpListener? _tcpListener; // Made nullable to avoid initialization warning

        public ReceivePage()
        {
            InitializeComponent();
            _udpClient = new UdpClient(ListeningPort);
            _tcpListener = null; // Explicitly initialize to null
            StartListeningForFileRequests();
        }

        private void StartListeningForFileRequests()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var result = await _udpClient.ReceiveAsync();
                        var requestData = Encoding.UTF8.GetString(result.Buffer);
                        var requestParts = requestData.Split('|');

                        if (requestParts.Length == 2)
                        {
                            var senderDevice = requestParts[0];
                            var fileName = requestParts[1];

                            Dispatcher.Dispatch(() =>
                            {
                                // Display the floating bar with sender info
                                SenderInfoLabel.Text = $"Sender: {senderDevice}";
                                FileNameLabel.Text = $"File: {fileName}";
                                RequestBar.IsVisible = true;
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error receiving file request: {ex.Message}");
                    }
                }
            });
        }

        private async void OnAcceptClicked(object sender, EventArgs e)
        {
            RequestBar.IsVisible = false;

            try
            {
                // Start TCP listener for file transfer
                _tcpListener = new TcpListener(IPAddress.Any, ListeningPort);
                _tcpListener.Start();

                var tcpClient = await _tcpListener.AcceptTcpClientAsync();
                using var networkStream = tcpClient.GetStream();
                var filePath = Path.Combine(FileSystem.AppDataDirectory, "received_file");

                using var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                var buffer = new byte[4096];
                int bytesRead;
                while ((bytesRead = await networkStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    await fileStream.WriteAsync(buffer, 0, bytesRead);
                }

                // Navigate to FileReceivePage
                await Navigation.PushAsync(new FileReceivePage(filePath));
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"File transfer failed: {ex.Message}", "OK");
            }
            finally
            {
                _tcpListener?.Stop();
            }
        }

        private async void OnRejectClicked(object sender, EventArgs e)
        {
            RequestBar.IsVisible = false;
            await Navigation.PopToRootAsync(); // Navigate back to MainPage
        }
    }
}