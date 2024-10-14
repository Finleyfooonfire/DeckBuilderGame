using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] Card cardStats;

    // matt modifications 
    public void Attack(Card targetCard)
    {

        targetCard.health -= cardStats.cardDamage;


        if (targetCard.health <= 0)
        {
            Debug.Log($"{targetCard.name} has been defeated by {cardStats.name}!");
        }
        else
        {
            Debug.Log($"{targetCard.name} now has {targetCard.health} health remaining.");
        }
    }
    //end
}
