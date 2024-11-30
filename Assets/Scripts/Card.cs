using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public bool isPlayerCard;
    //Keenan modification
    public CardStats stats;
    [HideInInspector] public int manaCost;
    [HideInInspector] public int attackValue;
    [HideInInspector] public int defenseValue;
    //End
    //matt mods
    [HideInInspector] public string cardName;
    //End
    [HideInInspector] public string cardFaction;
    public bool isInHand = true;


    private static Card selectedCard;
    private static GameObject placementIndicator;
    private static readonly Vector3 playedScale = new Vector3(0.635f, 0.01f, 0.889f);

=======
    public Sprite cardImage;
    private static Card selectedCard;
    private static GameObject placementIndicator;
    private static Vector3 playedScale = new Vector3(1.5f, 1f, 1f);

    private Transform cardPlayArea;
    //Keenan modification
    [HideInInspector] public Faction faction;
    [HideInInspector] public CardType cardType;
    //END
    void Start()
    {
        //Keenan modification
        manaCost = stats.manaCost;
        attackValue = stats.attackValue;
        defenseValue = stats.defenseValue;
        //End
        //mattmods
        cardName = stats.description;
        cardFaction = stats.faction.ToString();
        faction = stats.faction;
        cardType = stats.cardType;
        //end
        cardPlayArea = GameObject.Find("CardPlayArea").transform;
        if (cardPlayArea == null)
        {
            Debug.LogError("CardPlayArea not found in the scene!");
        }

        else
        {
            Debug.LogError("CardPlayArea GameObject not found in the scene.");
        }

        cardPlayAreaGrid = playAreaObject.GetComponent<CardPlayAreaGrid>();

        if (cardImage == null)
        {
            Image imageComponent = GetComponent<Image>();
            if (imageComponent != null)
            {
                cardImage = imageComponent.sprite;
            }
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
            CreatePlacementIndicator();
        }
        else
        {
            Debug.Log($"Not enough mana to play card. Required: {manaCost}, Available: {GameManager.Instance.playerMana}");
        }
    }

    void CreatePlacementIndicator()
    {
        if (placementIndicator != null)
        {
            Destroy(placementIndicator);
        }

        placementIndicator = GameObject.CreatePrimitive(PrimitiveType.Cube);
        if (placementIndicator != null) Destroy(placementIndicator);

        placementIndicator = GameObject.CreatePrimitive(PrimitiveType.Quad);
        placementIndicator.transform.localScale = playedScale;

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
            targetPosition.y = cardPlayArea.position.y + 0.05f;
            placementIndicator.transform.position = targetPosition;
        }
    }

    void PlaceCard()
    {
        if (cardPlayArea == null) return;

        GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        //Keenan addition
        cardObject.name = cardName;
        //END
        cardObject.transform.SetParent(cardPlayArea);
        cardObject.transform.position = placementIndicator.transform.position;
        if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;
        Vector3 closestSlot = cardPlayAreaGrid.FindClosestSlot(placementIndicator.transform.position);
        cardPlayAreaGrid.Remove(closestSlot);

        GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
        cardObject.name = cardName + (FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Count()).ToString() + (FindFirstObjectByType<GameServer>() != null ? "Server" : "Client");
        cardObject.transform.SetParent(cardPlayArea);

        closestSlot.y = .1f;
        cardObject.transform.localPosition = closestSlot;
        cardObject.transform.localScale = playedScale;
        cardObject.transform.rotation = Quaternion.Euler(90, 0, 0);

        Renderer cardRenderer = cardObject.GetComponent<Renderer>();
        Material cardMaterial = new Material(Shader.Find("Unlit/Texture"));

        if (cardImage != null)
        {
            cardMaterial.mainTexture = cardImage.texture;
        }
        else
        {
            cardMaterial = new Material(Shader.Find("Standard"));
            Image cardImageComponent = GetComponent<Image>();
            if (cardImageComponent != null)
            {
                cardMaterial.color = cardImageComponent.color;
            }
            else
            {
                cardMaterial.color = Color.white;
            }
        }
        cardRenderer.material = cardMaterial;

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.manaCost = this.manaCost;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;
        cardInfo.faction = this.faction;
        cardInfo.cardType = this.cardType;

        //Keenan Addition
        CardAttack cardAttack = cardObject.AddComponent<CardAttack>();
        //End

        Renderer cardRenderer = cardObject.GetComponent<Renderer>();
        Image cardImage = GetComponent<Image>();
        if (cardImage != null)
        {
            cardRenderer.material.color = cardImage.color;
        }

        GameManager.Instance.playerMana -= manaCost;
        GameManager.Instance.UpdateManaUI();

        Deck playerDeck = FindFirstObjectByType<Deck>();
        if (playerDeck != null)
        {
            playerDeck.handCards.Remove(this);
        }

        Destroy(gameObject);
        Destroy(placementIndicator);
        selectedCard = null;

        Debug.Log($"Card played successfully at position {cardObject.transform.position}");
    }

        GameManager.Instance.synch.AddPlayedCard(cardObject);
        Debug.Log($"Card played successfully at position {cardObject.transform.position}");
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

//Keenan modification: Moved to other files

