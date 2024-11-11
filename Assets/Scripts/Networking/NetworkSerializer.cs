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
        string output = "";
        //Whose turn is it? 1 for the opponent, 0 for us.
        output += Convert.ToInt32(!GameManager.Instance.isPlayerTurn).ToString();
        output += "\n";
        //Get the number of cards in each area and then add the card data.
        Transform cardPlayArea = playingField.Find("CardPlayArea");
        output += cardPlayArea.childCount.ToString();
        output += "\n";//Separate pieces of data with a '\n'
        foreach (Transform x in cardPlayArea)
        {
            //Put the name first then the health of the card then if the player owns the card (inverted).
            CardInfo cardInfo = x.GetComponent<CardInfo>();
            output += cardInfo.gameObject.name;
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
        // Initialize deserialize function variables
        Stage stage = Stage.PlayerTurn;
        int itemCount = 0;
        string subString = "";

        // Deserialized data
        int numberOfCards = 0;
        List<CardInfo> cardsInfo = new List<CardInfo>();
        // The player's cards
        List<Card> playerGraveCardList = new List<Card>();
        List<Card> playerHandCardList = new List<Card>();
        List<Card> playerDeckCardList = new List<Card>();
        // The opponent's cards
        List<Card> opponentGraveCardList = new List<Card>();
        List<Card> opponentHandCardList = new List<Card>();
        List<Card> opponentDeckCardList = new List<Card>();

        Debug.Log("Starting deserialization.");

        // Iterate through the input one character at a time
        foreach (char c in input)
        {
            if (c != '\n') // Build up the substring until it reaches the end
            {
                subString += c;
            }
            else // Once the end is reached, process the substring
            {
                Debug.Log($"Processing stage: {stage} with data: {subString}");

                // Different actions occur depending on the current stage
                switch (stage)
                {
                    case Stage.PlayerTurn:
                        GameManager.Instance.isPlayerTurn = (int.Parse(subString) == 1);
                        Debug.Log($"Player turn set to: {GameManager.Instance.isPlayerTurn}");
                        stage = Stage.CardPlayAreaCardCount;
                        break;

                    case Stage.CardPlayAreaCardCount:
                        numberOfCards = int.Parse(subString);
                        Debug.Log($"Card play area count: {numberOfCards}");

                        // Retrieve CardInfo components and adjust the list count
                        cardsInfo = playingField.Find("CardPlayArea").GetComponentsInChildren<CardInfo>().ToList();

                        // Resize the list if needed
                        if (cardsInfo.Count != numberOfCards)
                        {
                            Debug.LogWarning($"Adjusting cardsInfo list from {cardsInfo.Count} to match the expected count: {numberOfCards}");
                            while (cardsInfo.Count < numberOfCards) cardsInfo.Add(new CardInfo());
                            while (cardsInfo.Count > numberOfCards) cardsInfo.RemoveAt(cardsInfo.Count - 1);
                        }

                        Debug.Log($"cardsInfo list now has {cardsInfo.Count} items.");
                        stage = Stage.CardPlayAreaCardsName;
                        break;

                    case Stage.CardPlayAreaCardsName:
                        if (cardsInfo.Count > 0 && numberOfCards > 0)
                        {
                            cardsInfo[cardsInfo.Count - numberOfCards].name = subString;
                            Debug.Log($"Set card name: {cardsInfo[cardsInfo.Count - numberOfCards].name}");
                            stage = Stage.CardPlayAreaCardsHealth;
                        }
                        break;

                    case Stage.CardPlayAreaCardsHealth:
                        if (cardsInfo.Count > 0 && numberOfCards > 0)
                        {
                            cardsInfo[cardsInfo.Count - numberOfCards].defenseValue = int.Parse(subString);
                            Debug.Log($"Set card health: {cardsInfo[cardsInfo.Count - numberOfCards].defenseValue}");
                            stage = Stage.CardPlayAreaCardsOwner;
                        }
                        break;

                    case Stage.CardPlayAreaCardsOwner:
                        if (cardsInfo.Count > 0 && numberOfCards > 0)
                        {
                            cardsInfo[cardsInfo.Count - numberOfCards].isPlayerCard = bool.Parse(subString);
                            Debug.Log($"Set card owner to {(cardsInfo[cardsInfo.Count - numberOfCards].isPlayerCard ? "Player" : "Opponent")} for card {cardsInfo[cardsInfo.Count - numberOfCards].name}");

                            numberOfCards--;
                            if (numberOfCards > 0)
                            {
                                stage = Stage.CardPlayAreaCardsName;
                            }
                            else
                            {
                                Debug.Log("Finished setting play area cards.");
                                stage = Stage.PlayerGraveyardCardCount;
                            }
                        }
                        break;

                    case Stage.PlayerGraveyardCardCount:
                        numberOfCards = int.Parse(subString);
                        Debug.Log($"Player graveyard card count: {numberOfCards}");
                        playerGraveCardList = playingField.Find("Player").Find("PlayerGrave").GetComponentsInChildren<Card>().ToList();

                        // Resize the list if needed
                        while (playerGraveCardList.Count < numberOfCards) playerGraveCardList.Add(new Card());
                        while (playerGraveCardList.Count > numberOfCards) playerGraveCardList.RemoveAt(playerGraveCardList.Count - 1);

                        Debug.Log($"playerGraveCardList now has {playerGraveCardList.Count} items.");
                        stage = Stage.PlayerGraveyardCards;
                        break;

                    case Stage.PlayerGraveyardCards:
                        if (playerGraveCardList.Count > 0 && numberOfCards > 0)
                        {
                            var graveCard = playerGraveCardList[playerGraveCardList.Count - numberOfCards];
                            graveCard.cardName = cardStatNames[subString].name;
                            graveCard.manaCost = cardStatNames[subString].manaCost;
                            graveCard.attackValue = cardStatNames[subString].attackValue;
                            graveCard.defenseValue = cardStatNames[subString].defenseValue;
                            graveCard.faction = cardStatNames[subString].faction;
                            graveCard.cardType = cardStatNames[subString].cardType;
                            Debug.Log($"Set player graveyard card: {graveCard.cardName} with stats: Mana {graveCard.manaCost}, Attack {graveCard.attackValue}, Defense {graveCard.defenseValue}");

                            numberOfCards--;
                            if (numberOfCards == 0)
                            {
                                Debug.Log("Finished setting player graveyard cards.");
                                stage = Stage.PlayerHandCardCount;
                            }
                        }
                        break;
                    case Stage.PlayerHandCardCount:
                        numberOfCards = int.Parse(subString);
                        //Debug.Log($"Player hand card count: {numberOfCards}");
                        playerHandCardList = playingField.Find("Player").Find("PlayerHand").GetComponentsInChildren<Card>().ToList();
                        playerHandCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;

                    case Stage.PlayerHandCards:
                        var handCard = playerHandCardList[numberOfCards];
                        handCard.cardName = cardStatNames[subString].name;
                        handCard.manaCost = cardStatNames[subString].manaCost;
                        handCard.attackValue = cardStatNames[subString].attackValue;
                        handCard.defenseValue = cardStatNames[subString].defenseValue;
                        handCard.faction = cardStatNames[subString].faction;
                        handCard.cardType = cardStatNames[subString].cardType;
                   // Debug.Log($"Set player hand card: {handCard.cardName} with stats: Mana {handCard.manaCost}, Attack {handCard.attackValue}, Defense {handCard.defenseValue}");

                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                        Debug.Log("Finished setting player hand cards.");
                            stage += 1;
                        }
                        break;

                    case Stage.PlayerDeckCardCount:
                        numberOfCards = int.Parse(subString);
                  //  Debug.Log($"Player deck card count: {numberOfCards}");
                        playerDeckCardList = playingField.Find("Player").Find("PlayerDeck").GetComponentsInChildren<Card>().ToList();
                        playerDeckCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;

                    case Stage.PlayerDeckCards:
                        var deckCard = playerDeckCardList[numberOfCards];
                        deckCard.cardName = cardStatNames[subString].name;
                        deckCard.manaCost = cardStatNames[subString].manaCost;
                        deckCard.attackValue = cardStatNames[subString].attackValue;
                        deckCard.defenseValue = cardStatNames[subString].defenseValue;
                        deckCard.faction = cardStatNames[subString].faction;
                        deckCard.cardType = cardStatNames[subString].cardType;
                  // Debug.Log($"Set player deck card: {deckCard.cardName} with stats: Mana {deckCard.manaCost}, Attack {deckCard.attackValue}, Defense {deckCard.defenseValue}");

                        numberOfCards--;
                        if (numberOfCards == 0)
                        {
                         Debug.Log("Finished setting player deck cards.");
                            stage += 1;
                        }
                        break;

                    case Stage.OpponentGraveyardCardCount:
                        numberOfCards = int.Parse(subString);
                       //Debug.Log($"Opponent graveyard card count: {numberOfCards}");
                        opponentGraveCardList = playingField.Find("Opponent").Find("PlayerGrave").GetComponentsInChildren<Card>().ToList();
                        opponentGraveCardList.Capacity = numberOfCards;
                        stage += 1;
                        break;

                    case Stage.OpponentGraveyardCards:
                        var opponentGraveCard = opponentGraveCardList[numberOfCards];
                        opponentGraveCard.cardName = cardStatNames[subString].name;
                        opponentGraveCard.manaCost = cardStatNames[subString].manaCost;
                        opponentGraveCard.attackValue = cardStatNames[subString].attackValue;
                        opponentGraveCard.defenseValue = cardStatNames[subString].defenseValue;
                        opponentGraveCard.faction = cardStatNames[subString].faction;
                        opponentGraveCard.cardType = cardStatNames[subString].cardType;
                     //Debug.Log($"Set opponent graveyard card: {opponentGraveCard.cardName} with stats: Mana {opponentGraveCard.manaCost}, Attack {opponentGraveCard.attackValue}, Defense {opponentGraveCard.defenseValue}");

                 

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
                        #region Update CardPlayArea
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
                        #endregion
                        //Update the player's cards.
                        #region Update PlayerGrave
                        Transform playerGrave = playingField.Find("Player").Find("PlayerGrave");
                        //Change the number of cards in the player's grave to match the new number.
                        if (playerGrave.childCount > playerGraveCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = playerGrave.childCount - playerGraveCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(playerGrave.GetChild(0).gameObject);
                            }
                        }
                        else if (playerGrave.childCount < playerGraveCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), playerGrave);
                        }

                        index = 0;
                        foreach (Transform x in playerGrave)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerGraveCardList[index];
                            index++;
                        }
                        #endregion
                        #region Update PlayerHand
                        Transform playerHand = playingField.Find("Player").Find("PlayerHand");
                        //Change the number of cards in the player's hand to match the new number.
                        if (playerHand.childCount > playerHandCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = playerHand.childCount - playerHandCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(playerHand.GetChild(0).gameObject);
                            }
                        }
                        else if (playerHand.childCount < playerHandCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), playerHand);
                        }

                        index = 0;
                        foreach (Transform x in playerHand)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerHandCardList[index];
                            index++;
                        }
                        #endregion
                        #region Update PlayerDeck
                        Transform playerDeck = playingField.Find("Player").Find("PlayerDeck");
                        //Change the number of cards in the player's deck to match the new number.
                        if (playerDeck.childCount > playerDeckCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = playerDeck.childCount - playerDeckCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(playerDeck.GetChild(0).gameObject);
                            }
                        }
                        else if (playerDeck.childCount < playerDeckCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), playerDeck);
                        }

                        index = 0;
                        foreach (Transform x in playerDeck)
                        {
                            Card card = x.GetComponent<Card>();
                            card = playerDeckCardList[index];
                            index++;
                        }
                        #endregion
                        //Update the opponent's cards.
                        #region Update OpponentGrave
                        //Update the opponent's cards.
                        Transform opponentGrave = playingField.Find("Opponent").Find("OpponentGrave");
                        //Change the number of cards in the opponent's grave to match the new number.
                        if (opponentGrave.childCount > opponentGraveCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = opponentGrave.childCount - opponentGraveCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(opponentGrave.GetChild(0).gameObject);
                            }
                        }
                        else if (opponentGrave.childCount < opponentGraveCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), opponentGrave);
                        }

                        index = 0;
                        foreach (Transform x in opponentGrave)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentGraveCardList[index];
                            index++;
                        }
                        #endregion
                        #region Update OpponentHand
                        Transform opponentHand = playingField.Find("Opponent").Find("OpponentHand");
                        //Change the number of cards in the opponent's hand to match the new number.
                        if (opponentHand.childCount > opponentHandCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = opponentHand.childCount - opponentHandCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(opponentHand.GetChild(0).gameObject);
                            }
                        }
                        else if (opponentHand.childCount < opponentHandCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), opponentHand);
                        }

                        index = 0;
                        foreach (Transform x in opponentHand)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentHandCardList[index];
                            index++;
                        }
                        #endregion
                        #region Update OpponentDeck
                        Transform opponentDeck = playingField.Find("Opponent").Find("OpponentDeck");
                        //Change the number of cards in the opponent's deck to match the new number.
                        if (opponentDeck.childCount > opponentDeckCardList.Count)
                        {
                            //If there are more cards than necessary excess is to be removed
                            int excess = opponentDeck.childCount - opponentDeckCardList.Count;
                            for (int i = 0; i < excess; i++)
                            {
                                MonoBehaviour.Destroy(opponentDeck.GetChild(0).gameObject);
                            }
                        }
                        else if (opponentDeck.childCount < opponentDeckCardList.Count)
                        {
                            //If there are not enough cards, more should be added.
                            GameObject.Instantiate(Resources.Load("CardPrefab"), opponentDeck);
                        }

                        index = 0;
                        foreach (Transform x in opponentDeck)
                        {
                            Card card = x.GetComponent<Card>();
                            card = opponentDeckCardList[index];
                            index++;
                        }
                        #endregion

                        break;
                }
                //Clear the substring
                subString = "";
            }
        }
    }
}