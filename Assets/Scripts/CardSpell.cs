using System;
using UnityEngine;

public abstract class CardSpell : MonoBehaviour
{
    
    void Start()
    {
        
    }

    public virtual void OnUpdateTurn()
    {
        if (!GetComponent<CardInfo>().isPlayerCard) return;//Don't update if the card isn't owned by the player.
    }

    //This is where the card's effects go
    public abstract void DoMagic();
}
