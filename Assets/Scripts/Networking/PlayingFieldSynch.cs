using System.Collections.Generic;
using UnityEngine;

///TODO: Relating card Changes from client/host into moves: https://www.notion.so/finleyfooonfire/Decomposition-13c4b7e33ee880389e8be96f21928b4c
public class PlayingFieldSynch : MonoBehaviour
{
    [SerializeField] Transform cardPlayArea;
    //Player
    [SerializeField] Transform playerGrave;
    //Opponent
    [SerializeField] Transform opponentGrave;
    [SerializeField] Transform opponentHand;
    [SerializeField] Transform opponentDeck;
    CardPlayAreaGrid cardPlayAreaGrid;

    List<KeyValuePair<string, CardInfo>> playedCards = new List<KeyValuePair<string, CardInfo>>();
    List<KeyValuePair<string, CardInfo>> changedCards = new List<KeyValuePair<string, CardInfo>>();
    List<KeyValuePair<string, CardInfo>> killedCards = new List<KeyValuePair<string, CardInfo>>();
    List<KeyValuePair<string, CardInfo>> killedFriendlyCards = new List<KeyValuePair<string, CardInfo>>();
    List<KeyValuePair<string, CardInfo>> revivedCards = new List<KeyValuePair<string, CardInfo>>();

    HealthAndMana healthChange;

    private void Start()
    {
        cardPlayAreaGrid = cardPlayArea.gameObject.GetComponent<CardPlayAreaGrid>();
    }

    //Call these methods to add the change to the log.
    //Call this when a card has been played
    public void AddPlayedCard(GameObject playedCard)
    {
        playedCards.Add(new KeyValuePair<string, CardInfo>(playedCard.name, playedCard.GetComponent<CardInfo>()));
    }
    //Call this when a cards stats have changed (i.e. a reduction in the defense value)
    public void AddChangedCard(GameObject changedCard)
    {
        changedCards.Add(new KeyValuePair<string, CardInfo>(changedCard.name, changedCard.GetComponent<CardInfo>()));
    }
    //Call this when an enemy card is killed
    public void AddKilledCard(GameObject killedCard)
    {
        if (!killedCard.GetComponent<CardInfo>().invincible)
        {
            killedCards.Add(new KeyValuePair<string, CardInfo>(killedCard.name, killedCard.GetComponent<CardInfo>()));
            if (killedCard.GetComponent<CardInfo>().cardType.Equals(CardType.Spell))
            {
                cardPlayAreaGrid.FreeSpellSlot(killedCard.transform.position);
            }
            else
            {
                cardPlayAreaGrid.FreeSlot(killedCard.transform.position, false);
            }
            killedCard.transform.parent = opponentGrave;
            killedCard.transform.localPosition = new Vector3();
        }
    }
    //Call this when a player's card is killed
    public void AddKilledFriendlyCard(GameObject killedCard)
    {
        if (!killedCard.GetComponent<CardInfo>().invincible)
        {
            killedFriendlyCards.Add(new KeyValuePair<string, CardInfo>(killedCard.name, killedCard.GetComponent<CardInfo>()));
            if (killedCard.GetComponent<CardInfo>().cardType.Equals(CardType.Spell))
            {
                cardPlayAreaGrid.FreeSpellSlot(killedCard.transform.position);
            }
            else
            {
                cardPlayAreaGrid.FreeSlot(killedCard.transform.position, true);
            }
            killedCard.transform.parent = playerGrave;
            killedCard.transform.localPosition = new Vector3();
        }
    }
    //Call this when a card is revived
    public void AddRevivedCard(GameObject revivedCard)
    {
        revivedCards.Add(new KeyValuePair<string, CardInfo>(revivedCard.name, revivedCard.GetComponent<CardInfo>()));
    }


    //Returns a CardsChange with all the things that have changed since the start of the turn.
    CardsChangeIn GetCardStatus()
    {
        CardsChangeIn cardsChange = new CardsChangeIn(playedCards, changedCards, killedCards, killedFriendlyCards, revivedCards);
        return cardsChange;
    }

    public void SetHealthStatus(HealthAndMana changeIn)
    {
        healthChange = changeIn;
    }

    //Uses the client/server to send data to the other device.
    public void SendCardChange()
    {
        CardsChangeIn changes = GetCardStatus();
        var client = FindAnyObjectByType<GameClient>();
        Debug.Log("Sending Card Changes: " + changes);
        if (client != null)
        {
            //The client exists so Send data.
            client.SendCardChange(changes);
        }
        else
        {
            //It is server. Send data.
            FindAnyObjectByType<GameServer>().SendCardChange(changes);
        }
    }

    public void SendHealthAndMana()
    {
        Debug.Log("Sending Stats Changes: " + healthChange);
        var client = FindAnyObjectByType<GameClient>();
        if (client != null)
        {
            //The client exists so Send data.
            client.SendHealthAndMana(healthChange);
        }
        else
        {
            //It is server. Send data.
            FindAnyObjectByType<GameServer>().SendHealthAndMana(healthChange);
        }
    }

    //Uses the client/server to send game end signal to the other device.
    public void GameEnd()
    {
        CardsChangeIn changes = GetCardStatus();
        var client = FindAnyObjectByType<GameClient>();
        if (client != null)
        {
            //The client exists so Send data.
            client.SendEndGame();
        }
        else
        {
            //It is server. Send data.
            FindAnyObjectByType<GameServer>().SendEndGame();
        }
    }

    private void ResetChanges()
    {
        //Reset the changes.
        playedCards.Clear();
        changedCards.Clear();
        killedCards.Clear();
        killedFriendlyCards.Clear();
        revivedCards.Clear();
    }

    //Called by the client/server when data is recieved and acts upon it.
    public void RecieveCards(CardsChangeOut recievedCardsUpdate)
    {
        SetCardStatus(recievedCardsUpdate);
    }
    public void RecieveStats(HealthAndMana healthMana)
    {
        UpdateHealthStatus(healthMana);
    }

    //Updates the board using the changes recieved.
    void SetCardStatus(CardsChangeOut changeIn)
    {
        //Move cards from the hand to the play area
        foreach (KeyValuePair<string, CardInfoStruct> card in changeIn.PlayedCards)
        {
            if (cardPlayArea == null || cardPlayAreaGrid.GridSlots.Count == 0) return;

            string colour = card.Value.manaColour;
            string cardName = card.Key[..card.Key.IndexOf("[ENDOFNAME]")].Replace(" ", string.Empty);
            Debug.Log("Adding from opponent: " + colour + " " + cardName);
            GameObject cardObject = Instantiate(Resources.Load<GameObject>("Cards\\" + colour + "Cards\\" + cardName));
            cardObject.name = card.Key;
            cardObject.transform.SetParent(cardPlayArea);
            cardObject.transform.localPosition = card.Value.position;
            cardObject.transform.rotation = Quaternion.Euler(0, 0, 0);

            CardInfo cardInfo = cardObject.AddComponent<CardInfo>();//Get the card info so that the card type of the card can be used in card placement.

            Vector3 closestSlot = cardPlayAreaGrid.FindClosestSlot(cardObject.transform.position, false, cardInfo.cardType.Equals(CardType.Spell));

            if (card.Value.cardType.Equals(CardType.Spell))
            {
                cardPlayAreaGrid.FillSpellSlot(closestSlot);// Occupy this slot so no other spell card uses it
            }
            else
            {
                cardPlayAreaGrid.FillSlot(closestSlot, card.Value.isPlayerCard);// Occupy this slot so no other card uses it
            }


            cardInfo.manaCost = card.Value.manaCost;
            cardInfo.manaColour = card.Value.manaColour;
            cardInfo.attackValue = card.Value.attackValue;
            cardInfo.defenseValue = card.Value.defenseValue;
            cardInfo.faction = card.Value.faction;
            cardInfo.cardType = card.Value.cardType;
            cardInfo.cardImage = card.Value.cardImage;
            if (cardInfo.cardImage == null)
            {
                Debug.LogError("Card sprite is null");
            }
            //cardObject.GetComponentInChildren<SpriteRenderer>().sprite = cardInfo.cardImage;

            CardAttack cardAttack = cardObject.AddComponent<CardAttack>();
        }

        //Update any cards that have changed. Only the defense value and exhausted variables change.
        foreach (KeyValuePair<string, CardInfoStruct> card in changeIn.ChangedCards)
        {
            CardInfo oldCard = cardPlayArea.Find(card.Key).GetComponent<CardInfo>();
            oldCard.defenseValue = card.Value.defenseValue;
            oldCard.exhausted = card.Value.exhausted;
        }

        //Update any friendly cards that have been killed. (killedCards are friendly cards after being sent over the network)
        foreach (KeyValuePair<string, CardInfoStruct> card in changeIn.KilledCards)
        {
            Transform killedCard = cardPlayArea.Find(card.Key);
            if (card.Value.cardType.Equals(CardType.Spell))
            {
                cardPlayAreaGrid.FreeSpellSlot(killedCard.transform.position);
            }
            else
            {
                cardPlayAreaGrid.FreeSlot(killedCard.transform.position, true);
            }
            killedCard.SetParent(playerGrave);
            killedCard.localPosition = Vector3.zero;
        }

        //Update any opponent cards that have been killed. (friendly cards are the opponent's cards after being sent over the network)
        foreach (KeyValuePair<string, CardInfoStruct> card in changeIn.KilledFriendlyCards)
        {
            Transform killedCard = cardPlayArea.Find(card.Key);
            if (card.Value.cardType.Equals(CardType.Spell))
            {
                cardPlayAreaGrid.FreeSpellSlot(killedCard.transform.position);
            }
            else
            {
                cardPlayAreaGrid.FreeSlot(killedCard.transform.position, false);
            }
            killedCard.SetParent(opponentGrave);
            killedCard.localPosition = Vector3.zero;
        }
        ResetChanges();
        foreach (Transform card in cardPlayArea)
        {
            if (card.GetComponent<CardInfo>().cardType.Equals(CardType.Unit))
            {
                card.GetComponent<CardAttack>().OnUpdateTurn();
            }
        }
    }

    void UpdateHealthStatus(HealthAndMana healthChange)
    {
        GameManager manager = FindFirstObjectByType<GameManager>();
        if (manager == null) return;
        manager.playerMana = healthChange.playerMana;
        manager.opponentMana = healthChange.opponentMana;
        manager.playerLife = healthChange.playerLife;
        manager.opponentLife = healthChange.opponentLife;
    }
}
