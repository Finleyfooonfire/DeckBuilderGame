using UnityEngine;

[CreateAssetMenu]
public class Card : ScriptableObject
{
    public string faction;
    [Multiline] public string description;
    public int cardDamage;
    public int health;
    public int manaRequired;
    public string manaTypeRequired;

   // matt modifications 
    public void Attack(Card targetCard)
    {
        
        targetCard.health -= this.cardDamage;

       
        if (targetCard.health <= 0)
        {
            Debug.Log($"{targetCard.name} has been defeated by {this.name}!");
        }
        else
        {
            Debug.Log($"{targetCard.name} now has {targetCard.health} health remaining.");
        }
    }
    //end
}
