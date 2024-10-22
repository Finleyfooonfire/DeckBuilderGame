using UnityEngine;

public class CardAbilities : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

   bool cardplaced = false;
    public GameManager gamemanager;

   string cardname = "";
    string cardfaction = "";
    public Card cardcode;
    int attack = 0;
    string ontable = "";

    bool alive = false;
    void Start()
    {

        if (cardplaced == true)
        {
            cardname = cardcode.cardName;
            cardfaction = cardcode.cardFaction;
            attack = cardcode.attackValue;
            ontable = gamemanager.OnTable; 
            string modifiedTable = AddBonusDamage(ontable, "Elf", "+2"); // THIS NEEDS TO LATER BE CHANGED ONCE ONTABLE FUNCTION IS COMPLETE TO ACTUALLY INCREASE DAMAGE RATHER THAN EXAMPLE STRING

            if (cardname == "Elven Chief")
            {
                if (alive == true)
                {

                }
            }
        }
        string AddBonusDamage(string source, string target, string bonus)
        {
            string modified = source.Replace(target, target + bonus);
            return modified;
        }
    }

    // Update is called once per frame
    void Update()
    {
       
     
            
    }
}
