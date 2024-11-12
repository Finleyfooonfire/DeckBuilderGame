using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
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
    public void Serialize(Transform playingField, ref DataStreamWriter writer)
    {
        
    }


    ///TODO: Translating Card Data packets that are sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public void Deserialize(ref Transform playingField, ref DataStreamReader reader)
    {
        CardsAddedToDeck(ref reader);
        CardsMovedFromDeckToHand(ref reader);
        CardsMovedFromHandToPlayArea(ref reader);
        CardsKilled(ref reader);
        CardsRevived(ref reader);
        
    }

    //Cards that have been added to the deck
    void CardsAddedToDeck(ref DataStreamReader reader) { }
    //Cards that have moved from the deck to the hand
    void CardsMovedFromDeckToHand(ref DataStreamReader reader) { }
    //Cards that have been played
    void CardsMovedFromHandToPlayArea(ref DataStreamReader reader) { }
    //Cards that have lost all health and thus move to the graveyard
    void CardsKilled(ref DataStreamReader reader) { }
    //Cards revived from the graveyard and are back in play
    void CardsRevived(ref DataStreamReader reader) { }
}