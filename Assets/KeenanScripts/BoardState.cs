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
        Debug.Log(NetworkSerializer.Serialize(gameObject.transform));
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
