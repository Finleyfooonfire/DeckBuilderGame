using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public bool isPlayerCard;
    public CardStats stats;  // Card stats from ScriptableObject

    [HideInInspector] public int manaCost;
    [HideInInspector] public int attackValue;
    [HideInInspector] public int defenseValue;
    [HideInInspector] public string cardName;
    [HideInInspector] public string cardFaction;
    public bool isInHand = true;

    private static Card selectedCard;
    private static GameObject placementIndicator;
    private Transform cardPlayArea;

    [HideInInspector] public Faction faction;
    [HideInInspector] public CardType cardType;

    void Start()
    {
        // Initialize card properties from CardStats
        manaCost = stats.manaCost;
        attackValue = stats.attackValue;
        defenseValue = stats.defenseValue;
        cardName = stats.description;
        cardFaction = stats.faction.ToString();
        faction = stats.faction;
        cardType = stats.cardType;

        cardPlayArea = GameObject.Find("CardPlayArea")?.transform;
        if (cardPlayArea == null)
        {
            Debug.LogError("CardPlayArea not found in the scene!");
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlayerTurn && isPlayerCard && isInHand)
        {
            SelectCard();
        }
    }

    void SelectCard()
    {
        if (GameManager.Instance.playerMana >= manaCost)
        {
            selectedCard = this;
            CreatePlacementIndicator();  // Keeps the indicator for card placement
        }
        else
        {
            Debug.Log($"Not enough mana to play card. Required: {manaCost}, Available: {GameManager.Instance.playerMana}");
        }
    }

    void CreatePlacementIndicator()
    {
        // If you want a placement indicator, use the cube, otherwise, you can remove it
        if (placementIndicator != null)
        {
            Destroy(placementIndicator);
        }

        placementIndicator = Instantiate(stats.cardPrefab, Vector3.zero, Quaternion.identity);
        placementIndicator.transform.localScale = new Vector3(1f, 1f, 1f);  // Placeholder for card scale
        //placementIndicator.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
        placementIndicator.transform.SetParent(cardPlayArea);
    }

    void Update()
    {
        if (selectedCard == this && placementIndicator != null)
        {
            UpdatePlacementIndicator();

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
            {
                PlaceCard();
            }
        }
    }

    void UpdatePlacementIndicator()
    {
        if (Camera.main == null)
        {
            Debug.LogError("No main camera found in the scene!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = cardPlayArea.position.y + 0.05f;  // Ensure it hovers above the card play area
            placementIndicator.transform.position = targetPosition;
        }
    }

    void PlaceCard()
    {
        if (cardPlayArea == null || stats.cardPrefab == null) return;

        // Instantiate the card prefab from CardStats at the position of the placement indicator
        GameObject cardObject = Instantiate(stats.cardPrefab, placementIndicator.transform.position, Quaternion.identity);
        cardObject.transform.SetParent(cardPlayArea);

        // Set the name of the card (optional, for easier identification in the hierarchy)
        cardObject.name = cardName;

        // Optionally, apply scaling and other adjustments to the card
        cardObject.transform.localScale = new Vector3(1f, 1f, 1f);  // Adjust the size if necessary

        // Add gameplay components like CardInfo, which are needed for the card's stats and behavior
        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.manaCost = this.manaCost;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;

        // Optionally add other gameplay components (e.g., CardAttack)
        CardAttack cardAttack = cardObject.AddComponent<CardAttack>();

        // Deduct mana and update UI
        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();

        // Remove the card from the player's hand
        Deck playerDeck = FindFirstObjectByType<Deck>();
        if (playerDeck != null)
        {
            playerDeck.handCards.Remove(this);
        }

        // Destroy the card from hand and remove the placement indicator
        Destroy(gameObject);   // Remove the original card object (this card)
        Destroy(placementIndicator);  // Remove the placement indicator
        selectedCard = null;   // Reset the selected card

        Debug.Log($"Card played successfully at position {cardObject.transform.position}");
    }


    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, hits);
        List<RaycastResult> results = new List<RaycastResult>();
        foreach (var hit in hits)
        {
            if (hit.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
            {
                results.Add(hit);
            }
        }
        return results.Count > 0;
    }

    public List<CardInfo> ReadCardsOnTable()
    {
        if (cardPlayArea == null)
        {
            cardPlayArea = GameObject.Find("CardPlayArea")?.transform;
            if (cardPlayArea == null)
            {
                Debug.LogError("CardPlayArea not found in the scene!");
                return new List<CardInfo>();
            }
        }

        List<CardInfo> cardsOnTable = new List<CardInfo>();
        foreach (Transform child in cardPlayArea)
        {
            CardInfo cardInfo = child.GetComponent<CardInfo>();
            if (cardInfo != null)
            {
                cardsOnTable.Add(cardInfo);
            }
        }
        return cardsOnTable;
    }
}
