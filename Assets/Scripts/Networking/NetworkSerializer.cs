using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct CardsChange
{
    public List<Card> newCards;
    public List<Card> drawnCards;
    public List<Card> playedCards;
    public List<KeyValuePair<string,CardInfo>> changedCards;
    public List<KeyValuePair<string,CardInfo>> killedCards;
    public List<Card> revivedCards;

    public CardsChange(List<Card> newCardsIn, List<Card> drawnCardsIn, 
        List<Card> playedCardsIn, List<KeyValuePair<string,CardInfo>> killedCardsIn, 
        List<KeyValuePair<string,CardInfo>> changedCardsIn, List<Card> revivedCardsIn)
    {
        newCards = newCardsIn;
        drawnCards = drawnCardsIn;
        playedCards = playedCardsIn;
        killedCards = killedCardsIn;
        changedCards = changedCardsIn;
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


        //Write the number of cards in the changedCards list
        writer.WriteByte((byte)cardsChange.changedCards.Count);
        //Write the cards in the killedCards list
        for (int i = 0; i < cardsChange.changedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.changedCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.changedCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.changedCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.changedCards[i].Value.defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.changedCards[i].Key);
            writer.WriteByte((byte)cardsChange.changedCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.changedCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.changedCards[i].Value.exhausted ? 1 : 0));
        }


        //Write the number of cards in the killedCards list
        writer.WriteByte((byte)cardsChange.killedCards.Count);
        //Write the cards in the killedCards list
        for (int i = 0; i < cardsChange.killedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.killedCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.killedCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.killedCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.killedCards[i].Value.defenseValue);
            writer.WriteFixedString32((FixedString32Bytes)cardsChange.killedCards[i].Key);
            writer.WriteByte((byte)cardsChange.killedCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.killedCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.killedCards[i].Value.exhausted ? 1 : 0));
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
        return new CardsChange(ReadListOfCard(ref reader),//newCards
            ReadListOfCard(ref reader),//drawnCards
            ReadListOfCard(ref reader),//playedCards
            ReadListOfCardInfo(ref reader),//changedCards
            ReadListOfCardInfo(ref reader),//killedCards
            ReadListOfCard(ref reader));//revivedCards
    }

    //Cards that have been added to the deck
    List<Card> ReadListOfCard(ref DataStreamReader reader)
    {
        List<Card> cardsAdded = new List<Card>();
        //The first byte stores the number of cards to look for.
        byte cards = reader.ReadByte();
        for (int i = 0; i < cards; i++)
        {
            Card card = new Card();
            card.manaCost = reader.ReadByte();
            card.attackValue = reader.ReadByte();
            card.defenseValue = reader.ReadByte();
            card.cardName = reader.ReadFixedString32().ToString();
            card.faction = (Faction)reader.ReadByte();
            card.cardType = (CardType)reader.ReadByte();
            cardsAdded.Add(card);
        }
        return cardsAdded;
    }

    //Cards that have lost all health and thus move to the graveyard
    List<KeyValuePair<string, CardInfo>> ReadListOfCardInfo(ref DataStreamReader reader)
    {
        List<KeyValuePair<string,CardInfo>> cardsAdded = new List<KeyValuePair<string, CardInfo>>();
        //The first byte stores the number of cards to look for.
        byte cards = reader.ReadByte();
        for (int i = 0; i < cards; i++)
        {
            CardInfo card = new CardInfo();
            card.manaCost = reader.ReadByte();
            card.attackValue = reader.ReadByte();
            card.defenseValue = reader.ReadByte();
            card.faction = (Faction)reader.ReadByte();
            card.cardType = (CardType)reader.ReadByte();
            card.exhausted = reader.ReadByte() == 1;
            cardsAdded.Add(new KeyValuePair<string, CardInfo>(reader.ReadFixedString32().ToString(), card));
        }
        return cardsAdded;
    }

}