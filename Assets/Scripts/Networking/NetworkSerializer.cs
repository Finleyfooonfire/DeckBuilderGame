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

    //Convert a string that can be sent on the network into changes ingame.
    private enum Stage
    {
        None = 0,
        PlayerTurn = 1,
        CardPlayAreaCardCount = 2,
        CardPlayAreaCardsName = 3,
        CardPlayAreaCardsHealth = 4,
        CardPlayAreaCardsOwner = 5,
        PlayerGraveyardCardCount = 6,
        PlayerGraveyardCards = 7,
        PlayerHandCardCount = 8,
        PlayerHandCards = 9,
        PlayerDeckCardCount = 10,
        PlayerDeckCards = 11,
        OpponentGraveyardCardCount = 12,
        OpponentGraveyardCards = 13,
        OpponentHandCardCount = 14,
        OpponentHandCards = 15,
        OpponentDeckCardCount = 16,
        OpponentDeckCards = 17,
        Finished = 18,
    }

    ///TODO: Translating Card Data packets that are sent: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
    public void Deserialize(ref Transform playingField, DataStreamReader reader)
    {
       

        
    }
}