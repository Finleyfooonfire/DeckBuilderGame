using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardpanel; 
    public string cardpath = "Cards";
    public GameObject factiongroupingpanel; 
    public GameObject largeViewCanvas; 
    public Image largeViewImage; 

    private void Start()
    {
        //Debug.Log("load cards");
        CardDetails[] allCards = LoadCardsFromResources(cardpath);

        if (allCards.Length == 0)
        {
            Debug.LogWarning("no cards in folder");
        }
        else
        {
            //Debug.Log($"loaded all the cards");
        }
        DisplayCardsByFaction(allCards);
    }

    private CardDetails[] LoadCardsFromResources(string folderPath)
    {

        Debug.Log($"loading cards {folderPath}");
        GameObject[] cardObjects = Resources.LoadAll<GameObject>(folderPath);
        if (cardObjects.Length == 0)
        {
            //Debug.LogWarning($"nothing found in prefab folder");
        }
        List<CardDetails> cards = new List<CardDetails>();
        foreach (GameObject cardObject in cardObjects)
        {
            CardDetails card = cardObject.GetComponent<CardDetails>(); 
            if (card != null)
            {
                cards.Add(card);
                Debug.Log($"card has been loaded");
            }
            else
            {
                Debug.LogWarning($"card details not found");
            }
        }
        return cards.ToArray();
    }

    private void DisplayCardsByFaction(CardDetails[] cards)
    {
        var factionGroups = cards.GroupBy(card => card.cardFaction);

        if (!factionGroups.Any())
        {
           // Debug.LogWarning("no factions found in details");
        }

        foreach (var group in factionGroups)
        {
            //Debug.Log($"displaying a faction");
            GameObject factionPanel = Instantiate(factiongroupingpanel, cardpanel);
            factionPanel.name = group.Key;
            TextMeshProUGUI factionTitle = factionPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (factionTitle != null)
            {
                factionTitle.text = group.Key;
            }
            else
            {
                //Debug.LogWarning($"faction panel isnt working");
            }

            Transform cardContainer = factionPanel.transform.Find("ContentPanel");
            if (cardContainer == null)
            {
               // Debug.LogWarning($"content panel isn't found");
                cardContainer = factionPanel.transform;
            }
            foreach (CardDetails card in group)
            {
                GameObject cardInstance = Instantiate(cardPrefab, cardContainer);
                CardDetails cardDetailsInstance = cardInstance.GetComponent<CardDetails>();
                if (cardDetailsInstance == null)
                {
                    cardDetailsInstance = cardInstance.AddComponent<CardDetails>();
                }
                CopyCardDetails(card, cardDetailsInstance);
                Image cardImage = cardInstance.GetComponent<Image>();
                if (cardImage != null && card.cardImage != null)
                {
                    cardImage.sprite = card.cardImage;
                }
                TextMeshProUGUI cardNameText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                }
            }
        }
    }

    private void CopyCardDetails(CardDetails source, CardDetails destination)
    {
        destination.cardName = source.cardName;
        destination.cardFaction = source.cardFaction;
        destination.cardImage = source.cardImage;
        destination.cardDescription = source.cardDescription;
        //Debug.Log($"carddetails: {source.cardName} to {destination.gameObject.name}.");
    }
}
