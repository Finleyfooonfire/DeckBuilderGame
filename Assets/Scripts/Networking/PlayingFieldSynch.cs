using System.Collections.Generic;
using UnityEngine;

///TODO: Relating card Changes from client/host into moves: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class PlayingFieldSynch : MonoBehaviour
{
    //Uses the client/server to send data to the other device.
    void Send()
    {
        throw new System.NotImplementedException();
    }

    //Called by the client/server when data is recieved and acts upon it.
    public void Recieve(CardsChange recievedCardsUpdate)
    {
        throw new System.NotImplementedException();
    }


}
