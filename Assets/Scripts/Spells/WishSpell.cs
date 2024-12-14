using UnityEngine;

public class WishSpell : CardSpell
{
    //Prevent death for 1 turn

    protected override void OnDecommissionCard()
    {
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        if (card != null)
        {
            //Remove 1 health from the card.
            card.attackValue--;
        }
        base.OnDecommissionCard();
    }

    public override void DoMagic()
    {
        //Last wish of a dying star: for the rest of the turn
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        if (card != null)
        {
            //Remove 1 health from the card.
            if (card.defenseValue == 0)
            {
                card.defenseValue++;
                GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//Once used, destroy self.
            }
        }
        else
        {
            Debug.Log($"{GetComponent<CardInfo>().name} has no parent card. Moving to graveyard.");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//If there is no card attached, destroy self.
        }
    }
}
