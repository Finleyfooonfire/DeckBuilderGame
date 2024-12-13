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

    private GameObject tag1;
    private GameObject tag2;


    public void OnDealsnap ()
    {
        widget.SetActive(false);

        print("hola");
        //snapcard.SetActive(true);
        int i = 0;
        while (i < player1card.Length - 1)
        {
            player1card[i].SetActive(false);
            i++;
            //tag1 = player1card[i];

        }
        int cardnumber = Random.Range(0, 5);
        player1card[cardnumber].SetActive(true);
        tag1 = player1card[cardnumber];
        print(cardnumber);

        i = 0;
        while (i < player2card.Length - 1)
        {
            player2card[i].SetActive(false);
            i++;
            //tag2 = player2card[i];

        }
        cardnumber = Random.Range(0, 5);
        player2card[cardnumber].SetActive(true);
        tag2 = player2card[cardnumber];
        print(cardnumber);

        if (tag1.CompareTag(tag2.tag))
        {
            widget.SetActive(true);
            print(tag1.tag);
            print(tag2.tag);
        }

    }

    

}
