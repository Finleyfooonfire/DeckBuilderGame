using System.Net.Sockets;
using System.Net;
using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    bool isHost;
    [SerializeField] GameObject server;
    [SerializeField] GameObject client;
    [SerializeField] GameObject serverUI;
    [SerializeField] GameObject clientUI;
    [SerializeField] GameObject startButtons;
    

    public void OnStartGame(bool setHost)
    {
        startButtons.SetActive(false);
        isHost = setHost;
        StartUpNetwork();
    }

    //Gets the local IP address
    string GetLocalIPAddress()
    {
        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList)
        {
            if (ip.AddressFamily == AddressFamily.InterNetwork)
            {
                return ip.ToString();
            }
        }
        throw new System.Exception("No network adapters with an IPv4 address in the system!");
    }

    void StartUpNetwork()
    {
        if (isHost)
        {
            server.SetActive(true);
            serverUI.SetActive(true);
            clientUI.SetActive(true);
            string deviceIP;
            deviceIP = GetLocalIPAddress();
            //Get this device IP and used it to connect the TCPClient to the TCPServer
            client.GetComponent<TCPClient1>().OnAttemptConnectToServer(deviceIP);
        }
        else
        {
            server.SetActive(false);
            serverUI.SetActive(false);
            clientUI.SetActive(true);
            
        }
    }
}
