using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card/Stats", order = 1)]
public class CardStats : ScriptableObject
{
    [field: SerializeField] public Faction faction { get; private set; }
    [field: SerializeField][Multiline] public string description { get; private set; }
    [field: SerializeField] public int attackValue { get; private set; }
    [field: SerializeField] public int defenseValue { get; private set; }
    [field: SerializeField] public int manaCost { get; private set; }
    [field: SerializeField] public string manaTypeRequired { get; private set; }
    [field: SerializeField] public CardType cardType { get; private set; }
    [field: SerializeField] public Sprite cardImage { get; private set; }

    // Add a prefab field to specify the card's prefab in the editor
    [field: SerializeField] public GameObject cardPrefab { get; private set; }
}