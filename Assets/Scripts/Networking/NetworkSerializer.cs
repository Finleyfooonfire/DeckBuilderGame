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
        throw new NotImplementedException();
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