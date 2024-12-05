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

        // Go through all the loaded card prefabs and categorize them by faction
        foreach (GameObject cardObject in cardObjects)
        {
            // Extract the path information to determine the faction (the folder name)
            string[] pathParts = cardObject.name.Split('/');

            // The faction name should be the folder the prefab is inside
            string factionName = pathParts[0];

            // Log the faction we're currently working with
            Debug.Log($"Found card: {cardObject.name} in faction: {factionName}");

            // Don't modify the CardDetails component; just read its data
            CardDetails card = cardObject.GetComponent<CardDetails>(); // Get CardDetails script attached to the prefab
            if (card != null)
            {
                // Store the card in the list
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

        if (factionGroups.Count() == 0)
        {
            Debug.LogWarning("No factions found in the loaded cards.");
        }

        foreach (var group in factionGroups)
        {
            Debug.Log($"Displaying faction: {group.Key} with {group.Count()} cards.");

            // Create a faction panel for each group (this will be a horizontal section of cards)
            GameObject factionPanel = Instantiate(factionPanelPrefab, contentPanel); // Instantiate faction panel as a child of contentPanel
            factionPanel.name = group.Key; // Optionally name the faction panel for debugging

            // Find the TextMeshProUGUI component (assuming the faction name is displayed)
            TextMeshProUGUI factionTitle = factionPanel.GetComponentInChildren<TextMeshProUGUI>();
            if (factionTitle != null)
            {
                factionTitle.text = group.Key; // Set the faction name
            }
            else
            {
                Debug.LogWarning($"Faction panel does not have a TextMeshProUGUI component to display the faction name.");
            }

            // Find the content panel inside the faction panel (the place where we want to put the cards)
            Transform cardContainer = factionPanel.transform.Find("ContentPanel"); // Look for the ContentPanel in the faction panel prefab

            if (cardContainer == null)
            {
                Debug.LogWarning($"ContentPanel not found inside {factionPanel.name}, using the faction panel itself.");
                cardContainer = factionPanel.transform; // Use the faction panel itself if no "ContentPanel" is found
            }

            // Add each card from the group to the faction panel's ContentPanel
            foreach (CardDetails card in group)
            {
                // Instantiate the card prefab as a child of the content panel inside the faction panel
                GameObject cardInstance = Instantiate(cardPrefab, cardContainer); // Ensure this is the right container
                Image cardImage = cardInstance.GetComponent<Image>(); // Assuming the card prefab has an Image component for display

                if (cardImage != null)
                {
                    // Set the card image (assuming `card.cardImage` is a sprite)
                    cardImage.sprite = card.cardImage;
                }
                else
                {
                    Debug.LogWarning($"Card prefab for {card.cardName} does not have an Image component.");
                }

                // Optionally add card name or other details to the card prefab using TextMeshProUGUI
                TextMeshProUGUI cardNameText = cardInstance.GetComponentInChildren<TextMeshProUGUI>();
                if (cardNameText != null)
                {
                    cardNameText.text = card.cardName; // Set the name of the card
                }
            }
        }
    }
}
