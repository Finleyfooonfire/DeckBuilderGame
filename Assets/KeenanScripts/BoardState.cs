using System.Collections.Generic;
using UnityEngine;

public class BoardState : MonoBehaviour
{

    Transform deck;
    Transform hand;
    Transform board;
    Transform graveyard;

    //Keenan addition: For debugging
    float lTime;
    void Update()
    {
        if (lTime + 5f >  Time.time) return;
        string serializedData = NetworkSerializer.Serialize(gameObject.transform);
        Debug.Log(serializedData);//Test to see if Serialize works
        Debug.Log(transform.Equals(NetworkSerializer.Deserialize(transform, serializedData)));//Test to see if Deserialize works
        lTime = Time.time;
    }
    //END

    void OnAddToBoard(int cardSelected)
    {
       hand.GetChild(cardSelected).parent = board;
    }

    void OnKillCard(int cardSelected)
    {
        board.GetChild(cardSelected).parent = graveyard;
    }
}
