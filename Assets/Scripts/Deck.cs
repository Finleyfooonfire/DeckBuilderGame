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
            GameObject cardObj = Instantiate(Resources.Load("CardPrefab")) as GameObject;
            Card card = cardObj.GetComponent<Card>();
            card.isPlayerCard = isPlayerDeck;
            deckCards.Add(card);
            cardObj.SetActive(false);
        }
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

            drawnCard.transform.SetParent(handPosition);

            drawnCard.gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("No more cards in the deck!");
        }
    }
}
