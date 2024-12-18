using System.Net;
using System.Net.Sockets;
using System.Text;

public class FileServer
{
    private readonly int _port = 9000; // Define the port to listen on
    private readonly string _sharedDirectory = "SharedFiles"; // Directory to share files

    public void StartServer()
    {
        if (!Directory.Exists(_sharedDirectory))
        {
            Directory.CreateDirectory(_sharedDirectory);
        }

        Task.Run(() =>
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Console.WriteLine("Server started and listening...");

            while (true)
            {
                var client = listener.AcceptTcpClient();
                Task.Run(() => HandleClient(client));
            }
        });
    }

    private void HandleClient(TcpClient client)
    {
        using (var networkStream = client.GetStream())
        {
            // Read the request
            byte[] buffer = new byte[1024];
            int bytesRead = networkStream.Read(buffer, 0, buffer.Length);
            string requestedFileName = Encoding.UTF8.GetString(buffer, 0, bytesRead);

            // Check if the file exists
            string filePath = Path.Combine(_sharedDirectory, requestedFileName);
            if (File.Exists(filePath))
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                networkStream.Write(fileBytes, 0, fileBytes.Length);
            }
            else
            {
                byte[] response = Encoding.UTF8.GetBytes("File not found");
                networkStream.Write(response, 0, response.Length);
            }
        }

        client.Close();
    }
}
