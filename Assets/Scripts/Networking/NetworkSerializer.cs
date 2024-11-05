using UnityEngine;
static class NetworkSerializer
{
    //Convert changes ingame into a string that can be sent on the network.
    public static string Serialize(Transform playingField)
    {
        string output = "";
        //Get the number of cards in each area and then add the card data.
        Transform cardPlayArea = playingField.Find("CardPlayArea");
        output += cardPlayArea.childCount.ToString();
        output += "\n";//Separate pieces of data with a '\n'
        foreach (Transform x in cardPlayArea)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }

        //Serialize all player card stats.
        //Grave cards
        Transform playerGrave = playingField.Find("Player").Find("PlayerGrave");
        output += playerGrave.childCount.ToString();
        output += "\n";
        foreach (Transform x in playerGrave)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }
        //Hand cards
        Transform playerHand = playingField.Find("Player").Find("PlayerHand");
        output += playerHand.childCount.ToString();
        output += "\n";
        foreach (Transform x in playerHand)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }
        //Deck cards
        Transform playerDeck = playingField.Find("Player").Find("PlayerDeck");
        output += playerDeck.childCount.ToString();
        output += "\n";
        foreach (Transform x in playerDeck)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }


        //Serialize all opponent card stats
        //Grave cards
        Transform opponentGrave = playingField.Find("Opponent").Find("OpponentGrave");
        output += opponentGrave.childCount.ToString();
        output += "\n";
        foreach (Transform x in opponentGrave)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }
        //Hand cards
        Transform opponentHand = playingField.Find("Opponent").Find("OpponentHand");
        output += opponentHand.childCount.ToString();
        output += "\n";
        foreach (Transform x in opponentHand)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }
        //Deck cards
        Transform opponentDeck = playingField.Find("Opponent").Find("OpponentDeck");
        output += opponentDeck.childCount.ToString();
        output += "\n";
        foreach (Transform x in opponentDeck)
        {
            output += x.GetComponent<Card>().stats.name;
            output += "\n";
        }


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