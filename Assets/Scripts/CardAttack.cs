//Keenan Addition
using UnityEngine;

public class CardAttack : MonoBehaviour
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

    private void OnMouseDown()
    {
        //Select the card.
        Debug.Log("The card \"" + gameObject.name + "\" has been clicked");
        //FindAnyObjectByType<GameManager>().SelectCard(gameObject);
    }
}
//End