using System.Collections.Generic;
using UnityEngine;

///TODO: Relating card Changes from client/host into moves: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class PlayingFieldSynch : MonoBehaviour
{
    [SerializeField] Transform cardPlayArea;
    //Player
    [SerializeField] Transform playerGrave;
    //Opponent
    [SerializeField] Transform opponentGrave;
    [SerializeField] Transform opponentHand;
    [SerializeField] Transform opponentDeck;

    CardsChange prevCardsChange;

    void Start()
    {
        prevCardsChange = new CardsChange();
    }

    //Returns a CardsChange with all the things that have changed since the start of the turn.
    CardsChange GetCardStatus()
    {
        List<Card> playedCards = new List<Card>();
        List<KeyValuePair<string, CardInfo>> changedCards = new List<KeyValuePair<string, CardInfo>>();
        List<KeyValuePair<string, CardInfo>> killedCards = new List<KeyValuePair<string, CardInfo>>();
        List<KeyValuePair<string, CardInfo>> killedFriendlyCards = new List<KeyValuePair<string, CardInfo>>();
        List<Card> revivedCards = new List<Card>();

        CardsChange cardsChange;


        //Get the changes
        throw new System.NotImplementedException();

        cardsChange = new CardsChange(playedCards, changedCards, killedCards, killedFriendlyCards, revivedCards);
        return cardsChange;
    }

    //Uses the client/server to send data to the other device.
    void Send()
    {
        CardsChange changes = GetCardStatus();
        var client = FindAnyObjectByType<GameClient>();
        if (client != null)
        {
            //The client exists so Send data.
            client.SendToServer(changes);
        }
        else
        {
            //It is server. Send data.
            FindAnyObjectByType<GameServer>().SendToClient(changes);
        }
    }

    //Called by the client/server when data is recieved and acts upon it.
    public void Recieve(CardsChange recievedCardsUpdate)
    {
        SetCardStatus(recievedCardsUpdate);
    }

    //Updates the board using the changes recieved.
    void SetCardStatus(CardsChange changeIn)
    {
        //Move cards from the hand to the play area
        foreach (var card in changeIn.playedCards)
        {
            opponentDeck.GetComponent<Deck>().handCards.Remove(card);
            GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cardObject.name = card.cardName;
            cardObject.transform.SetParent(cardPlayArea);
            cardObject.transform.position = new Vector3();
            cardObject.transform.localScale = new Vector3(0.635f, 0.01f, 0.889f);
            cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
            cardInfo.manaCost = card.manaCost;
            cardInfo.attackValue = card.attackValue;
            cardInfo.defenseValue = card.defenseValue;
            cardInfo.faction = card.faction;
            cardInfo.cardType = card.cardType;

            CardAttack cardAttack = cardObject.AddComponent<CardAttack>();
        }

        //Update any cards that have changed. Only the defense value and exhausted variables change.
        foreach (var card in changeIn.changedCards)
        {
            CardInfo oldCard = cardPlayArea.Find(card.Key).GetComponent<CardInfo>();
            oldCard.defenseValue = card.Value.defenseValue;
            oldCard.exhausted = card.Value.exhausted;
        }

        //Update any enemy cards that have been killed.
        foreach (var card in changeIn.killedCards)
        {
            cardPlayArea.Find(card.Key).SetParent(opponentGrave);
            cardPlayArea.Find(card.Key).transform.localPosition = new Vector3();
        }

        //Update any friendly cards that have been killed.
        foreach (var card in changeIn.killedFriendlyCards)
        {
            cardPlayArea.Find(card.Key).SetParent(playerGrave);
            cardPlayArea.Find(card.Key).transform.localPosition = new Vector3();
        }
    }
}
