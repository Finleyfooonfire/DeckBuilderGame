using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Keenan modifications
    public CardBehaviour player1Card;
    public CardBehaviour targetCard;  
    //End

    public void Player1Attack()
    {
        if (player1Card != null && targetCard != null)
        {
            Debug.Log($"{player1Card.name} is attacking {targetCard.name}!");
            player1Card.Attack(targetCard); // Player 1's card attacks the chosen target card
        }
        else
        {
            Debug.Log("Player 1 must select both an attacking card and a target card.");
        }
    }

    //Keenan modifications
    public void SelectCard(GameObject card)
    {
        if (card.TryGetComponent<CardBehaviour>(out CardBehaviour cardBehaviour)) 
        {
            switch (cardBehaviour.player)
            {
                case PlayerUser.player1:
                    player1Card = cardBehaviour;
                    break;
                case PlayerUser.player2:
                    targetCard = cardBehaviour;
                    break;
            }
        }
        else
        {
            Debug.Log("GameObject must have CardBehaviour component.");
        }
    } 
    //End
}
