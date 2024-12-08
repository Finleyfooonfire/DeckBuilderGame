using UnityEngine;

public class PoisonDropSpell : CardSpell
{
    int life;

    private void Start()
    {
        life = GetComponent<CardInfo>().defenseValue;
    }

    public override void OnUpdateTurn()
    {
        base.OnUpdateTurn();
        //Do the spell update
        life--;
        if (life <= 0)
        {
            Destroy(gameObject);//Destroy the spell card once its time has ran out.
        }
    }

    public override void DoMagic()
    {
        //Poison drop: for two rounds take one hp.
        GetComponent<CardInfo>();
    }
}
