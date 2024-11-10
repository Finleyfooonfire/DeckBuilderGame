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
    public CardAttack selectedAttackingCard;//Keenan modification
    private int turnsTaken = 0;
    private int damageDealt = 0;
    //matt additions
    public string OnTable = "";
    //end
    //Keenan addition
    PlayingFieldSynch synch;
    //END 
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
        synch = FindAnyObjectByType<PlayingFieldSynch>();
        UpdateLifeUI();
        UpdateManaUI();

        endTurnButton.onClick.AddListener(EndTurn);
    }

    //Call this when the client and server are connected.
    //Keenan modification
    public void StartGame(bool isHost)
    {
        //Only if the user is a server. Do the coinflip and tell the other device the outcome.
        if (isHost)
        {
            CoinFlip();
        }
        //Otherwise get who starts via the network.
        else
        {
            synch.RecieveSynchroniseDevices();
            UpdateTurn();
        }
    }
    //End

    void CoinFlip()
    {
        //Keenan addition
        Random.InitState((int)Time.time);
        //End
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
            Debug.Log("turn");
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
        //Keenan addition
        synch.SendSynchroniseDevices();
        //END
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

    //Modified by Keenan
    public void SelectAttackingCard(CardAttack card)
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

    public void Attack(CardAttack targetCard)
    {
        if (selectedAttackingCard != null && targetCard != null)
        {
            targetCard.Attack(selectedAttackingCard);

            selectedAttackingCard = null;
        }
    }
    //END

    public void AttackPlayerDirectly()
    {
        if (selectedAttackingCard != null)
        {
            opponentLife -= selectedAttackingCard.GetComponent<CardInfo>().attackValue;
            UpdateLifeUI();

            damageDealt += selectedAttackingCard.GetComponent<CardInfo>().attackValue;

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
