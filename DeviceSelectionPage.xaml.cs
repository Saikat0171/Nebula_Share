using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class DeviceSelectionPage : ContentPage
    {
        private readonly List<string> _selectedFiles; // Selected files to send
        private readonly UdpClient _udpClient; // UDP client for device discovery
        private readonly List<string> _availableDevices; // List of discovered devices

        public DeviceSelectionPage(List<string> selectedFiles)
        {
            InitializeComponent();

            // Initialize fields
            _selectedFiles = selectedFiles;
            _udpClient = new UdpClient(8888); // Listening on port 8888
            _availableDevices = new List<string>();

            // Bind the ListView to the devices list
            DeviceListView.ItemsSource = _availableDevices;

            // Start listening for LAN devices
            StartListeningForDevices();
        }

        private async void OnScanButtonClicked(object sender, EventArgs e)
        {
            // Clear the device list and refresh
            _availableDevices.Clear();
            DeviceListView.ItemsSource = null;
            DeviceListView.ItemsSource = _availableDevices;

            // Notify user that scanning is in progress
            await DisplayAlert("Scanning", "Scanning the LAN for devices...", "OK");
        }

        private void StartListeningForDevices()
        {
            Task.Run(async () =>
            {
                while (true)
                {
                    try
                    {
                        // Wait for broadcasted messages
                        var result = await _udpClient.ReceiveAsync();
                        var deviceInfo = Encoding.UTF8.GetString(result.Buffer);

                        // Add discovered device if not already in the list
                        if (!_availableDevices.Contains(deviceInfo))
                        {
                            _availableDevices.Add(deviceInfo);

                            // Update the UI on the main thread using Dispatcher
                            await Dispatcher.DispatchAsync(() =>
                            {
                                DeviceListView.ItemsSource = null;
                                DeviceListView.ItemsSource = _availableDevices;
                            });
                        }
                    }
                    catch
                    {
                        // Handle socket errors (e.g., socket closed)
                    }
                }
            });
        }

        private async void OnDeviceSelected(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is string deviceName)
            {
                // Navigate to the File Transfer Page
                await Navigation.PushAsync(new FileTransferPage(_selectedFiles, deviceName));
            }
        }
    }
}