using UnityEngine;

public class PoisonDropSpell : CardSpell
{
    //Inflicts poison

    public override void DoMagic()
    {
        //Poison drop: for two rounds take one hp.
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        if (card != null)
        {
            //Remove 1 health from the card.
            card.defenseValue--;
        }
        else
        {
            Debug.Log($"{GetComponent<CardInfo>().name} has no parent card. Moving to graveyard.");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//If there is no card attached, destroy self.
        }
    }
}
