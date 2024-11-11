using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameTCPServer : MonoBehaviour
{
    public string networkStringOut;
    public string networkStringIn;
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];  // Buffer to store received data
    PlayingFieldSynch synch;

    void Start()
    {
        // Initialize and start the server
        try
        {
            server = new TcpListener(IPAddress.Any, 7777);
            server.Start();
            Debug.Log("Server started, waiting for client...");

            // Begin accepting client connections asynchronously
            server.BeginAcceptTcpClient(OnClientConnected, null);
            synch = FindAnyObjectByType<PlayingFieldSynch>();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to start server: " + ex.Message);
        }
    }

    void OnClientConnected(IAsyncResult ar)
    {
        try
        {
            client = server.EndAcceptTcpClient(ar);
            Debug.Log("Client connected!");

            stream = client.GetStream();

            // Ensure the GameManager starts the game on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.StartGame(true);
            });

            // Start reading data from the client asynchronously
            Debug.Log("Beginning to read data from client...");
            stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
        }
        catch (Exception ex)
        {
            Debug.LogError("Error on client connection: " + ex.Message);
        }
    }

    void OnDataReceived(IAsyncResult ar)
    {
        try
        {
            int bytesRead = stream.EndRead(ar);

            if (bytesRead > 0)
            {
                // Convert the byte data into a string and log it
                string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
                Debug.Log("Received message from client: " + message);

                // Update the UI on the main thread
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    networkStringIn = message;
                });

                // Continue reading data from the client
                stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
            }
            else
            {
                // Client has disconnected
                Debug.LogWarning("Client has disconnected. Stopping read.");
                stream.Close();
                client.Close();
                client = null;

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    Debug.Log("Client disconnected, waiting for a new client...");
                    server.BeginAcceptTcpClient(OnClientConnected, null);
                });
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error in data reception: " + ex.Message);
            if (client != null)
            {
                client.Close();
                client = null;
            }
        }

        // Execute any synchronized device update, if needed
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            if (synch != null)
            {
                synch.RecieveSynchroniseDevices();
            }
        });
    }

    public void SendMessageToClient()
    {
        if (client != null && stream != null && stream.CanWrite)
        {
            try
            {
                string messageToSend = networkStringOut;
                byte[] data = Encoding.ASCII.GetBytes(messageToSend);

                // Write the data to the stream
                stream.Write(data, 0, data.Length);
                Debug.Log("Sent message to client: " + messageToSend);

                // Clear the network string after sending
                networkStringOut = "";
            }
            catch (Exception ex)
            {
                Debug.LogError("Error sending message to client: " + ex.Message);
            }
        }
        else
        {
            Debug.LogWarning("Client is not connected or stream is not writable.");
        }
    }

    void OnApplicationQuit()
    {
        // Properly stop the server and close client connection on quit
        if (server != null)
        {
            server.Stop();
            Debug.Log("Server stopped.");
        }

        if (client != null)
        {
            client.Close();
            Debug.Log("Client connection closed on application quit.");
        }
    }
}
