using System.Collections.Generic;
using System.Linq;
using UnityEngine;
static class NetworkSerializer
{
    //A dictionary of all the card stats
    private static Dictionary<string, CardStats> cardStatNames = new Dictionary<string, CardStats>();

    static NetworkSerializer()
    {
        //Populate the dictionary with the CardStats and their names
        Object[] foundAssets = Resources.FindObjectsOfTypeAll(typeof(CardStats));
        foreach (CardStats item in foundAssets)
        {
            cardStatNames.Add(item.name, item);
        }
    }

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
            output += x.GetComponent<Card>().ToString();
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
            output += x.GetComponent<Card>().ToString();
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
        List<CardInfo> cardsInfo = new List<CardInfo>();
        //The player's cards
        List<Card> playerGraveCardList = new List<Card>();
        List<Card> playerHandCardList = new List<Card>();
        List<Card> playerDeckCardList = new List<Card>();
        //The opponent's cards
        List<Card> opponentGraveCardList = new List<Card>();
        List<Card> opponentHandCardList = new List<Card>();
        List<Card> opponentDeckCardList = new List<Card>();

        //Convert the string into the game thingy
        //iterate through the input a character at a time.
        foreach (char c in input)
        {
            if (c != '\n')//Build up the substring until it reaches the end.
            {
                subString += c;
            }
            else//Once the end is reached: do something. Then go to the next stage
            {
                //Different things happen depending on the stage that is in progress.
                switch (stage)
                {
                    case Stage.CardPlayAreaCardCount:
                        //Get the number of cards in the play area and create them
                        numberOfCards = int.Parse(subString);
                        cardsInfo = playingField.Find("CardPlayArea").GetComponentsInChildren<CardInfo>().ToList();
                        //Resize the array if the number of CardInfos are incorrect
                        cardsInfo.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.CardPlayAreaCardsName:
                        //Get the name of the card
                        cardsInfo[numberOfCards].name = subString;
                        stage += 1;
                        break;
                    case Stage.CardPlayAreaCardsHealth:
                        //Get the health of the card
                        cardsInfo[numberOfCards].defenseValue = int.Parse(subString);
                        stage += 1;
                        break;
                    case Stage.CardPlayAreaCardsOwner:
                        //Get the owner of the card
                        cardsInfo[numberOfCards].isPlayerCard = bool.Parse(subString);
                        numberOfCards--;//Reduce the number of cards left to look at.
                        //If there are more cards left: go back to Stage.CardPlayAreaCardsName
                        if (numberOfCards > 0)
                        {
                            stage = Stage.CardPlayAreaCardsName;
                        }
                        else
                        {
                            //Overwrite the playing field

                            //Go to the next stage
                            stage += 1;
                        }
                        break;
                    case Stage.PlayerGraveyardCardCount:
                        //Get the number of cards in the player's graveyard
                        numberOfCards = int.Parse(subString);
                        playerGraveCardList = playingField.Find("Player").Find("PlayerGrave").GetComponentsInChildren<Card>().ToList();
                        playerGraveCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.PlayerGraveyardCards:
                        //Get the cards in the graveyard
                        playerGraveCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        playerGraveCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        playerGraveCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        playerGraveCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        playerGraveCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        playerGraveCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.PlayerHandCardCount:
                        //Get the number of cards in the player's hand
                        numberOfCards = int.Parse(subString);
                        playerHandCardList = playingField.Find("Player").Find("PlayerHand").GetComponentsInChildren<Card>().ToList();
                        playerHandCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.PlayerHandCards:
                        //Get the cards in the hand
                        playerHandCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        playerHandCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        playerHandCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        playerHandCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        playerHandCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        playerHandCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.PlayerDeckCardCount:
                        //Get the number of cards in the player's deck
                        numberOfCards = int.Parse(subString);
                        playerDeckCardList = playingField.Find("Player").Find("PlayerDeck").GetComponentsInChildren<Card>().ToList();
                        playerDeckCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.PlayerDeckCards:
                        //Get the cards in the deck
                        playerDeckCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        playerDeckCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        playerDeckCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        playerDeckCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        playerDeckCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        playerDeckCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.OpponentGraveyardCardCount:
                        //Get the number of cards in the Opponent's graveyard
                        numberOfCards = int.Parse(subString);
                        opponentGraveCardList = playingField.Find("Opponent").Find("PlayerGrave").GetComponentsInChildren<Card>().ToList();
                        opponentGraveCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.OpponentGraveyardCards:
                        //Get the cards in the graveyard
                        opponentGraveCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        opponentGraveCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        opponentGraveCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        opponentGraveCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        opponentGraveCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        opponentGraveCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.OpponentHandCardCount:
                        //Get the number of cards in the opponent's hand
                        numberOfCards = int.Parse(subString);
                        opponentHandCardList = playingField.Find("Opponent").Find("PlayerHand").GetComponentsInChildren<Card>().ToList();
                        opponentHandCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.OpponentHandCards:
                        //Get the cards in the hand
                        opponentHandCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        opponentHandCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        opponentHandCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        opponentHandCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        opponentHandCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        opponentHandCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.OpponentDeckCardCount:
                        //Get the number of cards in the opponent's deck
                        numberOfCards = int.Parse(subString);
                        opponentDeckCardList = playingField.Find("Opponent").Find("PlayerDeck").GetComponentsInChildren<Card>().ToList();
                        opponentDeckCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;
                    case Stage.OpponentDeckCards:
                        //Get the cards in the deck
                        opponentDeckCardList[numberOfCards].cardName = cardStatNames[subString].name;
                        opponentDeckCardList[numberOfCards].manaCost = cardStatNames[subString].manaCost;
                        opponentDeckCardList[numberOfCards].attackValue = cardStatNames[subString].attackValue;
                        opponentDeckCardList[numberOfCards].defenseValue = cardStatNames[subString].defenseValue;
                        opponentDeckCardList[numberOfCards].faction = cardStatNames[subString].faction;
                        opponentDeckCardList[numberOfCards].cardType = cardStatNames[subString].cardType;
                        //If there are more cards left: read the rest
                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                            stage += 1;
                        }
                        break;
                    case Stage.Finished:
                        //The deserialization is finished. Use the data collected to update the playing field.
                        //Update card play area
                        Transform cardPlayArea = playingField.Find("CardPlayArea");
                        //Change the number of cards in the play area to match the new number.
                        if (cardPlayArea.childCount > cardsInfo.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = cardPlayArea.childCount - cardsInfo.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(cardPlayArea.GetChild(0).gameObject);
                            }
                        }
                        else if (cardPlayArea.childCount < cardsInfo.Count)
                        {
                            //If there are not enough cards, more should be added.
                            if (cardPlayArea.childCount != 0)
                            {
                                GameObject.Instantiate(cardPlayArea.GetChild(0).gameObject, cardPlayArea);
                            }
                            else
                            {
                                //Based on code in Card.cs
                                GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                                cardObject.transform.SetParent(cardPlayArea);
                                cardObject.transform.localScale = new Vector3(0.635f, 0.01f, 0.889f);
                                cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

                                _ = cardObject.AddComponent<CardInfo>();
                                _ = cardObject.AddComponent<CardAttack>();
                                //END
                            }
                        }


                        int index = 0;
                        foreach (Transform x in cardPlayArea)
                        {
                            CardInfo cardInfo = x.GetComponent<CardInfo>();
                            cardInfo = cardsInfo[index];
                            index++;
                        }


                        //Update the player's cards.
                        Transform playerGrave = playingField.Find("Player").Find("PlayerGrave");
                        index = 0;
                        foreach (Transform x in playerGrave)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerGraveCardList[index];
                            index++;
                        }

                        Transform playerHand = playingField.Find("Player").Find("PlayerHand");
                        index = 0;
                        foreach (Transform x in playerHand)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerHandCardList[index];
                            index++;
                        }

                        Transform playerDeck = playingField.Find("Player").Find("PlayerDeck");
                        index = 0;
                        foreach (Transform x in playerDeck)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerDeckCardList[index];
                            index++;
                        }

                        //Update the opponent's cards.
                        Transform opponentGrave = playingField.Find("Opponent").Find("OpponentGrave");
                        index = 0;
                        foreach (Transform x in opponentGrave)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentGraveCardList[index];
                            index++;
                        }

                        Transform opponentHand = playingField.Find("Opponent").Find("OpponentHand");
                        index = 0;
                        foreach (Transform x in opponentHand)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentHandCardList[index];
                            index++;
                        }

                        Transform opponentDeck = playingField.Find("Opponent").Find("OpponentDeck");
                        index = 0;
                        foreach (Transform x in opponentDeck)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentDeckCardList[index];
                            index++;
                        }

                        break;
                }
                //Clear the substring
                subString = "";
            }
        }
        return playingField;
    }
}