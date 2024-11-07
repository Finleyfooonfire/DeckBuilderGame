using UnityEngine;

public class PlayingFieldSynch : MonoBehaviour
{
    [SerializeField] GameTCPClient client;
    [SerializeField] GameTCPServer server;

    GameManager manager;
    Transform playingField;

    bool isHost;

    void Start()
    {
        //Set isHost to true if the server is active.
        isHost = server.isActiveAndEnabled;
        playingField = transform;
    }

    //Depending on whose turn it is, the device is to send or recieve data.
    public void SynchroniseDevices()
    {
        //Send data
        if (manager.isPlayerTurn)
        {
            if (isHost)
            {
                server.networkString = NetworkSerializer.Serialize(playingField);
            }
            else
            {
                client.networkString = NetworkSerializer.Serialize(playingField);
            }
        }
        //Recieve data and do things with it
        else
        {
            if (isHost)
            {
                NetworkSerializer.Deserialize(ref playingField, server.networkString);
            }
            else
            {
                NetworkSerializer.Deserialize(ref playingField, client.networkString);
            }
        }
    }
}
