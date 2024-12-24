using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class cardsappearing : MonoBehaviour
{
    public GameObject snapcard;
    public GameObject[] player1card;
    public GameObject[] player2card;
    public GameObject widget;
    public GameObject widget2;
    private bool pair = false;
    private GameObject tag1;
    private GameObject tag2;
    private bool winner = false;


    public void OnDealsnap ()
    {
        //widget.SetActive(false);

        print("hola");
        //snapcard.SetActive(true);
        int i = 0;
        while (i < player1card.Length)
        {
            player1card[i].SetActive(false);
            print(player1card[i].name);
            i++;
            //tag1 = player1card[i];


        }
        int cardnumber = Random.Range(0, 5);
        player1card[cardnumber].SetActive(true);
        tag1 = player1card[cardnumber];
        //print(cardnumber);

        i = 0;
        while (i < player2card.Length)
        {
            player2card[i].SetActive(false);
            i++;
            //tag2 = player2card[i];

        }
        cardnumber = Random.Range(0, 5);
        player2card[cardnumber].SetActive(true);
        tag2 = player2card[cardnumber];
        //print(cardnumber);

        if (tag1.CompareTag(tag2.tag))
        {
            pair = true;
            //widget.SetActive(true);
            print(tag1.tag);
            print(tag2.tag);
        }
        
        else
        {
            pair = false;
        }

    }

    private void Update()
    {
        if (pair == true)
        {
            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                widget.SetActive(true);
                winner = true;
            }
            else if (Input.GetKeyDown(KeyCode.RightShift))
            {
                widget2.SetActive(true);
                winner = true;
            }
        }

        //if (winner == true)
        //{
        //    if (Input.)
        //}
    }

    public void Reset()
    {
        SceneManager.LoadScene("Millie");
    }


}
