using UnityEngine;

public class CardGenerate : MonoBehaviour
{
    // Generates mana for land card.
    void Start()
    {
        
    }

    public void OnUpdateTurn()
    {
        //Generates mana
        GameManager.Instance.playerMana = Mathf.Min(GameManager.Instance.playerMana + 1, GameManager.Instance.maxMana);
    }
}
