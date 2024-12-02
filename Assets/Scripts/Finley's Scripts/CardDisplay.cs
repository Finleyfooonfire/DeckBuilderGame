using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections.Generic;

public class CardDisplay : MonoBehaviour
{
    public GameObject cardPrefab; // The card prefab (UI card) to instantiate
    public Transform contentPanel; // The content panel inside the ScrollRect (for vertical scroll)
    public string cardFolderPath = "BlueCards"; // Correct folder in Resources where the card prefabs are located (no "Assets/Resources")
    public GameObject factionPanelPrefab; // Prefab for the faction panel (for holding horizontal cards)

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

    // Load all card prefabs from a specific folder in Resources
    private CardDetails[] LoadCardsFromResources(string folderPath)
    {
        Debug.Log($"Loading cards from folder: {folderPath}");

        // Load all prefabs from folder
        GameObject[] cardObjects = Resources.LoadAll<GameObject>(folderPath); // folderPath is relative to the Resources folder

        if (cardObjects.Length == 0)
        {
            Debug.LogWarning($"No prefabs found in the {folderPath} folder.");
        }

        List<CardDetails> cards = new List<CardDetails>();

        foreach (GameObject cardObject in cardObjects)
        {
            Debug.Log($"Found prefab: {cardObject.name}");

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
        // Group the cards by their faction
        var factionGroups = cards.GroupBy(card => card.cardFaction);

        if (factionGroups.Count() == 0)
        {
            Debug.LogWarning("No factions found in the loaded cards.");
        }

        foreach (var group in factionGroups)
        {
            Debug.Log($"Displaying faction: {group.Key} with {group.Count()} cards.");

            // Create a faction panel for each group (this will be a vertical section of cards)
            GameObject factionPanel = Instantiate(factionPanelPrefab, contentPanel); // Instantiate faction panel
            Text factionTitle = factionPanel.GetComponentInChildren<Text>(); // Assuming a Text component for the faction name
            factionTitle.text = group.Key; // Set the faction name

            // Add each card from the group to the faction panel
            foreach (CardDetails card in group)
            {
                // Instantiate the card prefab (UI card for displaying)
                GameObject cardInstance = Instantiate(cardPrefab, factionPanel.transform); // Instantiate card prefab
                Image cardImage = cardInstance.GetComponent<Image>(); // Assuming the card prefab has an Image component for display

                if (cardImage != null)
                {
                    // If the card has an image (maybe a 2D sprite representing the card), set it
                    cardImage.sprite = card.cardImage;
                    Debug.Log($"Set image for card: {card.cardName}");
                }
                else
                {
                    Debug.LogWarning($"Card prefab for {card.cardName} does not have an Image component.");
                }

                // Optional: Add card name or other details to the card instance, e.g., using Text component
                Text cardNameText = cardInstance.GetComponentInChildren<Text>(); // Assuming each card prefab has a Text component for displaying name
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName; // Set the name of the card
                }
            }
        }
    }
}
