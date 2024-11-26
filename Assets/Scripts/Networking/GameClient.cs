using Unity.Networking.Transport;
using UnityEngine;

///TODO: Sending card data between client and host: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class GameClient : MonoBehaviour
{
    NetworkDriver m_Driver;
    NetworkConnection m_Connection;
    [SerializeField] PlayingFieldSynch playingFieldSynch;
    float lastKeepAlive;
    public string ipInput = "127.0.0.1";

    public void StartClient()
    {
        lastKeepAlive = Time.realtimeSinceStartup;
        m_Driver = NetworkDriver.Create();
        if (NetworkEndpoint.TryParse(ipInput, 45000, out var endpoint))
        {
            m_Connection = m_Driver.Connect(endpoint);
        }
        else
        {
            Debug.LogError("Unable to connect");
        }
    }

    void OnDestroy()
    {
        m_Driver.Dispose();
    }

    //Send a message to the server every few seconds to keep the connection from timing out.
    void KeepAlive()
    {
        if (Time.realtimeSinceStartup - lastKeepAlive > 15)
        {
            Debug.Log("Sending KeepAlive");
            m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out var writer);
            writer.WriteByte((byte)NetMessageType.KeepAlive);
            m_Driver.EndSend(writer);

            lastKeepAlive = Time.realtimeSinceStartup;
        }
    }

    void Update()
    {
        m_Driver.ScheduleUpdate().Complete();

        if (!m_Connection.IsCreated)
        {
            Debug.Log("Lost connection to server");
            return;
        }

        KeepAlive();

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
                NetMessageType msgType = (NetMessageType)stream.ReadByte();
                switch (msgType)
                {
                    case NetMessageType.KeepAlive:
                        Debug.Log("Still connected");
                        break;
                    case NetMessageType.CardChange:
                        playingFieldSynch.Recieve(NetworkSerializer.Instance.Deserialize(ref stream));
                        break;
                    case NetMessageType.EndGame:
                        Debug.Log("Ending game");
                        FindFirstObjectByType<GameManager>().GameOver(false);
                        break;
                    default:
                        Debug.Log("Invalid message type");
                        break;
                }
            }
            else if (cmd == NetworkEvent.Type.Disconnect)
            {
                Debug.Log("Client got disconnected from server.");
                m_Connection = default;
            }
        }
    }

    public void SendToServer(HealthAndMana healthMana, CardsChangeIn cardsChange)
    {
        //Send an update to the server.
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out var writer);
        writer.WriteByte((byte)NetMessageType.CardChange);
        NetworkSerializer.Instance.Serialize(healthMana, cardsChange, ref writer);
        m_Driver.EndSend(writer);
    }

    public void SendEndGame()
    {
        Debug.Log("Sending EndGame");
        m_Driver.BeginSend(NetworkPipeline.Null, m_Connection, out var writer);
        writer.WriteByte((byte)NetMessageType.EndGame);
        m_Driver.EndSend(writer);
    }
}

