using System;
using System.Collections.Generic;
using System.Linq;
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
    public string Serialize(Transform playingField)
    {
        return "";
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
    public void Deserialize(ref Transform playingField, string input)
    {
       

        
    }
}