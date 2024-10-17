using UnityEngine;

[CreateAssetMenu]
public class CardKeenan : ScriptableObject
{
    public string faction;
    [Multiline] public string description;
    public int cardDamage;
    public int health;
    public int manaRequired;
    public string manaTypeRequired;
}
