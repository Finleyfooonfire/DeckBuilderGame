using System.Collections.Generic;
using UnityEngine;

public class BoardState : MonoBehaviour
{

    Transform deck;
    Transform hand;
    Transform board;
    Transform graveyard;

    void Start()
    {
        
    }

    void OnAddToBoard(int cardSelected)
    {
       hand.GetChild(cardSelected).parent = board;
    }

    void OnKillCard(int cardSelected)
    {
        board.GetChild(cardSelected).parent = graveyard;
    }
}
