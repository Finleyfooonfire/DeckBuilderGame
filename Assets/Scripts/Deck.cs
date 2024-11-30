using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> deckCards = new List<Card>();
    public List<Card> handCards = new List<Card>();

    public Transform handPosition; // Position where cards in hand are displayed
    public bool isPlayerDeck;

    void Start()
    {
        InitializeDeck();
        DrawStartingHand();
    }

    void InitializeDeck()
    {
        for (int i = 0; i < 20; i++)
        {
            //Keenan modification
            GameObject cardObj = Instantiate(Resources.Load("CardPrefab"), transform) as GameObject;
            cardObj.transform.localPosition = new Vector3();
            cardObj.transform.localRotation = Quaternion.Euler(0, 0, 180);
            //End
            Card card = cardObj.GetComponent<Card>();
            card.isPlayerCard = isPlayerDeck;
            deckCards.Add(card);
            //Keenan remove line: cardObj.SetActive(false);
        }
        //Keenan addition
        DistributeHand();
        //END
    }

    public void DrawStartingHand()
    {
        for (int i = 0; i < 6; i++)
        {
            DrawCard();
        }
    }

    public void DrawCard()
    {
        if (deckCards.Count > 0)
        {
            Card drawnCard = deckCards[0];
            deckCards.RemoveAt(0);
            handCards.Add(drawnCard);
            drawnCard.isInHand = true;

            drawnCard.transform.SetParent(handPosition);

            //Keenan addition
            drawnCard.transform.rotation = new Quaternion();
            DistributeHand();
            //END

            //Keenan remove line: drawnCard.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("No more cards in the deck!");
        }
    }

    //Keenan addition
    void DistributeHand()
    {
        handCards.RemoveAll(card => card == null);
        int index = 0;
        foreach (Card handCard in handCards)
        {
            handCard.transform.localPosition = new Vector3((index - handCards.Count/2f), 0, 0);
            index++;
        }
    }
    //END
}
