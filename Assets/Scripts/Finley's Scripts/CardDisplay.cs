using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    public GameObject cardPrefab; // The card prefab (UI card) to instantiate
    public Transform contentPanel; // The content panel inside the ScrollRect (for vertical scroll)
    public string cardFolderPath = "Cards"; // Folder path in Resources containing the factions (no "Assets/Resources")
    public GameObject factionPanelPrefab; // Prefab for the faction panel (for holding horizontal cards)
    public GameObject largeViewCanvas; // Assign the large view canvas in the Inspector
    public Image largeViewImage; // Assign the image component on the large view canvas 


    // Start is called before the first frame update
    private void Start()
    {
        Debug.Log("Start method called - Loading cards from Resources...");

        // Load all the cards from Resources folder
        CardDetails[] allCards = LoadCardsFromResources(cardFolderPath);

        if (allCards.Length == 0)
        {
            Debug.LogWarning("No cards found in the Resources folder.");
        }
        else
        {
            Debug.Log($"Loaded {allCards.Length} cards.");
           
            
        }

        // Group the cards by their faction and display them
        DisplayCardsByFaction(allCards);
    }

    // Load all card prefabs from subfolders inside a given folder in Resources
    private CardDetails[] LoadCardsFromResources(string folderPath)
    {
        Debug.Log($"Loading cards from folder: {folderPath}");

        // Load all prefabs under the given folder (including subfolders)
        GameObject[] cardObjects = Resources.LoadAll<GameObject>(folderPath);

        if (cardObjects.Length == 0)
        {
            Debug.LogWarning($"No prefabs found in the {folderPath} folder.");
        }

        List<CardDetails> cards = new List<CardDetails>();

        foreach (GameObject cardObject in cardObjects)
        {
            CardDetails card = cardObject.GetComponent<CardDetails>(); // Get CardDetails script attached to the prefab
            if (card != null)
            {
                cards.Add(card);
                Debug.Log($"Loaded card: {card.cardName} from prefab: {cardObject.name}");
            }
            else
            {
                Debug.LogWarning($"No CardDetails component found on prefab: {cardObject.name}");
            }
        }

        return cards.ToArray();
    }

    // Display cards grouped by their faction
    private void DisplayCardsByFaction(CardDetails[] cards)
    {
        // Group the cards by their faction using their `cardFaction` property
        var factionGroups = cards.GroupBy(card => card.cardFaction);

        if (!factionGroups.Any())
        {
            Debug.LogWarning("No factions found in the loaded cards.");
        }

        foreach (var group in factionGroups)
        {
            Debug.Log($"Displaying faction: {group.Key} with {group.Count()} cards.");

            // Create a faction panel for each group
            GameObject factionPanel = Instantiate(factionPanelPrefab, contentPanel);
            factionPanel.name = group.Key;

            // Set faction name on the faction panel
            TextMeshProUGUI factionTitle = factionPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (factionTitle != null)
            {
                factionTitle.text = group.Key;
            }
            else
            {
                Debug.LogWarning($"Faction panel does not have a TextMeshProUGUI component to display the faction name.");
            }

            // Get the card container for adding cards
            Transform cardContainer = factionPanel.transform.Find("ContentPanel");
            if (cardContainer == null)
            {
                Debug.LogWarning($"ContentPanel not found inside {factionPanel.name}, using the faction panel itself.");
                cardContainer = factionPanel.transform;
            }

            // Add cards to the faction panel
            foreach (CardDetails card in group)
            {
                GameObject cardInstance = Instantiate(cardPrefab, cardContainer);

                // Add a CardDetails component dynamically if it doesn't exist
                CardDetails cardDetailsInstance = cardInstance.GetComponent<CardDetails>();
                if (cardDetailsInstance == null)
                {
                    cardDetailsInstance = cardInstance.AddComponent<CardDetails>();
                }

                // Copy the CardDetails data
                CopyCardDetails(card, cardDetailsInstance);

                // Set the card image
                Image cardImage = cardInstance.GetComponent<Image>();
                if (cardImage != null && card.cardImage != null)
                {
                    cardImage.sprite = card.cardImage;
                }

                // Set the card name
                TextMeshProUGUI cardNameText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName;
                }
            }
        }
    }

    // Copies the data from the original CardDetails to the instantiated CardDetails
    private void CopyCardDetails(CardDetails source, CardDetails destination)
    {
        destination.cardName = source.cardName;
        destination.cardFaction = source.cardFaction;
        destination.cardImage = source.cardImage;
        destination.cardDescription = source.cardDescription;
        // Add any additional properties you want to copy here
        Debug.Log($"Copied CardDetails from {source.cardName} to {destination.gameObject.name}.");
    }
}
