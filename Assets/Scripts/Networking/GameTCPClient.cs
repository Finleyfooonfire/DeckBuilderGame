using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Net.Sockets;
using System.Text;
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

    //Recieve updates constantly
    private void Update()
    {
        if (synch != null)
        {
            synch.RecieveSynchroniseDevices();
        }
    }

    public void OnAttemptConnectToServer(string serverIP = "127.0.0.1")//IP defaults to localhost
    {
        client = new TcpClient();
        client.BeginConnect(serverIP, 7777, OnConnected, null);  // Non-blocking connect to the server
        Debug.Log(serverIP);
    }

    void OnConnected(IAsyncResult ar)
    {
        if (client.Connected)
        {
            Debug.Log("Connected to server!");

            //Keenan addition:
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                GameManager.Instance.StartGame(false);
            });
            //End

            ReceiveMessages(); // Start receiving messages
        }
    }

    void ReceiveMessages()
    {
        NetworkStream stream = client.GetStream();
        byte[] data = new byte[512];

        stream.BeginRead(data, 0, data.Length, (IAsyncResult ar) =>
        {
            int bytesRead = stream.EndRead(ar);
            string message = Encoding.ASCII.GetString(data, 0, bytesRead);
            Debug.Log("Received from server: " + message);
            networkString = message;

            ReceiveMessages(); // Continue receiving messages
        }, null);
    }

    public void SendMessageToServer()
    {
        if (client.Connected)
        {
            string messageToSend = networkString;
            byte[] data = Encoding.ASCII.GetBytes(messageToSend);

            NetworkStream stream = client.GetStream();
            stream.Write(data, 0, data.Length); // Send message to server

            Debug.Log("Sent to server: " + messageToSend);
            networkString = ""; // Clear the input field after sending
        }
        else
        {
            Debug.LogWarning("Client is not connected to the server!");
        }
    }

    void OnApplicationQuit()
    {
        if (client != null)
        {
            client.Close();  // Close the client connection when the application quits
        }
    }
}
