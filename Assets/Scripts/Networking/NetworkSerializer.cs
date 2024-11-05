using System;
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
            //Put the name first then the health of the card then if the player owns the card (inverted).
            CardInfo cardInfo = x.GetComponent<CardInfo>();
            output += cardInfo.name;
            output += "\n";
            output += cardInfo.defenseValue.ToString();
            output += "\n";
            output += (!cardInfo.isPlayerCard).ToString();
            output += "\n";
        }

        //Serialize all player card stats.
        //Grave cards
        Transform playerGrave = playingField.Find("Player").Find("PlayerGrave");
        output += playerGrave.childCount.ToString();
        output += "\n";
        foreach (Transform x in playerGrave)
        {
            output += x.GetComponent<CardInfo>().ToString();
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
            output += x.GetComponent<CardInfo>().ToString();
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
    private enum Stage
    {
        None = 0,
        CardPlayAreaCardCount = 1,
        CardPlayAreaCardsName = 2,
        CardPlayAreaCardsHealth = 3,
        CardPlayAreaCardsOwner = 4,
        PlayerGraveyardCardCount = 5,
        PlayerGraveyardCards = 6,
        PlayerHandCardCount = 7,
        PlayerHandCards = 8,
        PlayerDeckCardCount = 9,
        PlayerDeckCards = 10,
        OpponentGraveyardCardCount = 11,
        OpponentGraveyardCards = 12,
        OpponentHandCardCount = 13,
        OpponentHandCards = 14,
        OpponentDeckCardCount = 15,
        OpponentDeckCards = 16,
        Finished = 17,
    }
    static Transform Deserialize(Transform playingField, string input)
    {
        //Initialize deserialize function variables
        Stage stage = Stage.None;
        int itemCount = 0;
        string subString = "";

        //Deserialized data
        int numberOfCards = 0;

        //Convert the string into the game thingy
        //iterate through the input a character at a time.
        foreach (char c in input)
        {
            if (c != '\n')//Build up the substring until it reaches the end.
            {
                subString += c;
            }
            else//Once the end is reached, do something. Then go to the next stage
            {
                //Different things happen depending on the stage that is in progress.
                switch (stage)
                {
                    case Stage.CardPlayAreaCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.CardPlayAreaCardsName:

                        break;
                    case Stage.CardPlayAreaCardsHealth:

                        break;
                    case Stage.CardPlayAreaCardsOwner:
                        numberOfCards--;//Reduce the number of cards left to look at.
                        break;
                    case Stage.PlayerGraveyardCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.PlayerGraveyardCards:

                        break;
                    case Stage.PlayerHandCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.PlayerHandCards:

                        break;
                    case Stage.PlayerDeckCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.PlayerDeckCards:

                        break;
                    case Stage.OpponentGraveyardCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.OpponentGraveyardCards:

                        break;
                    case Stage.OpponentHandCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.OpponentHandCards:

                        break;
                    case Stage.OpponentDeckCardCount:
                        numberOfCards = int.Parse(subString);
                        break;
                    case Stage.OpponentDeckCards:

                        break;
                    case Stage.Finished:

                        break;
                }
                //Clear the substring and go to the next stage.
                subString = "";
                stage += 1;
            }
        }
        
        
        return playingField;
    }
}