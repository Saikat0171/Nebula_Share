using System.IO;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class FileReceivePage : ContentPage
    {
        private readonly string _filePath;

        public FileReceivePage(string filePath)
        {
            InitializeComponent();
            _filePath = filePath;
            StartFileProcessing();
        }

        private async void StartFileProcessing()
        {
            // Update the file name label
            FileNameLabel.Text = $"Received File: {Path.GetFileName(_filePath)}";

            // Simulate file receiving progress
            for (int i = 1; i <= 100; i++)
            {
                ProgressBar.Progress = i / 100.0; // Update progress
                await Task.Delay(50); // Simulate delay
            }

            // Show success message and navigate back
            await DisplayAlert("Success", "File received successfully!", "OK");
            await Navigation.PopToRootAsync();
        }
    }
}