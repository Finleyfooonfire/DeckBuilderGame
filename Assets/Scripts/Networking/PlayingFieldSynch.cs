using UnityEngine;

public class PlayingFieldSynch : MonoBehaviour
{
    [SerializeField] GameTCPClient client;
    [SerializeField] GameTCPServer server;

    GameManager manager;
    Transform playingField;

    bool isHost;
    bool setUp;

    void Start()
    {
        //Set isHost to true if the server is active.
        isHost = server.isActiveAndEnabled;
        manager = GameManager.Instance;
        playingField = transform;
        setUp = true;
    }


    //Depending on whose turn it is, the device is to send or recieve data.
    public void SendSynchroniseDevices()
    {
        if (!setUp) return;

        //Send data
        if (isHost)
        {
            server.networkString = NetworkSerializer.Serialize(playingField);
        }
        else
        {
            client.networkString = NetworkSerializer.Serialize(playingField);
        }
        
    }
    
    public void RecieveSynchroniseDevices()
    {
        if (!setUp) return;
        
        //Recieve data and do things with it
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
