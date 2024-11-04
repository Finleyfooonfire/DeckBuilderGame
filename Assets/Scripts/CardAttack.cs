//Keenan Addition
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;

public class CardAttack : MonoBehaviour, IPointerClickHandler
{
    public void Attack(CardInfo targetCard)
    {
        targetCard.defenseValue -= GetComponent<CardInfo>().attackValue;


        if (targetCard.defenseValue <= 0)
        {
            Debug.Log($"{targetCard.gameObject.name} has been defeated by {gameObject.name}!");
        }
        else
        {
            Debug.Log($"{targetCard.gameObject.name} now has {targetCard.defenseValue} health remaining.");
        }
    }


    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlayerTurn)
        {
            //Select the card.
            Debug.Log("The card \"" + gameObject.name + "\" has been clicked");
            //FindAnyObjectByType<GameManager>().SelectCard(gameObject);
        }

    }
}
//End