using System;
using System.IO;
using System.Net.Sockets;
using System.Text;

public class FileClient
{
    private readonly string _serverIp;
    private readonly int _port;

    public FileClient(string serverIp, int port)
    {
        _serverIp = serverIp;
        _port = port;
    }

    public void RequestFile(string fileName, string savePath)
    {
        try
        {
            using (var client = new TcpClient(_serverIp, _port))
            using (var networkStream = client.GetStream())
            {
                // Send file name to the server
                byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                networkStream.Write(fileNameBytes, 0, fileNameBytes.Length);

                // Receive file data and save it locally
                using (var fileStream = new FileStream(savePath, FileMode.Create))
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead;
                    while ((bytesRead = networkStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error requesting file: {ex.Message}");
        }
    }
}