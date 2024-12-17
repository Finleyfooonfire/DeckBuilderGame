using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public bool isPlayerCard;//Keenan addition
    public int manaCost;
    public int attackValue;
    public int defenseValue;
    public Faction faction;
    public CardType cardType;
    public Sprite cardImage;
    public bool exhausted;//Keenan addition
    public bool invincible;//Keenan addition
    public CardSpell spell;//Keenan addition
}