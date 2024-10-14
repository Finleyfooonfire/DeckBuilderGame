using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] CardStats cardStats;

    // matt modifications 
    public void Attack(CardBehaviour targetCard)
    {

        targetCard.cardStats.health -= cardStats.cardDamage;


        if (targetCard.cardStats.health <= 0)
        {
            Debug.Log($"{targetCard.cardStats.name} has been defeated by {cardStats.name}!");
        }
        else
        {
            Debug.Log($"{targetCard.cardStats.name} now has {targetCard.cardStats.health} health remaining.");
        }
    }
    //end
}
