using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;


public class GameTCPServer : MonoBehaviour
{
    public string networkString;  // Reference to the Text UI component
    private TcpListener server;
    private TcpClient client;
    private NetworkStream stream;
    private byte[] buffer = new byte[1024];  // Buffer to store received data
    PlayingFieldSynch synch;

    void Start()
    {

        server = new TcpListener(IPAddress.Any, 7777);
        server.Start();
        Debug.Log("Server started, waiting for client...");

        // Accept client connection asynchronously
        server.BeginAcceptTcpClient(OnClientConnected, null);
        synch = FindAnyObjectByType<PlayingFieldSynch>();
    }

    void OnClientConnected(IAsyncResult ar)
    {
        client = server.EndAcceptTcpClient(ar);
        Debug.Log("Client connected!");

        stream = client.GetStream();

        //Keenan addition:
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.StartGame(true);
        });
        //End

        // Start reading data from the client asynchronously
        stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);

        //Keenan addition:
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            GameManager.Instance.StartGame(true);
        });
        //End
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
                networkString = message;
            });

            // Continue reading data
            stream.BeginRead(buffer, 0, buffer.Length, OnDataReceived, null);
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            synch.RecieveSynchroniseDevices();
        });
    }

    public void SendMessageToClient()
    {
        if (client != null)
        {
            string messageToSend = networkString;
            byte[] data = Encoding.ASCII.GetBytes(messageToSend);

            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length); // Send message to client

            Debug.Log("Sent to server: " + messageToSend);
            networkString = ""; // Clear the network string after sending
        }
        else
        {
            Debug.LogWarning("Client is not connected to the server!");
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

