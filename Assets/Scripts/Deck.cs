using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<Card> deckCards = new List<Card>();
    public List<Card> handCards = new List<Card>();

    public Transform handPosition; // Position where cards in hand are displayed
    public bool isPlayerDeck;

    [SerializeField] int maxHandSize = 6;
    [SerializeField] CardStats[] possibleCards;//Temporary variable. Set the card for all the cards in the deck here.

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
            CardStats cardStats = possibleCards[Random.Range(0, possibleCards.Length - 1)];
            GameObject cardObj = Instantiate<GameObject>(cardStats.cardPrefab, transform);

            cardObj.transform.localPosition = new Vector3();
            cardObj.transform.localRotation = Quaternion.Euler(90, 180, 0);

            Card card = cardObj.AddComponent<Card>();
            card.stats = cardStats;
            //End
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
        if (deckCards.Count == 0)
        {
            Debug.Log("No more cards in the deck!");
            return;
        }
        if (handCards.Count < maxHandSize)
        {
            Card drawnCard = deckCards[0];
            deckCards.RemoveAt(0);
            deckCards.TrimExcess();
            if (handCards.Contains(null))
            {
                handCards[handCards.FindIndex(null)] = drawnCard;
            }
            else
            {
                handCards.Add(drawnCard);
            }
            drawnCard.isInHand = true;

            drawnCard.transform.SetParent(handPosition);

            drawnCard.transform.rotation = Quaternion.Euler(-90, -180, 0);
            DistributeHand();
        }
        else
        {
            Debug.Log("Hand full");
        }
    }

    public void DistributeHand()
    {
        for (int i = handCards.Count - 1; i >= 0; i--)
        {
            if (handCards[i] == null)
            {
                handCards.RemoveAt(i);
            }
        }
        Debug.Log("Distributing hand: " + string.Join<Card>(",", handCards.ToArray()));
        int index = 0;
        foreach (Card handCard in handCards)
        {
            handCard.transform.localPosition = new Vector3((index - handCards.Count / 2f), 0, 0);
            index++;
        }
    }

    public void PlayCard(Card card)
    {
        if (handCards.Remove(card)) Debug.Log($"The card {card} has been removed from handCards.");
        Destroy(card);
        DistributeHand();
    }
}
