using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public struct CardsChangeIn
{
    public List<KeyValuePair<string, CardInfo>> PlayedCards { get; private set; }
    public List<KeyValuePair<string,CardInfo>> ChangedCards { get; private set; }
    public List<KeyValuePair<string,CardInfo>> KilledCards { get; private set; }
    public List<KeyValuePair<string,CardInfo>> KilledFriendlyCards { get; private set; }
    public List<KeyValuePair<string, CardInfo>> RevivedCards { get; private set; }

    public CardsChangeIn(List<KeyValuePair<string, CardInfo>> playedCardsIn, List<KeyValuePair<string,CardInfo>> changedCardsIn,
         List<KeyValuePair<string, CardInfo>> killedCardsIn, List<KeyValuePair<string, CardInfo>> killedFriendlyCardsIn,
         List<KeyValuePair<string, CardInfo>> revivedCardsIn)
    {
        PlayedCards = playedCardsIn;
        ChangedCards = changedCardsIn;
        KilledCards = killedCardsIn;
        KilledFriendlyCards = killedFriendlyCardsIn;
        RevivedCards = revivedCardsIn;
    }
}

public struct CardsChangeOut
{
    public List<KeyValuePair<string, CardInfoStruct>> PlayedCards { get; private set; }
    public List<KeyValuePair<string, CardInfoStruct>> ChangedCards { get; private set; }
    public List<KeyValuePair<string, CardInfoStruct>> KilledCards { get; private set; }
    public List<KeyValuePair<string, CardInfoStruct>> KilledFriendlyCards { get; private set; }
    public List<KeyValuePair<string, CardInfoStruct>> RevivedCards { get; private set; }

    public CardsChangeOut(List<KeyValuePair<string, CardInfoStruct>> playedCardsIn, List<KeyValuePair<string, CardInfoStruct>> changedCardsIn,
         List<KeyValuePair<string, CardInfoStruct>> killedCardsIn, List<KeyValuePair<string, CardInfoStruct>> killedFriendlyCardsIn,
         List<KeyValuePair<string, CardInfoStruct>> revivedCardsIn)
    {
        PlayedCards = playedCardsIn;
        ChangedCards = changedCardsIn;
        KilledCards = killedCardsIn;
        KilledFriendlyCards = killedFriendlyCardsIn;
        RevivedCards = revivedCardsIn;
    }
}

public struct CardInfoStruct
{
    public bool isPlayerCard;
    public int manaCost;
    public int attackValue;
    public int defenseValue;
    public Faction faction;
    public CardType cardType;
    public bool exhausted;
    public Vector3 position;
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
    public void Serialize(CardsChangeIn cardsChange, ref DataStreamWriter writer)
    {
        //Write the number of cards in the playedCards list
        writer.WriteByte((byte)cardsChange.PlayedCards.Count);
        //Write the cards in the playedCards list
        for (int i = 0; i < cardsChange.PlayedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.PlayedCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.PlayedCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.PlayedCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.PlayedCards[i].Value.defenseValue);
            writer.WriteFixedString4096((FixedString4096Bytes)cardsChange.PlayedCards[i].Key);
            writer.WriteByte((byte)cardsChange.PlayedCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.PlayedCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.PlayedCards[i].Value.exhausted ? 1 : 0));
            writer.WriteFloat(cardsChange.PlayedCards[i].Value.gameObject.transform.position.x);
            writer.WriteFloat(cardsChange.PlayedCards[i].Value.gameObject.transform.position.y);
            writer.WriteFloat(cardsChange.PlayedCards[i].Value.gameObject.transform.position.z);
        }


        //Write the number of cards in the changedCards list
        writer.WriteByte((byte)cardsChange.ChangedCards.Count);
        //Write the cards in the killedCards list
        for (int i = 0; i < cardsChange.ChangedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.ChangedCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.ChangedCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.ChangedCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.ChangedCards[i].Value.defenseValue);
            writer.WriteFixedString4096((FixedString4096Bytes)cardsChange.ChangedCards[i].Key);
            writer.WriteByte((byte)cardsChange.ChangedCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.ChangedCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.ChangedCards[i].Value.exhausted ? 1 : 0));
            writer.WriteFloat(cardsChange.ChangedCards[i].Value.gameObject.transform.position.x);
            writer.WriteFloat(cardsChange.ChangedCards[i].Value.gameObject.transform.position.y);
            writer.WriteFloat(cardsChange.ChangedCards[i].Value.gameObject.transform.position.z);
        }


        //Write the number of cards in the killedCards list
        writer.WriteByte((byte)cardsChange.KilledCards.Count);
        //Write the cards in the killedCards list
        for (int i = 0; i < cardsChange.KilledCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.KilledCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.KilledCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.KilledCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.KilledCards[i].Value.defenseValue);
            writer.WriteFixedString4096((FixedString4096Bytes)cardsChange.KilledCards[i].Key);
            writer.WriteByte((byte)cardsChange.KilledCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.KilledCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.KilledCards[i].Value.exhausted ? 1 : 0));
            writer.WriteFloat(cardsChange.KilledCards[i].Value.gameObject.transform.position.x);
            writer.WriteFloat(cardsChange.KilledCards[i].Value.gameObject.transform.position.y);
            writer.WriteFloat(cardsChange.KilledCards[i].Value.gameObject.transform.position.z);
        }

        //Write the number of cards in the killedFriendlyCards list
        writer.WriteByte((byte)cardsChange.KilledFriendlyCards.Count);
        //Write the cards in the killedFriendlyCards list
        for (int i = 0; i < cardsChange.KilledFriendlyCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.KilledFriendlyCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.KilledFriendlyCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.KilledFriendlyCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.KilledFriendlyCards[i].Value.defenseValue);
            writer.WriteFixedString4096((FixedString4096Bytes)cardsChange.KilledFriendlyCards[i].Key);
            writer.WriteByte((byte)cardsChange.KilledFriendlyCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.KilledFriendlyCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.KilledFriendlyCards[i].Value.exhausted ? 1 : 0));
            writer.WriteFloat(cardsChange.KilledFriendlyCards[i].Value.gameObject.transform.position.x);
            writer.WriteFloat(cardsChange.KilledFriendlyCards[i].Value.gameObject.transform.position.y);
            writer.WriteFloat(cardsChange.KilledFriendlyCards[i].Value.gameObject.transform.position.z);
        }


        //Write the number of cards in the revivedCards list
        writer.WriteByte((byte)cardsChange.RevivedCards.Count);
        //Write the cards in the revivedCards list
        for (int i = 0; i < cardsChange.RevivedCards.Count; i++)
        {
            writer.WriteByte((byte)(cardsChange.RevivedCards[i].Value.isPlayerCard ? 1 : 0));
            writer.WriteByte((byte)cardsChange.RevivedCards[i].Value.manaCost);
            writer.WriteByte((byte)cardsChange.RevivedCards[i].Value.attackValue);
            writer.WriteByte((byte)cardsChange.RevivedCards[i].Value.defenseValue);
            writer.WriteFixedString4096((FixedString4096Bytes)cardsChange.RevivedCards[i].Key);
            writer.WriteByte((byte)cardsChange.RevivedCards[i].Value.faction);
            writer.WriteByte((byte)cardsChange.RevivedCards[i].Value.cardType);
            writer.WriteByte((byte)(cardsChange.RevivedCards[i].Value.exhausted ? 1 : 0));
            writer.WriteFloat(cardsChange.RevivedCards[i].Value.gameObject.transform.position.x);
            writer.WriteFloat(cardsChange.RevivedCards[i].Value.gameObject.transform.position.y);
            writer.WriteFloat(cardsChange.RevivedCards[i].Value.gameObject.transform.position.z);
        }
    }


    ///TODO: Translating Card Data packets that are sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public CardsChangeOut Deserialize(ref DataStreamReader reader)
    {
        return new CardsChangeOut(
            ReadListOfCardInfo(ref reader),//playedCards
            ReadListOfCardInfo(ref reader),//changedCards
            ReadListOfCardInfo(ref reader),//killedCards
            ReadListOfCardInfo(ref reader),//killedFriendlyCards
            ReadListOfCardInfo(ref reader));//revivedCards
    }

    
    List<KeyValuePair<string, CardInfoStruct>> ReadListOfCardInfo(ref DataStreamReader reader)
    {
        List<KeyValuePair<string,CardInfoStruct>> cardsAdded = new List<KeyValuePair<string, CardInfoStruct>>();
        //The first byte stores the number of cards to look for.
        byte cards = reader.ReadByte();
        for (int i = 0; i < cards; i++)
        {
            CardInfoStruct card = new CardInfoStruct();
            card.isPlayerCard = reader.ReadByte() == 1;
            card.manaCost = reader.ReadByte();
            card.attackValue = reader.ReadByte();
            card.defenseValue = reader.ReadByte();
            string name = reader.ReadFixedString4096().ToString();
            card.faction = (Faction)reader.ReadByte();
            card.cardType = (CardType)reader.ReadByte();
            card.exhausted = reader.ReadByte() == 1;
            float x = reader.ReadFloat();
            float y = reader.ReadFloat();
            float z = reader.ReadFloat();
            card.position = new Vector3(x, y, x);
            cardsAdded.Add(new KeyValuePair<string, CardInfoStruct>(name, card));
        }
        return cardsAdded;
    }

}