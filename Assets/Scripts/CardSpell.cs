using System;
using UnityEngine;

public abstract class CardSpell : MonoBehaviour
{
    protected int life;
    protected CardPlayAreaGrid cardGrid;

    protected void Start()
    {
        life = GetComponent<CardInfo>().defenseValue;
        Transform cardPlayArea = GameObject.Find("CardPlayArea").transform;
        cardGrid = cardPlayArea.gameObject.GetComponent<CardPlayAreaGrid>();
    }

    public virtual void OnUpdateTurn()
    {
        if (!GetComponent<CardInfo>().isPlayerCard) return;//Don't update if the card isn't owned by the player.
        //Do the spell update
        DoMagic();
        life--;
        if (life != 0) return;//Using a life of 0 make it invincible.
        OnDecommissionCard();
    }

    protected virtual void OnDecommissionCard()
    {
        Debug.Log($"{GetComponent<CardInfo>().name}'s usefulness has ran out. Moving card to graveyard.");
        GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//Destroy the spell card once its time has ran out.
    }

    //This is where the card's effects go
    public abstract void DoMagic();
}
