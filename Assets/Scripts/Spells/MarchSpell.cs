using UnityEngine;

public class MarchSpell : CardSpell
{
    //Gives all allies 1 attack buff.

    protected override void OnDecommissionCard() 
    { 
        //Get all cards on the same team
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        Vector3[] cardPositions = cardGrid.GetSlotPositions(true, false);
        CardInfo[] cards = new CardInfo[5];
        for (int i = 0; i < cards.Length; i++)
        {
            cards[i] = cardGrid.FindCardAtSlotPosition(cardPositions[i]);
            if (cards[i] != null)
            {
                //Remove 1 attack from the card.
                cards[i].attackValue--;
                //Debug.Log("March card more attack has ended");
            }
        }
        base.OnDecommissionCard();
    }

    public override void DoMagic()
    {
        //March Of Judgement: for the rest of the turn
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        if (card == null)
        {
            //Debug.Log($"{GetComponent<CardInfo>().name} has no parent card. Moving to graveyard.");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//If there is no card attached, destroy self.
        }

        //Buff the cards
        Vector3[] cardPositions = cardGrid.GetSlotPositions(true, false);
        CardInfo[] cards = new CardInfo[5];
        for (int i = 0; i < cards.Length; i++)
        {
            var temp = cardPositions[i];
            temp.y = .1f;
            cards[i] = cardGrid.FindCardAtSlotPosition(temp);
            if (cards[i] != null)
            {
                //Remove 1 attack from the card.
                cards[i].attackValue++;
                //Debug.Log("March card more attack has started");
            }
        }
        life--;
    }
}
