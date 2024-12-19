using System;
using Microsoft.Maui.Controls;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nebula_Share
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSendButtonClicked(object sender, EventArgs e)
        {
            try
            {
                // Open file picker
                var fileResults = await FilePicker.PickMultipleAsync(new PickOptions
                {
                    PickerTitle = "Select files to send",
                    FileTypes = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.iOS, new[] { "public.data" } },
                { DevicePlatform.Android, new[] { "*/*" } },
                { DevicePlatform.WinUI, new[] { "*.*" } }
            })
                });

                if (fileResults != null && fileResults.Any())
                {
                    // Pass selected files to the device selection page
                    var selectedFiles = fileResults.Select(file => file.FullPath).ToList();
                    await Navigation.PushAsync(new DeviceSelectionPage(selectedFiles));
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., user cancels file selection)
                await DisplayAlert("Error", $"File selection failed: {ex.Message}", "OK");
            }
        }


        private void OnReceiveButtonClicked(object sender, EventArgs e)
        {
            // Logic for receiving files
            DisplayAlert("Receive", "Receive functionality not implemented yet.", "OK");
        }
    }

    public class DeviceSelectionPage : ContentPage
    {
        private readonly List<string> _selectedFiles;

        public DeviceSelectionPage(List<string> selectedFiles)
        {
            _selectedFiles = selectedFiles;
            Title = "Select a Device";
            InitializeUI();
        }

        private void InitializeUI()
        {
            var deviceListView = new ListView
            {
                ItemsSource = GetAvailableDevices(), // Mock list of available devices
                ItemTemplate = new DataTemplate(() =>
                {
                    var cell = new TextCell();
                    cell.SetBinding(TextCell.TextProperty, ".");
                    return cell;
                })
            };

            deviceListView.ItemTapped += OnDeviceSelected;

            Content = new StackLayout
            {
                Padding = new Thickness(10),
                Children =
                {
                    new Label { Text = "Available Devices:", FontSize = 18, Margin = new Thickness(0, 10, 0, 10) },
                    deviceListView
                }
            };
        }

        private List<string> GetAvailableDevices()
        {
            // Mocking device discovery for now
            return new List<string> { "Device 1", "Device 2", "Device 3" };
        }

        private async void OnDeviceSelected(object? sender, ItemTappedEventArgs? e)
        {
            if (e?.Item is string deviceName)
            {
                // Simulate file sending
                await DisplayAlert("Sending", $"Sending files to {deviceName}.", "OK");
                await Navigation.PopToRootAsync();
            }
        }
    }
}