using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public bool isPlayerCard;
    //Keenan modification
    public CardStats stats;
    [HideInInspector] public int manaCost;
    [HideInInspector] public string manaColour;
    [HideInInspector] public int attackValue;
    [HideInInspector] public int defenseValue;
    //End
    //matt mods
    [HideInInspector] public string cardName;
    //End
    [HideInInspector] public string cardFaction;
    [HideInInspector] public string cardDescription;
    public TextMeshPro descriptionText;
    public bool isInHand = true;
    private Transform PlayerHand;

    public Sprite cardImage;

    private static Card selectedCard;
    private static GameObject placementIndicator;
    private static Vector3 playedScale = new Vector3(0.635f, 0.01f, 0.889f);

    private Transform cardPlayArea;
    //Keenan modification
    [HideInInspector] public Faction faction;
    [HideInInspector] public CardType cardType;

    private static GameObject zoomPanel;
    private static bool isZoomed = false;
    private static Card hoveredCard;
    private static Vector3 defaultZoomPosition = new Vector3(0, 0, -5);

    CardPlayAreaGrid cardPlayAreaGrid;
    //END
    void Start()
    {
        //Keenan modification
        manaCost = stats.manaCost;
        manaColour = stats.manaTypeRequired;
        attackValue = stats.attackValue;
        defenseValue = stats.defenseValue;
        //End
        //mattmods
        cardName = stats.description;
        cardFaction = stats.faction.ToString();

        cardImage = stats.cardImage;


        faction = stats.faction;
        cardType = stats.cardType;

        //end
        cardPlayArea = GameObject.Find("CardPlayArea").transform;
        PlayerHand = GameObject.Find("PlayerHand").transform;
        if (cardPlayArea == null)
        {
            //Debug.LogError("CardPlayArea not found in the scene!");
        }


        cardPlayAreaGrid = cardPlayArea.GetComponent<CardPlayAreaGrid>();

        if (cardImage == null)
        {
            Image imageComponent = GetComponent<Image>();
            if (imageComponent != null)
            {
                cardImage = imageComponent.sprite;
            }
        }

        if (zoomPanel == null)
        {
            CreateZoomPanel();
        }
        
    }



    void CreateZoomPanel()
    {
        zoomPanel = new GameObject("ZoomPanel");
        zoomPanel.transform.SetParent(GameObject.Find("GameUI").transform, false);
        RectTransform rectTransform = zoomPanel.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(300, 400);

        Image image = zoomPanel.AddComponent<Image>();
        image.raycastTarget = false;

        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        rectTransform.anchoredPosition = Vector2.zero;

        zoomPanel.SetActive(false);
        // matt additions
        GameObject textObject = new GameObject("CardText");
        textObject.transform.SetParent(zoomPanel.transform, false);
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.sizeDelta = new Vector2(300, 50);
        textRect.anchorMin = new Vector2(0.5f, 0);
        textRect.anchorMax = new Vector2(0.5f, 0);
        textRect.pivot = new Vector2(0.5f, 0);
        textRect.anchoredPosition = new Vector2(0, 25);
        TextMeshProUGUI cardText = textObject.AddComponent<TextMeshProUGUI>();

        cardText.color = Color.black;
        // matt addition end
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoveredCard = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hoveredCard == this)
        {
            hoveredCard = null;
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
            //Debug.Log($"{selectedCard.cardType} card selected");

            CreatePlacementIndicator();
        }
        else
        {
            //Debug.Log($"Not enough mana to play card. Required: {manaCost}, Available: {GameManager.Instance.playerMana}");
        }
    }

    void CreatePlacementIndicator()
    {

        if (placementIndicator != null) Destroy(placementIndicator);

        placementIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);

        placementIndicator.transform.localScale = playedScale;
        placementIndicator.transform.rotation = Quaternion.Euler(0, 0, 0);

        Renderer renderer = placementIndicator.GetComponent<Renderer>();
        Material material = new Material(Shader.Find("Transparent/Diffuse"));
        material.color = new Color(1f, 1f, 1f, 0.5f);
        if (cardImage != null)
        {
            material.mainTexture = cardImage.texture;
        }
        renderer.material = material;

        placementIndicator.transform.SetParent(cardPlayArea);
    }

    void Update()
    {

        if (selectedCard == this && placementIndicator != null)
        {
            UpdatePlacementIndicator();

            if (Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
            {
                switch (selectedCard.cardType)
                {
                    case CardType.Spell:
                        //Spell cards are special
                        //Debug.Log("Spell card placed");
                        PlaceSpellCard();
                        break;
                    case CardType.Unit:
                        //Unit and land cards are to be placed normally
                        //Debug.Log(selectedCard.cardType.ToString() + " card placed");
                        PlaceUnitCard();
                        break;
                    case CardType.Land:
                        //Unit and land cards are to be placed normally
                        PlaceLandCard();
                        break;
                }
            }
        }


        if (hoveredCard != null)
        {
            if (Input.GetMouseButtonDown(1) && !isZoomed)
            {
                ShowZoomedCard(hoveredCard);
            }
        }

        if (isZoomed && Input.GetMouseButtonDown(0))
        {
            HideZoomedCard();
        }
    }

    void ShowZoomedCard(Card card)
    {
        if (zoomPanel != null)
        {
            Image panelImage = zoomPanel.GetComponent<Image>();
            TextMeshProUGUI panelText = zoomPanel.GetComponentInChildren<TextMeshProUGUI>();
      

            // Pass the selected card's GameObject to AssignCardDescription
            AssignCardDescription(card.gameObject);

            if (panelImage != null)
            {
                panelText.enableAutoSizing = true;
                panelText.fontSizeMin = 10; // Minimum font size
                panelText.fontSizeMax = 40; // Maximum font size
                panelText.text = descriptionText.text;
                panelImage.sprite = card.cardImage;
                zoomPanel.SetActive(true);
                isZoomed = true;
            }
            if (panelText != null && descriptionText != null)
            {
                panelText.text = descriptionText.text;
                
            }
        }
    }

    public void AssignCardDescription(GameObject selectedCard)
    {
        string GetFullPath(Transform obj)
        {
            if (obj.parent == null) return obj.name;
            return $"{GetFullPath(obj.parent)}/{obj.name}";
        }

        // Find the "Description" GameObject in the selected card
        GameObject descriptionObject = FindNestedGameObject(selectedCard.transform, "Description");

        if (descriptionObject != null)
        {
            Debug.Log($"Found object: {descriptionObject.name}, Path: {GetFullPath(descriptionObject.transform)}");

            Component[] components = descriptionObject.GetComponents<Component>();
            foreach (var component in components)
            {
                Debug.Log($"Component: {component.GetType()}");
            }

            descriptionText = descriptionObject.GetComponent<TextMeshPro>();
            if (descriptionText != null)
            {
                Debug.Log($"Description Text: {descriptionText.text}");

                // Update Player Deck's Description
                Transform playerDeckDescription = FindNestedTransform(selectedCard.transform, "Description");

                if (playerDeckDescription != null)
                {
                    Debug.Log($"Player Deck Description Name: {playerDeckDescription.name}");
                    TextMeshPro playerDeckText = playerDeckDescription.GetComponent<TextMeshPro>();
                    if (playerDeckText != null)
                    {
                        Debug.Log($"Player Deck Text: {playerDeckText.text}");
                        playerDeckText.text = descriptionText.text;
                    }
                }
                else
                {
                    Debug.LogError("Description child not found in PlayerDeck!");
                }
            }
            else
            {
                Debug.LogError("TextMeshProUGUI component not found in Description GameObject!");
            }
        }
        else
        {
            Debug.LogError("Description GameObject not found in the selected card!");
        }
    }



    GameObject FindNestedGameObject(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child.gameObject;
                }
                GameObject result = FindNestedGameObject(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }

        Transform FindNestedTransform(Transform parent, string name)
        {
            foreach (Transform child in parent)
            {
                if (child.name == name)
                {
                    return child;
                }
                Transform result = FindNestedTransform(child, name);
                if (result != null)
                {
                    return result;
                }
            }
            return null;
        }
    




void HideZoomedCard()
    {
        if (zoomPanel != null)
        {
            zoomPanel.SetActive(false);
            isZoomed = false;
        }
    }

    void UpdatePlacementIndicator()
    {
        if (Camera.main == null)
        {
            //Debug.LogError("No main camera found in the scene!");
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = cardPlayArea.position.y + 0.05f;
            placementIndicator.transform.position = targetPosition;
        }
    }
    //Places a non-spell card type card into an empty space on the player's side of the board.
    void PlaceUnitCard()
    {

        if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;
        Vector3 closestSlot = cardPlayAreaGrid.FindClosestSlot(placementIndicator.transform.position, true);
        cardPlayAreaGrid.FillSlot(closestSlot, true);

        GameObject cardObject = gameObject;
        cardObject.name = cardName + "[ENDOFNAME]" + (FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Count()).ToString() + (FindFirstObjectByType<GameServer>() != null ? "Server" : "Client");//The substring "[ENDOFNAME]" is used in PlayingFieldSynch.cs to isolate the card type name from the individual card name so that the proper prefab can be referenced.
        cardObject.transform.SetParent(cardPlayArea);

        closestSlot.y = .1f;
        cardObject.transform.localPosition = closestSlot;

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.isPlayerCard = this.isPlayerCard;
        cardInfo.manaCost = this.manaCost;
        cardInfo.manaColour = this.manaColour;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;
        cardInfo.cardImage = this.cardImage;

        //Keenan Addition
        CardAttack cardAttack = cardObject.AddComponent<CardAttack>();


        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();

        FindObjectsByType<Deck>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "PlayerDeck").ToArray()[0].PlayCard(this);
        Destroy(placementIndicator);
        selectedCard = null;

        GameManager.Instance.synch.AddPlayedCard(cardObject);
        //Debug.Log($"Non spell card played successfully at position {cardObject.transform.position}");
    }

    //Places a spell card type card onto a space on the player's side of the board that has a different card on it.
    void PlaceSpellCard()
    {
        if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;
        Vector3 closestSlot = cardPlayAreaGrid.FindClosestSlot(placementIndicator.transform.position, true, true);//Finds the closest slot with a card that can accept spell cards in it.
        cardPlayAreaGrid.FillSpellSlot(closestSlot);

        GameObject cardObject = gameObject;
        cardObject.name = cardName + "[ENDOFNAME]" + (FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Count()).ToString() + (FindFirstObjectByType<GameServer>() != null ? "Server" : "Client");//The substring "[ENDOFNAME]" is used in PlayingFieldSynch.cs to isolate the card type name from the individual card name so that the proper prefab can be referenced.
        cardObject.transform.SetParent(cardPlayArea);

        closestSlot.y = .05f;//Place the card physically bellow the card it is modulating
        cardObject.transform.localPosition = closestSlot;

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.isPlayerCard = this.isPlayerCard;
        cardInfo.manaCost = this.manaCost;
        cardInfo.manaColour = this.manaColour;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;
        cardInfo.cardImage = this.cardImage;


        CardSpell cardSpell;
        switch (cardName)
        {
            case "Poison Drop":
                cardSpell = cardObject.AddComponent<PoisonDropSpell>();
                break;
            case "March Of Judgement":
                cardSpell = cardObject.AddComponent<MarchSpell>();
                break;
            case "Last wish of a dying star":
                cardSpell = cardObject.AddComponent<WishSpell>();
                break;
            default:
                //Debug.LogError($"NO SPELL TYPE MATCHES GIVEN SPELL: {cardInfo.spell}");
                break;
        }

        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();

        FindObjectsByType<Deck>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "PlayerDeck").ToArray()[0].PlayCard(this);
        Destroy(placementIndicator);
        selectedCard = null;

        GameManager.Instance.synch.AddPlayedCard(cardObject);
        //Debug.Log($"Spell card played successfully at position {cardObject.transform.position}");
    }

    void PlaceLandCard()
    {
        if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;
        Vector3 closestSlot = cardPlayAreaGrid.FindClosestSlot(placementIndicator.transform.position, true);
        cardPlayAreaGrid.FillSlot(closestSlot, true);

        GameObject cardObject = gameObject;
        cardObject.name = cardName + "[ENDOFNAME]" + (FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Count()).ToString() + (FindFirstObjectByType<GameServer>() != null ? "Server" : "Client");//The substring "[ENDOFNAME]" is used in PlayingFieldSynch.cs to isolate the card type name from the individual card name so that the proper prefab can be referenced.
        cardObject.transform.SetParent(cardPlayArea);

        closestSlot.y = .1f;
        cardObject.transform.localPosition = closestSlot;

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.isPlayerCard = this.isPlayerCard;
        cardInfo.manaCost = this.manaCost;
        cardInfo.manaColour = this.manaColour;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;
        cardInfo.cardImage = this.cardImage;

        //Keenan Addition
        CardGenerate cardAttack = cardObject.AddComponent<CardGenerate>();


        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();

        FindObjectsByType<Deck>(FindObjectsSortMode.None).Where(x => x.gameObject.name == "PlayerDeck").ToArray()[0].PlayCard(this);
        Destroy(placementIndicator);
        selectedCard = null;

        GameManager.Instance.synch.AddPlayedCard(cardObject);
    }


    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        //Keenan modification
        List<RaycastResult> hits = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, hits);
        List<RaycastResult> results = new List<RaycastResult>();
        foreach (var hit in hits)
        {
            //Only get UI objects and not all objects.
            if (hit.gameObject.layer.Equals(LayerMask.NameToLayer("UI")))
            {
                results.Add(hit);
            }
        }
        //END
        return results.Count > 0;
    }

    public List<CardInfo> ReadCardsOnTable()
    {
        if (cardPlayArea == null)
        {
            cardPlayArea = GameObject.Find("CardPlayArea")?.transform;
            if (cardPlayArea == null)
            {
                //Debug.LogError("CardPlayArea not found in the scene!");
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

//Keenan modification: Moved to other files

