using System.Net.Sockets;
using System.Net;
using UnityEngine;
using TMPro;

public class NetworkManager : MonoBehaviour
{
    bool isHost;
    [SerializeField] GameObject server;
    [SerializeField] GameObject client;
    [SerializeField] GameObject serverUI;
    [SerializeField] GameObject clientUI;
    [SerializeField] GameObject startButtons;
    [SerializeField] TMP_InputField IPInput;
    [SerializeField] GameObject IPButton;
    

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

    string deviceIP;

    void StartUpNetwork()
    {
        
        if (isHost)
        {
            server.SetActive(true);
            serverUI.SetActive(true);
            clientUI.SetActive(true);
            IPInput.gameObject.SetActive(false);
            IPButton.SetActive(false);
            //Get this device IP and used it to connect the TCPClient to the TCPServer
            deviceIP = GetLocalIPAddress();
            client.GetComponent<TCPClient1>().OnAttemptConnectToServer(deviceIP);
        }
        else
        {
            server.SetActive(false);
            serverUI.SetActive(false);
            clientUI.SetActive(true);
            //Get the IP from the input box and attempt to connect
            deviceIP = IPInput.text;
        }
    }

    public void OnConnectButton()
    {
        client.GetComponent<TCPClient1>().OnAttemptConnectToServer(deviceIP);
        IPInput.gameObject.SetActive(false);
        IPButton.SetActive(false);
    }
}
