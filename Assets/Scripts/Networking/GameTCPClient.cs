using System.Net.Sockets;
using System.Text;
using System;
using UnityEngine;

public class GameTCPClient : MonoBehaviour
{
    private TcpClient client;
    public string networkString;  // Reference to the InputField UI component
    PlayingFieldSynch synch;

    private void Start()
    {
        synch = FindAnyObjectByType<PlayingFieldSynch>();
    }

    public void OnAttemptConnectToServer(string serverIP = "127.0.0.1") // IP defaults to localhost
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverIP, 7777);  // Blocking connect for easier debugging
            Debug.Log("Connected to server at " + serverIP);

            // Start receiving messages now that we’re connected
            ReceiveMessages();
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to connect to server: " + ex.Message);
        }
    }

    void ReceiveMessages()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogWarning("Not connected to server, cannot receive messages.");
            return;
        }

        NetworkStream stream = client.GetStream();
        byte[] data = new byte[512];

        stream.BeginRead(data, 0, data.Length, (IAsyncResult ar) =>
        {
            try
            {
                int bytesRead = stream.EndRead(ar);

                if (bytesRead == 0)
                {
                    Debug.LogWarning("Server has closed the connection.");
                    client.Close();
                    client = null;
                    return;
                }

                string message = Encoding.ASCII.GetString(data, 0, bytesRead);
                Debug.Log("Received from server: " + message);
                networkString = message;

                // Continue receiving messages
                ReceiveMessages();
            }
            catch (Exception ex)
            {
                Debug.LogError("Error receiving messages: " + ex.Message);
            }
        }, null);
    }

    public void SendMessageToServer()
    {
        if (client == null || !client.Connected)
        {
            Debug.LogWarning("Client is not connected to the server.");
            return;
        }

        try
        {
            if (string.IsNullOrEmpty(networkString))
            {
                Debug.LogWarning("No message to send. networkString is empty.");
                return;
            }

            NetworkStream stream = client.GetStream();
            if (stream.CanWrite)
            {
                byte[] data = Encoding.ASCII.GetBytes(networkString);
                stream.Write(data, 0, data.Length);
                Debug.Log("Sent to server: " + networkString);
                networkString = ""; // Clear message after sending
            }
            else
            {
                Debug.LogWarning("NetworkStream is not writable.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Error sending message to server: " + ex.Message);
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Close();
            client = null;
            Debug.Log("Client connection closed on application quit.");
        }
    }
}
