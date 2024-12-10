using UnityEngine;

public class MarchSpell : CardSpell
{
    int life;
    CardPlayAreaGrid cardGrid;

    private void Start()
    {
        life = GetComponent<CardInfo>().defenseValue;
        Transform cardPlayArea = GameObject.Find("CardPlayArea").transform;
        cardGrid = cardPlayArea.gameObject.GetComponent<CardPlayAreaGrid>(); ;
    }

    public override void OnUpdateTurn()
    {
        base.OnUpdateTurn();
        //Do the spell update
        DoMagic();
        life--;
        if (life == 0)//Using a life of 0 make it invincible.
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
                Debug.Log($"{GetComponent<CardInfo>().name}'s usefulness has ran out. Moving card to graveyard.");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//Destroy the spell card once its time has ran out.
        }
    }

    public override void DoMagic()
    {
        //March Of Judgement: for the rest of the turn
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        if (card != null)
        {
            //Remove 1 health from the card.
            card.attackValue++;
        }
        else
        {
            Debug.Log($"{GetComponent<CardInfo>().name} has no parent card. Moving to graveyard.");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//If there is no card attached, destroy self.
        }
    }
}
