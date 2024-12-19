using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

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
                else
                {
                    await DisplayAlert("No Files", "No files selected.", "OK");
                }
            }
            catch (Exception ex)
            {
                // Handle errors (e.g., user cancels file selection)
                await DisplayAlert("Error", $"File selection failed: {ex.Message}", "OK");
            }
        }

        private async void OnReceiveButtonClicked(object sender, EventArgs e)
        {
            // Navigate to receive page
            await Navigation.PushAsync(new ReceivePage());
        }
    }
}