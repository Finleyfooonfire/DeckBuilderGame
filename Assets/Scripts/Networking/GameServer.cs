using Unity.Collections;
using Unity.Networking.Transport;
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
                    NetMessageType msgType = (NetMessageType)stream.ReadByte();
                    switch (msgType)
                    {
                        case NetMessageType.KeepAlive:
                            m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[i], out var writer);
                            writer.WriteByte((byte)NetMessageType.KeepAlive);
                            m_Driver.EndSend(writer);
                            break;
                        case NetMessageType.CardChange:
                            playingFieldSynch.Recieve(NetworkSerializer.Instance.Deserialize(ref stream));
                            break;
                        default:
                            Debug.Log("Invalid message type");
                            break;
                    }
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



    public void SendToClient(HealthAndMana healthMana, CardsChangeIn cardsChange)
    {
        //Send an update to the client.
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connections[0], out var writer);
        writer.WriteByte((byte)NetMessageType.CardChange);
        NetworkSerializer.Instance.Serialize(healthMana, cardsChange, ref writer);
        m_Driver.EndSend(writer);
    }
}