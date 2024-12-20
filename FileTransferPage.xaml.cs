using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;

namespace Nebula_Share
{
    public partial class FileTransferPage : ContentPage
    {
        private readonly List<string> _selectedFiles;
        private readonly string _deviceName;

        // Constructor accepting selected files and device name
        public FileTransferPage(List<string> selectedFiles, string deviceName)
        {
            _selectedFiles = selectedFiles;
            _deviceName = deviceName;

            Title = "File Transfer";

            InitializeUI();
        }

        private void InitializeUI()
        {
            Content = new StackLayout
            {
                Padding = new Thickness(10),
                Children =
                {
                    new Label
                    {
                        Text = $"Sending to {_deviceName}",
                        FontSize = 20,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 20, 0, 10)
                    },
                    new ListView
                    {
                        ItemsSource = _selectedFiles,
                        ItemTemplate = new DataTemplate(() =>
                        {
                            var cell = new TextCell();
                            cell.SetBinding(TextCell.TextProperty, ".");
                            return cell;
                        })
                    },
                    new ActivityIndicator
                    {
                        IsRunning = true,
                        HorizontalOptions = LayoutOptions.Center,
                        Margin = new Thickness(0, 20, 0, 0)
                    },
                    new Label
                    {
                        Text = "Transferring files...",
                        HorizontalOptions = LayoutOptions.Center
                    }
                }
            };
        }
    }
}
