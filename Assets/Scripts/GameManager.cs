using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public PlayingFieldSynch synch { get; private set; }
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
        isPlayerTurn = isHost;
        UpdateTurn();
    }
    //End

    //void CoinFlip()
    //{
        //Keenan addition
        //Random.InitState((int)Time.time);
        //End
        //isPlayerTurn = Random.Range(0, 2) == 0;
        //UpdateTurn();
   // }

    void UpdateTurn()
    {
        Debug.Log(isPlayerTurn + "IMANNOYING");
        if (isPlayerTurn)
        {
            playerMana = Mathf.Min(playerMana + 1, maxMana);
            UpdateManaUI();

            Deck playerDeck = GameObject.Find("PlayerDeck").GetComponent<Deck>();
            playerDeck.DrawCard();

            statusText.text = "Your Turn";
            endTurnButton.interactable = true;
            Debug.Log("YOUR TURN");
        }
        else
        {
            statusText.text = "Opponent's Turn";
            Debug.Log("OPPONENTS TURN");
            endTurnButton.interactable = false;

            //StartCoroutine(AITurn());
        }
    }

    public void EndTurn()
    {
        isPlayerTurn = !isPlayerTurn;
        Debug.Log(isPlayerTurn);
        turnsTaken++;
        selectedAttackingCard = null;
        //UpdateTurn(); //No need to call this twice

        if (isPlayerTurn)
        {
            DrawCard();
        }
        else 
        {
            synch.SetHealthStatus(new HealthAndMana(playerMana, opponentMana, playerLife, opponentLife));
            synch.Send();//Keenan addition. Send the update to the other device
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
            selectedAttackingCard.Attack(targetCard);

            selectedAttackingCard = null;
            targetCard = null;
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
        if (playerWon)
        {
            synch.GameEnd();
        }
        PlayerPrefs.SetInt("PlayerWon", playerWon ? 1 : 0);
        PlayerPrefs.SetInt("TurnsTaken", turnsTaken);
        PlayerPrefs.SetInt("DamageDealt", damageDealt);

        //SceneManager.LoadScene("ScoreboardScene"); //Go to scoreboard
        SceneManager.LoadScene("MainMenu"); //Temporary
    }

    //KEENAN: Gets all the cards present.
    public Card[] GetAllCards()
    {
        return FindObjectsByType<Card>(FindObjectsSortMode.InstanceID);
    }
}
