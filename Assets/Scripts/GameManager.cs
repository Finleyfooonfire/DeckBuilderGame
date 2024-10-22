using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public TextMeshProUGUI statusText;
    public TextMeshProUGUI playerManaText;
    public TextMeshProUGUI opponentManaText;
    public TextMeshProUGUI playerLifeText;
    public TextMeshProUGUI opponentLifeText;

    public Button endTurnButton;

    public bool isPlayerTurn;
    public int playerMana = 0;
    public int opponentMana = 0;
    public int maxMana = 10;
    public int playerLife = 20;
    public int opponentLife = 20;
    public Transform playerField;
    public Transform opponentField;
    public Card selectedAttackingCard;
    private int turnsTaken = 0;
    private int damageDealt = 0;
    //matt additions
    public string OnTable = "";
    //end

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        UpdateLifeUI();
        UpdateManaUI();

        endTurnButton.onClick.AddListener(EndTurn);

        CoinFlip();
    }

    void CoinFlip()
    {
        isPlayerTurn = Random.Range(0, 2) == 0;
        UpdateTurn();
    }

    void UpdateTurn()
    {
        if (isPlayerTurn)
        {
            playerMana = Mathf.Min(playerMana + 1, maxMana);
            UpdateManaUI();

            Deck playerDeck = GameObject.Find("PlayerDeck").GetComponent<Deck>();
            playerDeck.DrawCard();

            statusText.text = "Your Turn";
            endTurnButton.interactable = true;
        }
        else
        {
            opponentMana = Mathf.Min(opponentMana + 1, maxMana);
            UpdateManaUI();

            Deck opponentDeck = GameObject.Find("OpponentDeck").GetComponent<Deck>();
            opponentDeck.DrawCard();

            statusText.text = "Opponent's Turn";
            endTurnButton.interactable = false;

            //StartCoroutine(AITurn());
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        turnsTaken++;
        selectedAttackingCard = null;
        UpdateTurn();

        if (isPlayerTurn)
        {
            DrawCard();
        }

        UpdateTurn();
    }

    void DrawCard()
    {
        Deck playerDeck = GameObject.Find("PlayerDeck").GetComponent<Deck>();
        if (playerDeck != null)
        {
            playerDeck.DrawCard();
        }
        else
        {
            Debug.LogError("PlayerDeck not found!");
        }
    }

    public void UpdateManaUI()
    {
        playerManaText.text = "Mana: " + playerMana + "/" + maxMana;
        opponentManaText.text = "Mana: " + opponentMana + "/" + maxMana;
    }

    void UpdateLifeUI()
    {
        playerLifeText.text = "Life: " + playerLife;
        opponentLifeText.text = "Life: " + opponentLife;
    }

    public void SelectAttackingCard(Card card)
    {
        if (isPlayerTurn && card.isPlayerCard && !card.isInHand)
        {
            if (selectedAttackingCard == card)
            {
                selectedAttackingCard = null;
            }
            else
            {
                selectedAttackingCard = card;
            }
        }
    }

    public void Attack(Card targetCard)
    {
        if (selectedAttackingCard != null && targetCard != null && !targetCard.isPlayerCard && !targetCard.isInHand)
        {
            targetCard.defenseValue -= selectedAttackingCard.attackValue;
            selectedAttackingCard.defenseValue -= targetCard.attackValue;

            damageDealt += selectedAttackingCard.attackValue;

            if (targetCard.defenseValue <= 0)
            {
                Destroy(targetCard.gameObject);
            }
            if (selectedAttackingCard.defenseValue <= 0)
            {
                Destroy(selectedAttackingCard.gameObject);
            }

            selectedAttackingCard = null;
        }
    }

    public void AttackPlayerDirectly()
    {
        if (selectedAttackingCard != null)
        {
            opponentLife -= selectedAttackingCard.attackValue;
            UpdateLifeUI();

            damageDealt += selectedAttackingCard.attackValue;

            if (opponentLife <= 0)
            {
                GameOver(true);
            }

            selectedAttackingCard = null;
        }
    }

    public void onTable()
    {

    }



   // IEnumerator AITurn()
  //  {
        // Simple AI logic for testing
      //  yield return new WaitForSeconds(2f);

  //      Debug.Log("AI forfeits its turn.");

   //     EndTurn();
  //  }

    public void GameOver(bool playerWon)
    {
        PlayerPrefs.SetInt("PlayerWon", playerWon ? 1 : 0);
        PlayerPrefs.SetInt("TurnsTaken", turnsTaken);
        PlayerPrefs.SetInt("DamageDealt", damageDealt);

        SceneManager.LoadScene("ScoreboardScene");
    }

    //KEENAN: Gets all the cards present.
    public Card[] GetAllCards()
    {
        return FindObjectsByType<Card>(FindObjectsSortMode.InstanceID);
    }
}
