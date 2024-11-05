using UnityEngine;
static class NetworkSerializer
{
    //Convert changes ingame into a string that can be sent on the network.
    static string Serialize(Transform playingField)
    {
        Transform cardPlayArea = playingField.Find("CardPlayArea");
        
        Transform playerGrave = playingField.Find("PlayerGrave");
        Transform playerHand = playingField.Find("PlayerHand");
        Transform playerDeck = playingField.Find("PlayerDeck");

        Transform opponentGrave = playingField.Find("OpponentGrave");
        Transform opponentHand = playingField.Find("OpponentHand");
        Transform opponentDeck = playingField.Find("OpponentDeck");

        string output = "";

        //Conver the data into the string here.

        return output;
    }

    //Convert a string that can be sent on the network into changes ingame.
    static Transform Deserialize(Transform playingField, string input)
    {
        //Convert the string into the game thingy

        return playingField;
    }
}