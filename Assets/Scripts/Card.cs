using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Card : MonoBehaviour, IPointerClickHandler
{
    public bool isPlayerCard;
    //Keenan modification
    public CardStats stats;
    [HideInInspector] public int manaCost;
    [HideInInspector] public int attackValue;
    [HideInInspector] public int defenseValue;
    //End
    public bool isInHand = true;

    private static Card selectedCard;
    private static GameObject placementIndicator;
    private static Vector3 playedScale = new Vector3(1f, 0.1f, 1.5f);

    private Transform cardPlayArea;

    void Start()
    {
        //Keenan modification
        manaCost = stats.manaCost;
        attackValue = stats.attackValue;
        defenseValue = stats.defenseValue;
        //End
        cardPlayArea = GameObject.Find("CardPlayArea").transform;
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
        placementIndicator.transform.localScale = playedScale;
        placementIndicator.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 0.5f);
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
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = cardPlayArea.position.y + 0.05f; // Above CPA
            placementIndicator.transform.position = targetPosition;
        }
    }

    void PlaceCard()
    {
        GameObject cardObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cardObject.transform.SetParent(cardPlayArea);
        cardObject.transform.position = placementIndicator.transform.position;
        cardObject.transform.localScale = playedScale;
        cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        Renderer cardRenderer = cardObject.GetComponent<Renderer>();
        Image cardImage = GetComponent<Image>();
        if (cardImage != null)
        {
            cardRenderer.material.color = cardImage.color;
        }

        CardInfo cardInfo = cardObject.AddComponent<CardInfo>();
        cardInfo.manaCost = this.manaCost;
        cardInfo.attackValue = this.attackValue;
        cardInfo.defenseValue = this.defenseValue;

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

    bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        System.Collections.Generic.List<RaycastResult> results = new System.Collections.Generic.List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}

public class CardInfo : MonoBehaviour
{
    public int manaCost;
    public int attackValue;
    public int defenseValue;
}