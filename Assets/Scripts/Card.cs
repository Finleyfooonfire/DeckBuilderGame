using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public bool isPlayerCard;
    public CardStats stats;
    [HideInInspector] public int manaCost;
    [HideInInspector] public int attackValue;
    [HideInInspector] public int defenseValue;
    [HideInInspector] public string cardName;
    
    [HideInInspector] public string cardFaction;
    public bool isInHand = true;
    private static Card selectedCard;
    private static GameObject placementIndicator;
    private static readonly Vector3 playedScale = new Vector3(0.635f, 0.01f, 0.889f);
    private Transform cardPlayArea;
    [HideInInspector] public Faction faction;
    [HideInInspector] public CardType cardType;

    CardPlayAreaGrid cardPlayAreaGrid;
    
    void Start()
    {
        manaCost = stats.manaCost;
        attackValue = stats.attackValue;
        defenseValue = stats.defenseValue;
        cardName = stats.description;
        cardFaction = stats.faction.ToString();
        faction = stats.faction;
        cardType = stats.cardType;
        

        GameObject playAreaObject = GameObject.Find("CardPlayArea");
        if (playAreaObject != null)
        {
            cardPlayArea = playAreaObject.transform;
        }
        else
        {
            Debug.LogError("CardPlayArea GameObject not found in the scene.");
        }

        cardPlayAreaGrid = playAreaObject.GetComponent<CardPlayAreaGrid>();
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlayerTurn && isPlayerCard && isInHand) SelectCard();
    }

    

    void SelectCard()
    {
        if (GameManager.Instance.playerMana >= manaCost)
        {
            selectedCard = this;
            CreatePlacementIndicator();
        }
        else
        {
            Debug.Log($"Not enough mana to play card. Required: {manaCost}, Available: {GameManager.Instance.playerMana}");
        }
    }

    void CreatePlacementIndicator()
    {
        if (placementIndicator != null) Destroy(placementIndicator);
        placementIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        placementIndicator.transform.localScale = playedScale;
        placementIndicator.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
        placementIndicator.transform.SetParent(cardPlayArea);
    }

    void Update()
    {
        if (selectedCard == this && placementIndicator != null)
        {
            UpdatePlacementIndicator();
            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject()) PlaceCard();
        }
    }

    void UpdatePlacementIndicator()
    {
        if (Camera.main == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Vector3 closestSlot = FindClosestSlot(hit.point);
            placementIndicator.transform.position = closestSlot;
        }
    }

    void PlaceCard()
    {
        if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;
        Vector3 closestSlot = FindClosestSlot(placementIndicator.transform.position);
        cardPlayAreaGrid.GridSlots.Remove(closestSlot); // Occupy this slot so no other card uses it

        GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Give each card a unique name to allow differentiation.
        cardObject.name = cardName + (FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Count()).ToString() + (FindFirstObjectByType<GameServer>() != null ? "Server" : "Client");
        cardObject.transform.SetParent(cardPlayArea);
        closestSlot.y = .1f;
        cardObject.transform.localPosition = closestSlot;
        cardObject.transform.localScale = playedScale;
        cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.isPlayerCard = this.isPlayerCard;
        cardInfo.manaCost = this.manaCost;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;
        cardInfo.isPlayerCard = this.isPlayerCard;

        CardAttack cardAttack = cardObject.AddComponent<CardAttack>();
        Renderer cardRenderer = cardObject.GetComponent<Renderer>();
        Image cardImage = GetComponent<Image>();
        if (cardImage != null) cardRenderer.material.color = cardImage.color;

        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();
        Deck playerDeck = FindFirstObjectByType<Deck>();
        if (playerDeck != null) playerDeck.handCards.Remove(this);

        Destroy(gameObject);
        Destroy(placementIndicator);
        selectedCard = null;

        GameManager.Instance.synch.AddPlayedCard(cardObject);//Keenan addition
        Debug.Log($"Card played successfully at position {cardObject.transform.position}");
    }

    Vector3 FindClosestSlot(Vector3 currentPosition)
    {
        Vector3 closestSlot = cardPlayAreaGrid.GridSlots[0];
        float shortestDistance = Vector3.Distance(currentPosition, closestSlot);
        foreach (Vector3 slot in cardPlayAreaGrid.GridSlots)
        {
            float distance = Vector3.Distance(currentPosition, slot);
            if (distance < shortestDistance)
            {
                closestSlot = slot;
                shortestDistance = distance;
            }
        }
        return closestSlot;

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
            if (hit.gameObject.layer.Equals(LayerMask.NameToLayer("UI"))) results.Add(hit);
        }
        return results.Count > 0;
    }

    public List<CardInfo> ReadCardsOnTable()
    {
        if (cardPlayArea == null)
        {
            cardPlayArea = GameObject.Find("CardPlayArea")?.transform;
            if (cardPlayArea == null) return new List<CardInfo>();
        }
        List<CardInfo> cardsOnTable = new List<CardInfo>();
        foreach (Transform child in cardPlayArea)
        {
            CardInfo cardInfo = child.GetComponent<CardInfo>();
            if (cardInfo != null) cardsOnTable.Add(cardInfo);
        }
        return cardsOnTable;
    }
}
