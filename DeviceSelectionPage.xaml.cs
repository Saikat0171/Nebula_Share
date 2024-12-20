using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class DeviceSelectionPage : ContentPage
    {
        private readonly UdpClient _udpClient;
        private const int BroadcastPort = 8888;
        private readonly List<string> _selectedFiles;

        public ObservableCollection<string> AvailableDevices { get; set; }

        // Constructor accepting a list of selected files
        public DeviceSelectionPage(List<string> selectedFiles)
        {
            InitializeComponent();
            _selectedFiles = selectedFiles;
            _udpClient = new UdpClient(BroadcastPort);
            AvailableDevices = new ObservableCollection<string>();
            BindingContext = this;

            StartListeningForDevices();
            StartBroadcasting(); // Start broadcasting the device info
        }

        // Method to broadcast the device info
        private void StartBroadcasting()
        {
            try
            {
                // Broadcast the device info to a specific broadcast address
                var broadcastAddress = new IPEndPoint(IPAddress.Broadcast, BroadcastPort);
                string deviceInfo = "MyDeviceName"; // Example device name
                byte[] data = Encoding.UTF8.GetBytes(deviceInfo);

                // Send the broadcast message
                _udpClient.Send(data, data.Length, broadcastAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error broadcasting device info: {ex.Message}");
            }
        }

        // Method to listen for devices
        private void StartListeningForDevices()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        var result = await _udpClient.ReceiveAsync();
                        var deviceInfo = Encoding.UTF8.GetString(result.Buffer);

                        // Add unique devices to the list
                        if (!AvailableDevices.Contains(deviceInfo))
                        {
                            Dispatcher.Dispatch(() =>
                            {
                                AvailableDevices.Add(deviceInfo);
                            });
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error receiving device info: {ex.Message}");
                    }
                }
            });
        }

        private async void OnDeviceSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is string selectedDevice)
            {
                // Navigate to FileTransferPage with the selected device
                await Navigation.PushAsync(new FileTransferPage(_selectedFiles, selectedDevice));
            }

            // Deselect the item
            ((ListView)sender).SelectedItem = null;
        }

        private void OnScanButtonClicked(object sender, EventArgs e)
        {
            // Clear the current list of devices and restart the discovery
            AvailableDevices.Clear();
            StartListeningForDevices();
            StartBroadcasting(); // Re-broadcast the device info
        }
    }
}