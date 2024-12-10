using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class cardsappearing : MonoBehaviour
{
    public GameObject snapcard;
    public GameObject[] player1card;
    public GameObject[] player2card;
    public GameObject widget;


    public void OnDealsnap ()
    {

        print("hola");
        //snapcard.SetActive(true);
        int i = 0;
        while (i < player1card.Length)
        {
            player1card[i].SetActive(false);
            i++;
            //gameObject.tag = player1card[i].tag;
        }
        int cardnumber = Random.Range(0, 5);
        player1card[cardnumber].SetActive(true);

        i = 0;
        while (i < player2card.Length)
        {
            player2card[i].SetActive(false);
            i++;
            //gameObject.tag = player2card[i].tag;
        }
        cardnumber = Random.Range(0, 5);
        player2card[cardnumber].SetActive(true);

        if (player1card[i].CompareTag(player2card[i].tag))
        {
            widget.SetActive(true);
        }

    }

    

}
