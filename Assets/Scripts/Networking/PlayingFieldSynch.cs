using NUnit.Framework.Internal;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

///TODO: Relating card Changes from client/host into moves: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class PlayingFieldSynch : MonoBehaviour
{
    /*//TEST:
    [SerializeField] TMP_Text textOut;
    [SerializeField] TMP_Text textIn;
    //END*/

    //Uses the client/server to send data to the other device.
    void Send()
    {
        throw new System.NotImplementedException();
    }

    //Called by the client/server when data is recieved and acts upon it.
    public void Recieve(CardsChange recievedCardsUpdate)
    {
        /*//TEST:
        Debug.Log("Recieved data: HASH = "+ recievedCardsUpdate.ToString());
        textIn.text = "IN: " + recievedCardsUpdate.ToString();
        //END
        */

        throw new System.NotImplementedException();
    }

    /*
    public void Test()
    {
        //TEST: Test the serializer, client and server.
        CardsChange test = new CardsChange(new List<Card>(), new List<Card>(), new List<Card>(), new List<KeyValuePair<string, CardInfo>>(), new List<KeyValuePair<string, CardInfo>>(), new List<Card>());
        FindAnyObjectByType<GameClient>().SendToServer(test);
        Debug.Log("Sent data: HASH = " + test.ToString());
        textOut.text = "OUT: " + test.ToString();
        //END
    }
    */
}
