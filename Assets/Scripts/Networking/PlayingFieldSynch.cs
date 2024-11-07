using UnityEngine;

public class PlayingFieldSynch : MonoBehaviour
{
    [SerializeField] GameTCPClient client;
    [SerializeField] GameTCPServer server;

    GameManager manager;

    bool isHost;

    void Start()
    {
        //Set isHost to true if the server is active.
        isHost = server.isActiveAndEnabled;
    }

    //Depending on whose turn it is, the device is to send or recieve data.
    public void SynchroniseDevices()
    {
        //Send data
        if (manager.isPlayerTurn)
        {
            if (isHost)
            {
                server.networkString = NetworkSerializer.Serialize(transform);
            }
            else
            {
                client.networkString = NetworkSerializer.Serialize(transform);
            }
        }
        //Recieve data and do things with it
        else
        {
            if (isHost)
            {
                transform = NetworkSerializer.Deserialize(transform, server.networkString);
            }
            else
            {
                transform = NetworkSerializer.Deserialize(transform, client.networkString);
            }
        }
    }
}
