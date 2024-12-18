using System;
using System.IO;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class MainPage : ContentPage
    {
        private FileClient _fileClient;

        public MainPage()
        {
            InitializeComponent();

            // Initialize FileClient with server IP and port
            _fileClient = new FileClient("192.168.1.2", 9000); // Replace with your server's IP and port
        }

        private void InitializeComponent()
        {
            throw new NotImplementedException();
        }

        private async void OnRequestFileClicked(object sender, EventArgs e)
        {
            // Get the file name from the Entry
            string fileName = FileNameEntry.Text;


            if (string.IsNullOrWhiteSpace(fileName))
            {
                await DisplayAlert("Error", "Please enter a valid file name.", "OK");
                return;
            }

            string savePath = Path.Combine(FileSystem.AppDataDirectory, fileName);

            try
            {
                // Request file from the server
                _fileClient.RequestFile(fileName, savePath);
                await DisplayAlert("Success", $"File saved at {savePath}", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to download file: {ex.Message}", "OK");
            }
        }
    }
}