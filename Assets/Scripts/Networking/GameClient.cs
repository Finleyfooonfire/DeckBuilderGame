using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;

///TODO: Sending card data between client and host: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class GameClient : MonoBehaviour
{
    NetworkDriver m_Driver;
    NetworkConnection m_Connection;
    [SerializeField] PlayingFieldSynch playingFieldSynch;

    void Start()
    {
        m_Driver = NetworkDriver.Create();

        var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(7777);//Use localhost
        m_Connection = m_Driver.Connect(endpoint);
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
    }


    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            return;
        }

        

        Unity.Collections.DataStreamReader stream;
        NetworkEvent.Type cmd;
        while ((cmd = m_Connection.PopEvent(m_Driver, out stream)) != NetworkEvent.Type.Empty)
        {
            if (cmd == NetworkEvent.Type.Connect)
            {
                Debug.Log("We are now connected to the server.");
                FindFirstObjectByType<GameManager>().StartGame(false);
            }
            else if (cmd == NetworkEvent.Type.Data)
            {
                //Get the game updates from the server
                Debug.Log("Client recieved data");
                playingFieldSynch.Recieve(NetworkSerializer.Instance.Deserialize(ref stream));
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                m_Connection = default;
            }
        }
    }

    public void SendToServer(CardsChangeIn cardsChange)
    {
        //Send an update to the server.
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out var writer);
        NetworkSerializer.Instance.Serialize(cardsChange, ref writer);
        m_Driver.EndSend(writer);
    }
}

