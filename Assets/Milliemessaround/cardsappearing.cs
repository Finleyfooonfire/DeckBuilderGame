using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class cardsappearing : MonoBehaviour
{
    public GameObject snapcard;
    public GameObject[] player1card;
    public GameObject[] player2card;

    public void OnDealsnap ()
    {
        print("hola");
        //snapcard.SetActive(true);
        int i = 0;
        while (i < player1card.Length)
        {
            player1card[i].SetActive(false);
            i++;
        }
        int cardnumber = Random.Range(0, 5);
        player1card[cardnumber].SetActive(true);

        i = 0;
        while (i < player2card.Length)
        {
            player2card[i].SetActive(false);
            i++;
        }
        cardnumber = Random.Range(0, 5);
        player2card[cardnumber].SetActive(true);
    }



}
