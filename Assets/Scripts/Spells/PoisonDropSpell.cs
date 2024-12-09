using UnityEngine;

public class PoisonDropSpell : CardSpell
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
        if (life == 0)
        {
            Destroy(gameObject);//Destroy the spell card once its time has ran out.
        }
    }

    public override void DoMagic()
    {
        //Poison drop: for two rounds take one hp.
        //Get the card the spell is attached to.
        Vector3 findCardPosition = transform.position;
        findCardPosition.y = 0.1f;
        CardInfo card = cardGrid.FindCardAtSlotPosition(findCardPosition);
        //Remove 1 health from the card.

    }
}
