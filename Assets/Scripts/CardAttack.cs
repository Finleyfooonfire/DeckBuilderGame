using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class CardAttack : MonoBehaviour, IPointerClickHandler
{
    bool canAttack;
    int exhaustionTimer;
    CardAttack targetCard;
    Vector3 restingPos;
    float animationProgress;
    bool animFinished;

    private void Start()
    {
        exhaustionTimer = 0;
        canAttack = false;
        GetComponent<CardInfo>().exhausted = false;
        restingPos = transform.position;
    }


    //Call this at the start of the turn.
    //Checks to see if the card is exhausted.
    public void OnUpdateTurn()
    {
        if (GetComponent<CardInfo>().isPlayerCard)
        {
            canAttack = true;
            GetComponent<CardInfo>().exhausted = exhaustionTimer > 0;
            exhaustionTimer--;
            canAttack |= !(GetComponent<CardInfo>().exhausted);//the card can't attack if exhausted
        }
    }

    void Update()
    {

        if (!animFinished) return;
        targetCard.GetComponent<CardInfo>().defenseValue -= GetComponent<CardInfo>().attackValue;
        bool killedInRetaliation = false;
        //Check to see if the target can retaliate
        if (!targetCard.GetComponent<CardInfo>().exhausted)
        {
            GetComponent<CardInfo>().defenseValue -= targetCard.GetComponent<CardInfo>().attackValue;
            killedInRetaliation = ((GetComponent<CardInfo>().defenseValue <= 0) && !GetComponent<CardInfo>().invincible);
        }

        if (targetCard.GetComponent<CardInfo>().defenseValue <= 0 && !targetCard.GetComponent<CardInfo>().invincible)
        {
            //Debug.Log($"{targetCard.GetComponent<CardInfo>().name} has been defeated by {GetComponent<CardInfo>().name}!");
            GameManager.Instance.synch.AddKilledCard(targetCard.gameObject);//The card has been damaged and therefore changed. Keenan addition.
        }
        else
        {
            //Debug.Log($"{targetCard.GetComponent<CardInfo>().name} now has {targetCard.GetComponent<CardInfo>().defenseValue} health remaining.");
            GameManager.Instance.synch.AddChangedCard(targetCard.gameObject);//Keenan addition
        }

        if (killedInRetaliation)
        {
            //Debug.Log($"{GetComponent<CardInfo>().name} has been defeated by {targetCard.GetComponent<CardInfo>().name}'s retaliation!");
            GameManager.Instance.synch.AddKilledFriendlyCard(gameObject);//The card has been defeated. Keenan addition.
        }
        else
        {
            //Debug.Log($"{GetComponent<CardInfo>().name} now has {GetComponent<CardInfo>().defenseValue} health remaining.");
            GameManager.Instance.synch.AddChangedCard(gameObject);//The card has been damaged and therefore changed. Keenan addition.
        }
        animFinished = false;
    }

    public void Attack(CardAttack targetCard)
    {
        if (canAttack)
        {
            this.targetCard = targetCard;
            StartCoroutine(AnimateAttack(targetCard.gameObject.transform.position));
            canAttack = false;
            exhaustionTimer = 1;
        }
    }

    //Selects the card when clicked on.
    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.isPlayerTurn)
        {
            //Select the card.
            //Debug.Log("The card \"" + gameObject.name + "\" has been clicked");
            //If they are the player's card, set as an attacking card. (And only allow to attack once)
            if (GetComponent<CardInfo>().isPlayerCard && canAttack)
            {
                if (FindAnyObjectByType<GameManager>().selectedAttackingCard != this && transform.parent == FindFirstObjectByType<CardPlayAreaGrid>().transform)
                {
                    FindAnyObjectByType<GameManager>().SelectAttackingCard(this);
                }
                else if (!(FindObjectsByType<CardInfo>(FindObjectsSortMode.None).Any(info => !info.isPlayerCard && info.transform.parent.gameObject.name.Equals("CardPlayArea"))))//Only attack the enemy directly if no enemy cards are found.
                {
                    FindAnyObjectByType<GameManager>().AttackPlayerDirectly();
                    canAttack = false;
                    exhaustionTimer = 1;
                }
            }
            //If not, set as the attacked card
            else
            {
                FindAnyObjectByType<GameManager>().Attack(this);
            }
        }

    }

    IEnumerator AnimateAttack(Vector3 targetPos)
    {
        animationProgress = 0;
        while (animationProgress < 1)
        {
            transform.position = Vector3.Lerp(restingPos, targetPos, animationProgress);
            animationProgress += .01f;
            yield return null;
        }
        while (animationProgress > 0)
        {
            transform.position = Vector3.Lerp(restingPos, targetPos, animationProgress);
            animationProgress -= .01f;
            yield return null;
        }
        animFinished = true;
    }
}