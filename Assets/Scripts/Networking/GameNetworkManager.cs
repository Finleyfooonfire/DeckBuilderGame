using TMPro;
using Unity.Networking.Transport;
using UnityEngine;

public class GameNetworkManager : MonoBehaviour
{
    bool isHost;
    [SerializeField] GameObject server;
    [SerializeField] GameObject client;
    [SerializeField] GameObject startButtons;
    [SerializeField] GameObject startButtonsButtons;
    //[SerializeField] GameObject serverUI;
    //[SerializeField] TMP_Text serverIP;
    //[SerializeField] GameObject clientUI;
    //[SerializeField] GameObject IPButton;
    [SerializeField] TMP_InputField IPInput;
    public bool Turn;
    bool connected;

    public void OnConnectedToOpponent()
    {
        connected = true;
    }

    //What happens when the user clicks on the start as host or start as client buttons.
    public void OnStartGame(bool setHost)
    {
        isHost = setHost;
        startButtonsButtons.SetActive(false);
        StartUpNetwork();
    }
    private void Update()
    {
        if (connected)
        { 
            startButtons.SetActive(false);
        }
    }

    /*
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

    public string deviceIP;
    */

    void StartUpNetwork()
    {

        if (isHost)//What happens if the user is a host.
        {
            //The host is the server.
            server.SetActive(true);
            //serverUI.SetActive(true);
            client.SetActive(false);
            //clientUI.SetActive(false);
            /*
            //Get this device IP and use it to show the device's IP
            deviceIP = GetLocalIPAddress();
            Debug.Log("Your device's IP is: " + deviceIP);
            serverIP.text = "Your device's IP is: " + deviceIP;
            */
        }
        else//What happens if the user is a client.
        {
            //The client is only a client.
            server.SetActive(false);
            //serverUI.SetActive(false);
            client.SetActive(true);
            var clientScript = client.GetComponent<GameClient>();
            if (NetworkEndpoint.TryParse(IPInput.text, 7777, out _))
            {
                clientScript.ipInput = IPInput.text;
            }
            clientScript.StartClient();

            //clientUI.SetActive(true);
            /*
            //Get the IP from the input box and attempt to connect
            deviceIP = IPInput.text;
            Debug.Log(deviceIP);
            */
        }
    }

    /*
    public void OnConnectButton()
    {
        // Update deviceIP right before connection
        deviceIP = IPInput.text;
        Debug.Log("Attempting to connect to Server:" + (deviceIP != "" ? deviceIP : "127.0.0.1"));

        

        // Hide client connect UI after initiating connection
        clientUI.gameObject.SetActive(false);
        Turn = true;
        Debug.Log(Turn);
        //SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
    */
}
