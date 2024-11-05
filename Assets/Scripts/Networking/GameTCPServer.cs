using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using PimDeWitte.UnityMainThreadDispatcher;
using TMPro;


public class GameTCPServer : MonoBehaviour
{
    public TMP_Text displayText;  // Reference to the Text UI component
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];  // Buffer to store received data

    void Start()
    {
        
        server = new TcpListener(IPAddress.Any, 7777);
        server.Start();
        Debug.Log("Server started, waiting for client...");

        // Accept client connection asynchronously
        server.BeginAcceptTcpClient(OnClientConnected, null);
    }

    void OnClientConnected(IAsyncResult ar)
    {
        client = server.EndAcceptTcpClient(ar);
        Debug.Log("Client connected!");

        stream = client.GetStream();

        // Start reading data from the client asynchronously
        stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
    }

    void OnDataReceived(IAsyncResult ar)
    {
        int bytesRead = stream.EndRead(ar);

        if (bytesRead > 0)
        {
            // Convert the byte data into a string
            string message = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Debug.Log("Received message: " + message);

            // Update the UI on the main thread
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                displayText.text = message;
            });

            // Continue reading data
            stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
        }
    }

    void OnApplicationQuit()
    {
        if (server != null)
        {
            server.Stop();  // Stop the server on application quit
        }
    }
}

