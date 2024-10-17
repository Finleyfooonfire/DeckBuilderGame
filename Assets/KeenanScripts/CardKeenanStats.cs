using UnityEngine;

[CreateAssetMenu]
public class CardKeenanStats : ScriptableObject
{
    [field: SerializeField] public string faction { get; private set; }
    [field: SerializeField][Multiline] public string description { get; private set; }
    [field: SerializeField] public int cardDamage { get; private set; }
    [field: SerializeField] public int maxHealth { get; private set; }
    [field: SerializeField] public int manaRequired { get; private set; }
    [field: SerializeField] public string manaTypeRequired { get; private set; }


}
