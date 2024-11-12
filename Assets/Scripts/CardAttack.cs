using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardAttack : MonoBehaviour, IPointerClickHandler
{
    public void Attack(CardAttack targetCard)
    {
        targetCard.GetComponent<CardInfo>().defenseValue -= GetComponent<CardInfo>().attackValue;


        if (targetCard.GetComponent<CardInfo>().defenseValue <= 0)
        {
            Debug.Log($"{targetCard.GetComponent<CardInfo>().name} has been defeated by {GetComponent<CardInfo>().name}!");
            Destroy(targetCard.gameObject);//The target card has reached 0 health so it shall die.
        }
        else
        {
            Debug.Log($"{targetCard.GetComponent<CardInfo>().name} now has {targetCard.GetComponent<CardInfo>().defenseValue} health remaining.");
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
                if (FindAnyObjectByType<GameManager>().selectedAttackingCard != this)
                {
                    FindAnyObjectByType<GameManager>().SelectAttackingCard(this);
                }
                else
                {
                    FindAnyObjectByType<GameManager>().AttackPlayerDirectly();
                }
            }
            //If not, set as the attacked card
            else
            {
                FindAnyObjectByType<GameManager>().Attack(this);
            }
        }

    }
}