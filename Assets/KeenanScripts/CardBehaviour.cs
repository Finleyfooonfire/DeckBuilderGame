using UnityEngine;

public enum PlayerUser { player1, player2 };

public class CardBehaviour : MonoBehaviour
{
    [SerializeField] CardStats cardStats;
    //Keenan modification: Created a health variable. The health variable in the CardStats ScriptableObject is for max health.
    int health;
    //end

    private void Start()
    {
        health = cardStats.defenseValue;
    }

    [Tooltip("Assign the player who owns the card")] public PlayerUser player;

    // matt modifications 
    public void Attack(CardBehaviour targetCard)
    {
        targetCard.health -= cardStats.attackValue;


        if (targetCard.health <= 0)
        {
            Debug.Log($"{targetCard.cardStats.name} has been defeated by {cardStats.name}!");
        }
        else
        {
            Debug.Log($"{targetCard.cardStats.name} now has {targetCard.health} health remaining.");
        }
    }
    //end

    private void OnMouseDown()
    {
        //Select the card.
        Debug.Log("The card \"" + cardStats.name + "\" owned by " + player.ToString() +" has been clicked");
        //FindAnyObjectByType<GameManager>().SelectCard(gameObject);
    }
}
