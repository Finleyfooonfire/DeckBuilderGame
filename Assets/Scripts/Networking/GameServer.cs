using System.Collections.Generic;
using Unity.Collections;
using Unity.Networking.Transport;
using Unity.VisualScripting;
using UnityEngine;

///TODO: Sending card data between client and host: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class GameServer : MonoBehaviour
{
    NetworkDriver m_Driver;
    NativeList<NetworkConnection> m_Connections;
    [SerializeField] PlayingFieldSynch playingFieldSynch;

    void Start()
    {
        m_Driver = NetworkDriver.Create();
        m_Connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

        var endpoint = NetworkEndpoint.AnyIpv4.WithPort(7777);//Accept connections.
        if (m_Driver.Bind(endpoint) != 0)
        {
            Debug.LogError("Failed to bind to port 7777.");
            return;
        }
        m_Driver.Listen();
    }

    void OnDestroy()
    {
        if (m_Driver.IsCreated)
        {
            m_Driver.Dispose();
            m_Connections.Dispose();
        }
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();
        // Clean up connections.
        for (int i = 0; i < m_Connections.Length; i++)
        {
            if (!m_Connections[i].IsCreated)
            {
                m_Connections.RemoveAtSwapBack(i);
                i--;
            }
        }

        // Accept new connections.
        NetworkConnection c;
        while ((c = m_Driver.Accept()) != default)
        {
            m_Connections.Add(c);
            Debug.Log("Accepted a connection.");
            FindFirstObjectByType<GameManager>().StartGame(true);
        }


        

        for (int i = 0; i < m_Connections.Length; i++)
        {
            DataStreamReader stream;
            NetworkEvent.Type cmd;
            while ((cmd = m_Driver.PopEventForConnection(m_Connections[i], out stream)) != NetworkEvent.Type.Empty)
            {
                if (cmd == NetworkEvent.Type.Data)
                {
                    //Get the game updates from the client
                    Debug.Log("Server recieved data");
                    playingFieldSynch.Recieve(NetworkSerializer.Instance.Deserialize(ref stream));
                }
                else if (cmd == NetworkEvent.Type.Disconnect)
                {
                    Debug.Log("Client disconnected from the server.");
                    m_Connections[i] = default;
                    break;
                }
            }
        }
    }

    

    public void SendToClient(CardsChangeIn cardsChange)
    {
        //Send an update to the client.
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[0], out var writer);
        NetworkSerializer.Instance.Serialize(cardsChange, ref writer);
        m_Driver.EndSend(writer);
    }
}