using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct CardsChange
{
    public List<Card> newCards;
    public List<Card> drawnCards;
    public List<Card> playedCards;
    public List<CardInfo> killedCards;
    public List<Card> revivedCards;

    public CardsChange(List<Card> newCardsIn, List<Card> drawnCardsIn, List<Card> playedCardsIn, List<CardInfo> killedCardsIn, List<Card> revivedCardsIn)
    {
        newCards = newCardsIn;
        drawnCards = drawnCardsIn;
        playedCards = playedCardsIn;
        killedCards = killedCardsIn;
        revivedCards = revivedCardsIn;
    }
}

class NetworkSerializer
{
    private static NetworkSerializer instance;
    public static NetworkSerializer Instance
    {
        get
        {
            if (instance == null)
                instance = new NetworkSerializer();
            return instance;
        }
    }

    //A dictionary of all the card stats
    private Dictionary<string, CardStats> cardStatNames = new Dictionary<string, CardStats>();

    NetworkSerializer()
    {
        //Populate the dictionary with the CardStats and their names
        UnityEngine.Object[] foundAssets = Resources.FindObjectsOfTypeAll(typeof(CardStats));
        foreach (CardStats item in foundAssets)
        {
            cardStatNames.Add(item.name, item);
        }
    }

    //Convert changes ingame into a string that can be sent on the network.
    ///TODO: Packaging Card movement data into packet to be sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public void Serialize(CardsChange cardsChange, ref DataStreamWriter writer)
    {
        //Write the number of cards in the newCards list
        writer.WriteByte((byte)cardsChange.newCards.Count);
        //Write the cards in the newCards list
        for (int i = 0; i < cardsChange.newCards.Count; i++)
        {
            writer.WriteByte((byte)cardsChange.newCards[i].manaCost);
            writer.WriteByte((byte)cardsChange.newCards[i].attackValue);
            writer.WriteByte((byte)cardsChange.newCards[i].defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.newCards[i].cardName);
            writer.WriteByte((byte)cardsChange.newCards[i].faction);
            writer.WriteByte((byte)cardsChange.newCards[i].cardType);
        }

        //Write the number of cards in the drawnCards list
        writer.WriteByte((byte)cardsChange.drawnCards.Count);
        //Write the cards in the drawnCards list
        for (int i = 0; i < cardsChange.drawnCards.Count; i++)
        {
            writer.WriteByte((byte)cardsChange.drawnCards[i].manaCost);
            writer.WriteByte((byte)cardsChange.drawnCards[i].attackValue);
            writer.WriteByte((byte)cardsChange.drawnCards[i].defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.drawnCards[i].cardName);
            writer.WriteByte((byte)cardsChange.drawnCards[i].faction);
            writer.WriteByte((byte)cardsChange.drawnCards[i].cardType);
        }


        //Write the number of cards in the playedCards list
        writer.WriteByte((byte)cardsChange.playedCards.Count);
        //Write the cards in the playedCards list
        for (int i = 0; i < cardsChange.playedCards.Count; i++)
        {
            writer.WriteByte((byte)cardsChange.playedCards[i].manaCost);
            writer.WriteByte((byte)cardsChange.playedCards[i].attackValue);
            writer.WriteByte((byte)cardsChange.playedCards[i].defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.playedCards[i].cardName);
            writer.WriteByte((byte)cardsChange.playedCards[i].faction);
            writer.WriteByte((byte)cardsChange.playedCards[i].cardType);
        }


        //Write the number of cards in the killedCards list
        writer.WriteByte((byte)cardsChange.killedCards.Count);
        //Write the cards in the killedCards list
        for (int i = 0; i < cardsChange.killedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.killedCards[i].isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.killedCards[i].manaCost);
            writer.WriteByte((byte)cardsChange.killedCards[i].attackValue);
            writer.WriteByte((byte)cardsChange.killedCards[i].defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.killedCards[i].gameObject.name);
            writer.WriteByte((byte)cardsChange.killedCards[i].faction);
            writer.WriteByte((byte)cardsChange.killedCards[i].cardType);
        }


        //Write the number of cards in the revivedCards list
        writer.WriteByte((byte)cardsChange.revivedCards.Count);
        //Write the cards in the revivedCards list
        for (int i = 0; i < cardsChange.revivedCards.Count; i++)
        {
            writer.WriteByte((byte)cardsChange.revivedCards[i].manaCost);
            writer.WriteByte((byte)cardsChange.revivedCards[i].attackValue);
            writer.WriteByte((byte)cardsChange.revivedCards[i].defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.newCards[i].cardName);
            writer.WriteByte((byte)cardsChange.revivedCards[i].faction);
            writer.WriteByte((byte)cardsChange.revivedCards[i].cardType);
        }
    }


    ///TODO: Translating Card Data packets that are sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public CardsChange Deserialize(ref DataStreamReader reader)
    {
        return new CardsChange(CardsAddedToDeck(ref reader),
            CardsMovedFromDeckToHand(ref reader),
            CardsMovedFromHandToPlayArea(ref reader),
            CardsKilled(ref reader),
            CardsRevived(ref reader));
    }

    //Cards that have been added to the deck
    List<Card> CardsAddedToDeck(ref DataStreamReader reader)
    {
        throw new NotImplementedException();
    }
    //Cards that have moved from the deck to the hand
    List<Card> CardsMovedFromDeckToHand(ref DataStreamReader reader)
    {
        throw new NotImplementedException();
    }
    //Cards that have been played
    List<Card> CardsMovedFromHandToPlayArea(ref DataStreamReader reader)
    {
        throw new NotImplementedException();
    }
    //Cards that have lost all health and thus move to the graveyard
    List<CardInfo> CardsKilled(ref DataStreamReader reader)
    {
        throw new NotImplementedException();
    }
    //Cards revived from the graveyard and are back in play
    List<Card> CardsRevived(ref DataStreamReader reader)
    {
        throw new NotImplementedException();
    }

}