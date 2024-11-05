using UnityEngine;

[CreateAssetMenu]
public class CardStats : ScriptableObject
{
    [field: SerializeField] public Faction faction { get; private set; }
    [field: SerializeField][Multiline] public string description { get; private set; }
    [field: SerializeField] public int attackValue { get; private set; }
    [field: SerializeField] public int defenseValue { get; private set; }
    [field: SerializeField] public int manaCost { get; private set; }
    [field: SerializeField] public string manaTypeRequired { get; private set; }
    [field: SerializeField] public CardType cardType { get; private set; }

}
