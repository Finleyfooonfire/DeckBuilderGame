using System.Collections;
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
    private static bool isGameEnding = false;

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

    public TextMeshProUGUI gameOverText;

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
        playerMana = 5;
        opponentMana = 5;

        UpdateLifeUI();
        UpdateManaUI();
        endTurnButton.onClick.AddListener(EndTurn);

        if (gameOverText != null)
        {
            gameOverText.gameObject.SetActive(false);
        }
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
        //Debug.Log(isPlayerTurn + "IMANNOYING");
        if (isPlayerTurn)
        {
            playerMana = Mathf.Min(playerMana, maxMana);
            UpdateManaUI();
            UpdateLifeUI();

            Deck playerDeck = GameObject.Find("PlayerDeck").GetComponent<Deck>();
            playerDeck.DrawCard();

            statusText.text = "Your Turn";
            endTurnButton.interactable = true;
            //Debug.Log("YOUR TURN");
        }
        else
        {
            statusText.text = "Opponent's Turn";
            //Debug.Log("OPPONENTS TURN");
            endTurnButton.interactable = false;

            //StartCoroutine(AITurn());
        }

        CheckLifeTotals();
    }

    public void EndTurn()
    {
        if (opponentLife <= 0)
        {
            GameOver(true);
        }

        if (isPlayerTurn) UpdateCards();
        isPlayerTurn = !isPlayerTurn;
        //Debug.Log(isPlayerTurn);
        turnsTaken++;
        selectedAttackingCard = null;
        //UpdateTurn(); //No need to call this twice
        synch.SetHealthStatus(new HealthAndMana(playerMana, opponentMana, playerLife, opponentLife));
        synch.SendHealthAndMana();
        if (isPlayerTurn)
        {
            DrawCard();
        }
        else 
        {
            synch.SendCardChange();//Keenan addition. Send the update to the other device
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
            //Debug.LogError("PlayerDeck not found!");
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

    void UpdateCards()
    {
        CardSpell spell = null;
        CardGenerate land = null;
        foreach (var cardInfo in FindObjectsByType<CardInfo>(FindObjectsSortMode.None))
        {
            if (cardInfo.gameObject.TryGetComponent<CardSpell>(out spell))
            {
                spell.OnUpdateTurn();
            }
            else if (cardInfo.gameObject.TryGetComponent<CardGenerate>(out land))
            {
                land.OnUpdateTurn();
            }
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

    //      //Debug.Log("AI forfeits its turn.");

    //     EndTurn();
    //  }

    public void GameOver(bool playerWon)
    {
        synch.SetHealthStatus(new HealthAndMana(playerMana, opponentMana, playerLife, opponentLife));
        synch.SendHealthAndMana();
        if (gameOverText != null)
        {
            if (GameManager.Instance.opponentLife <= 0)
            {
                gameOverText.text = "You Win!";
                if (playerWon)
                {
                    GameManager.Instance.synch.GameEnd();
                }
            }
            else
            {
                gameOverText.text = "You Lose!";
            }
            gameOverText.gameObject.SetActive(true);
        }

        if (GameManager.Instance.endTurnButton != null)
        {
            GameManager.Instance.endTurnButton.interactable = false;
        }

        StartCoroutine(DelayedSceneTransition());
    }

    private IEnumerator DelayedSceneTransition()
    {
        yield return new WaitForSeconds(5f);
        SceneManager.LoadScene("MainMenu");
    }

    public void OnReceiveGameEnd()
    {
        if (!isGameEnding)
        {
            isGameEnding = true;
            if (gameOverText != null)
            {
                gameOverText.text = "You Lose!";
                gameOverText.gameObject.SetActive(true);
            }

            if (endTurnButton != null)
            {
                endTurnButton.interactable = false;
            }

            StartCoroutine(DelayedSceneTransition());
        }
    }

    //KEENAN: Gets all the cards present.
    public Card[] GetAllCards()
    {
        return FindObjectsByType<Card>(FindObjectsSortMode.InstanceID);
    }

    private void CheckLifeTotals()
    {
        if (playerLife <= 0 || opponentLife <= 0)
        {
            if (endTurnButton != null)
            {
                endTurnButton.interactable = false;
            }
            GameOver(opponentLife <= 0);
        }
    }



}
