using UnityEngine;

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] CardStats cardStats;
    enum Player { player1, player2 };
    [Tooltip("Assign the player who owns the card")] [SerializeField] Player player;

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

    private void OnMouseDown()
    {
        //Select the card.
        Debug.Log("The card \"" + cardStats.name + "\" owned by " + player.ToString() +" has been clicked");
    }
}
