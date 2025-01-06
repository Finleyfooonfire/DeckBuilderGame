using TMPro;
using UnityEngine;

public class CardInfo : MonoBehaviour
{
    public bool isPlayerCard;//Keenan addition
    public int manaCost;
    public string manaColour;
    public int attackValue;
    public int defenseValue;
    public Faction faction;
    public CardType cardType;
    public Sprite cardImage;
    public bool exhausted;//Keenan addition
    public bool invincible;//Keenan addition
    public CardSpell spell;//Keenan addition

    TMP_Text damage; //Keenan addition
    TMP_Text health; //Keenan addition

    private void Start() //Keenan addition
    {
        damage = transform.Find("Damage").GetComponent<TMP_Text>();
        health = transform.Find("Health").GetComponent<TMP_Text>();
        UpdateStats();
    }

    public void UpdateStats() //Keenan addition
    {
        damage.text = attackValue.ToString();
        health.text = defenseValue.ToString();
    }
}