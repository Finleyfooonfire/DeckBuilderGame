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

    public IEnumerator<List<KeyValuePair<string, CardInfo>>> GetEnumerator()
    {
        yield return PlayedCards;
        yield return ChangedCards;  
        yield return KilledCards;
        yield return KilledFriendlyCards;
        yield return RevivedCards;
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
    public Sprite cardImage;
}

public struct HealthAndMana
{
    public int playerMana { get; private set; }
    public int opponentMana { get; private set; }
    public int playerLife { get; private set; }
    public int opponentLife { get; private set; }

    public HealthAndMana(int playerManaIn, int opponentManaIn, int playerLifeIn, int opponentLifeIn)
    {
        playerMana = playerManaIn;
        opponentMana = opponentManaIn;
        playerLife = playerLifeIn;
        opponentLife = opponentLifeIn;
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

    

    //Convert changes ingame into a string that can be sent on the network.
    ///TODO: Packaging Card movement data into packet to be sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public void Serialize(HealthAndMana healthMana, CardsChangeIn cardsChange, ref DataStreamWriter writer)
    {
        writer.WriteByte((byte)healthMana.playerMana);
        writer.WriteByte((byte)healthMana.opponentMana);
        writer.WriteByte((byte)healthMana.playerLife);
        writer.WriteByte((byte)healthMana.opponentLife);

        foreach (var x in cardsChange)
        {
            //Write the number of cards in the list
            writer.WriteByte((byte)x.Count);
            //Write the cards in the playedCards list
            for (int i = 0; i < x.Count; i++)
            {
                writer.WriteByte((byte)(x[i].Value.isPlayerCard ? 1 : 0));
                writer.WriteByte((byte)x[i].Value.manaCost);
                writer.WriteByte((byte)x[i].Value.attackValue);
                writer.WriteByte((byte)x[i].Value.defenseValue);
                writer.WriteFixedString4096((FixedString4096Bytes)x[i].Key);
                writer.WriteByte((byte)x[i].Value.faction);
                writer.WriteByte((byte)x[i].Value.cardType);
                writer.WriteByte((byte)(x[i].Value.exhausted ? 1 : 0));
                writer.WriteFloat(x[i].Value.gameObject.transform.localPosition.x);
                writer.WriteFloat(x[i].Value.gameObject.transform.localPosition.y);
                writer.WriteFloat(x[i].Value.gameObject.transform.localPosition.z);
                writer.WriteFixedString32((FixedString32Bytes)x[i].Value.cardImage.name);
            }
        }
    }


    ///TODO: Translating Card Data packets that are sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public (HealthAndMana healthMana, CardsChangeOut cardsChange) Deserialize(ref DataStreamReader reader)
    {
        return (ReadHealthAndMana(ref reader), new CardsChangeOut(
            ReadListOfCardInfo(ref reader),//playedCards
            ReadListOfCardInfo(ref reader),//changedCards
            ReadListOfCardInfo(ref reader),//killedCards
            ReadListOfCardInfo(ref reader),//killedFriendlyCards
            ReadListOfCardInfo(ref reader))//revivedCards
            );
    }

    HealthAndMana ReadHealthAndMana(ref DataStreamReader reader)
    {
        //The player of one device is the opponent of the other
        int opponentManaRead = reader.ReadByte();
        int playerManaRead = reader.ReadByte();
        int opponentLifeRead = reader.ReadByte();
        int playerLifeRead = reader.ReadByte();
        return new HealthAndMana(playerManaRead, opponentManaRead, playerLifeRead, opponentLifeRead);
    }

    List<KeyValuePair<string, CardInfoStruct>> ReadListOfCardInfo(ref DataStreamReader reader)
    {
        List<KeyValuePair<string,CardInfoStruct>> cardsAdded = new List<KeyValuePair<string, CardInfoStruct>>();
        //The first byte stores the number of cards to look for.
        byte cards = reader.ReadByte();
        for (int i = 0; i < cards; i++)
        {
            CardInfoStruct card = new CardInfoStruct();
            card.isPlayerCard = reader.ReadByte() != 1;//Invert the player card flag
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
            card.position = new Vector3(-x, y, -z);//Mirror card positions
            string cardPath = "CardTextures/" + card.faction.ToString() + "Cards/" + reader.ReadFixedString32().ToString();
            Debug.Log(cardPath);
            Sprite cardSprite = Resources.Load<Sprite>(cardPath);
            Debug.Log(cardSprite);
            card.cardImage = (cardSprite);
            cardsAdded.Add(new KeyValuePair<string, CardInfoStruct>(name, card));
        }
        return cardsAdded;
    }

}