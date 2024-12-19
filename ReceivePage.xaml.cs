using System;
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

        public ReceivePage()
        {
            _udpClient = new UdpClient();
            _udpClient.EnableBroadcast = true;

            Title = "Receiving Mode";

            Content = new StackLayout
            {
                Padding = 10,
                Children =
                {
                    new Label
                    {
                        Text = "Waiting for files...",
                        FontSize = 18,
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            };

            StartBroadcasting();
        }

        private void StartBroadcasting()
        {
            Task.Run(async () =>
            {
                var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, 8888);
                var deviceInfo = Encoding.UTF8.GetBytes("Device on " + Dns.GetHostName());

                while (true)
                {
                    await _udpClient.SendAsync(deviceInfo, deviceInfo.Length, broadcastAddress);
                    await Task.Delay(2000); // Broadcast every 2 seconds
                }
            });
        }
    }
}