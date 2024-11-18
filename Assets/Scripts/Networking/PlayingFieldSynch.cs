using UnityEngine;

///TODO: Relating card Changes from client/host into moves: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class PlayingFieldSynch : MonoBehaviour
{
    /*//TEST:
    [SerializeField] TMP_Text textOut;
    [SerializeField] TMP_Text textIn;
    //END*/

    [SerializeField] Transform cardPlayArea;
    //Opponent
    [SerializeField] Transform opponentGrave;
    [SerializeField] Transform opponentHand;
    [SerializeField] Transform opponentDeck;

    //Returns a CardsChange with all the things that have changed since the start of the turn.
    CardsChange GetCardStatus()
    {
        throw new System.NotImplementedException();
    }

    //Uses the client/server to send data to the other device.
    void Send()
    {
        CardsChange changes = GetCardStatus();
        var client = FindAnyObjectByType<GameClient>();
        if (client != null)
        {
            //The client exists so Send data.
            client.SendToServer(changes);
        }
        else
        {
            //It is server. Send data.
            FindAnyObjectByType<GameServer>().SendToClient(changes);
        }
        throw new System.NotImplementedException();
    }

    //Called by the client/server when data is recieved and acts upon it.
    public void Recieve(CardsChange recievedCardsUpdate)
    {
        /*//TEST:
        Debug.Log("Recieved data: HASH = "+ recievedCardsUpdate.ToString());
        textIn.text = "IN: " + recievedCardsUpdate.ToString();
        //END
        */
        SetCardStatus(recievedCardsUpdate);

        throw new System.NotImplementedException();
    }

    //Updates the board using the changes recieved.
    void SetCardStatus(CardsChange changeIn)
    {
        //Add new cards to the deck
        foreach (var newCard in changeIn.newCards)
        {
            opponentDeck.GetComponent<Deck>().deckCards.Add(newCard);
        }

        //Move cards from the deck to the hand
        foreach (var newCard in changeIn.drawnCards)
        {
            opponentDeck.GetComponent<Deck>().deckCards.Remove(newCard);
            opponentDeck.GetComponent<Deck>().handCards.Add(newCard);
        }

        //Move cards from the hand to the play area
        foreach (var card in changeIn.playedCards)
        {
            opponentDeck.GetComponent<Deck>().handCards.Remove(card);
            GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            //Keenan addition
            cardObject.name = card.cardName;
            //END
            cardObject.transform.SetParent(cardPlayArea);
            cardObject.transform.position = new Vector3();
            cardObject.transform.localScale = new Vector3(0.635f, 0.01f, 0.889f);
            cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
            cardInfo.manaCost = card.manaCost;
            cardInfo.attackValue = card.attackValue;
            cardInfo.defenseValue = card.defenseValue;
            cardInfo.faction = card.faction;
            cardInfo.cardType = card.cardType;

            //Keenan Addition
            CardAttack cardAttack = cardObject.AddComponent<CardAttack>();
            //End
        }

        //Update any cards that have changed. Only the defense value and exhausted variables change.
        foreach (var card in changeIn.changedCards)
        {
            CardInfo oldCard = cardPlayArea.Find(card.Key).GetComponent<CardInfo>();
            oldCard.defenseValue = card.Value.defenseValue;
            oldCard.exhausted = card.Value.exhausted;
        }

        //Update any cards that have been killed.
        foreach (var card in changeIn.killedCards) 
        {
            cardPlayArea.Find(card.Key).SetParent(opponentGrave);
            cardPlayArea.Find(card.Key).transform.localPosition = new Vector3();
        }
    }

    /*
    public void Test()
    {
        //TEST: Test the serializer, client and server.
        CardsChange test = new CardsChange(new List<Card>(), new List<Card>(), new List<Card>(), new List<KeyValuePair<string, CardInfo>>(), new List<KeyValuePair<string, CardInfo>>(), new List<Card>());
        FindAnyObjectByType<GameClient>().SendToServer(test);
        Debug.Log("Sent data: HASH = " + test.ToString());
        textOut.text = "OUT: " + test.ToString();
        //END
    }
    */
}
