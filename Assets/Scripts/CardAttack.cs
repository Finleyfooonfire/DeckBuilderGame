using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttack : MonoBehaviour, IPointerClickHandler
{
    bool canAttack;
    int exhaustionTimer;

    private void Start()
    {
        exhaustionTimer = 0;
        canAttack = true;
        GetComponent<CardInfo>().exhausted = false;
    }

    //Call this at the start of the turn.
    //Checks to see if the card is exhausted.
    public void OnUpdateTurn()
    {
        if (GetComponent<CardInfo>().isPlayerCard) 
        { 
            canAttack = true;
            GetComponent<CardInfo>().exhausted = exhaustionTimer > 0;
            exhaustionTimer--;
            canAttack |= !(GetComponent<CardInfo>().exhausted);//the card can't attack if exhausted
        }
    }

    public void Attack(CardAttack targetCard)
    {
        if (canAttack)
        {
            targetCard.GetComponent<CardInfo>().defenseValue -= GetComponent<CardInfo>().attackValue;

            //Check to see if the target can retaliate
            if (!targetCard.GetComponent<CardInfo>().exhausted) 
            {
                GetComponent<CardInfo>().defenseValue -= targetCard.GetComponent<CardInfo>().attackValue;
            }

            if (targetCard.GetComponent<CardInfo>().defenseValue <= 0)
            {
                Debug.Log($"{targetCard.GetComponent<CardInfo>().name} has been defeated by {GetComponent<CardInfo>().name}!");
                Destroy(targetCard.gameObject);//The target card has reached 0 health so it shall die.
            }
            else
            {
                Debug.Log($"{targetCard.GetComponent<CardInfo>().name} now has {targetCard.GetComponent<CardInfo>().defenseValue} health remaining.");
            }
            exhaustionTimer = 1;
        }
    }

    //Selects the card when clicked on.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlayerTurn)
        {
            //Select the card.
            Debug.Log("The card \"" + gameObject.name + "\" has been clicked");
            //If they are the player's card, set as an attacking card.
            if (GetComponent<CardInfo>().isPlayerCard)
            {
                FindAnyObjectByType<GameManager>().SelectAttackingCard(this);
            }
            //If not, set as the attacked card
            else
            {
                FindAnyObjectByType<GameManager>().Attack(this);
            }
        }

    }
}