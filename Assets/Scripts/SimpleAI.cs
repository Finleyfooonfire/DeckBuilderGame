using UnityEngine;

public class SimpleAI : MonoBehaviour
{
    public Transform cardPlayArea;
    private bool hasPlayedThisTurn = false;

    void Start()
    {
        if (cardPlayArea == null)
        {
            cardPlayArea = GameObject.Find("CardPlayArea").transform;
        }
    }

    void Update()
    {
        if (!GameManager.Instance.isPlayerTurn && !hasPlayedThisTurn)
        {
            RevealOneCard();
            hasPlayedThisTurn = true;
            Invoke("EndAITurn", 0.5f);
        }
    }

    void RevealOneCard()
    {
        foreach (Transform child in cardPlayArea)
        {
            if (!child.gameObject.activeSelf)
            {
                child.gameObject.SetActive(true);
                break;
            }
        }
    }

    void EndAITurn()
    {
        hasPlayedThisTurn = false;
        GameManager.Instance.EndTurn();
    }
}