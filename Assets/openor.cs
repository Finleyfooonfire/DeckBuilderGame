using UnityEngine;

public class openor : MonoBehaviour
{


    public GameObject optionsMenu;
    public GameObject optionsMenu1;
    public GameObject optionsMenu2;
    public GameObject optionsMenu3;
    public GameObject optionsMenu4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }


    

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);
    }

    public void  CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
    }

    public void OpenOptionsMenu2()
    {
        optionsMenu2.SetActive(true);
    }

    public void CloseOptionsMenu2()
    {
        optionsMenu2.SetActive(false);
    }

    public void OpenOptionsMenu1()
    {
        optionsMenu1.SetActive(true);
    }

    public void CloseOptionsMenu1()
    {
        optionsMenu1.SetActive(false);
    }

    public void OpenOptionsMenu3()
    {
        optionsMenu3.SetActive(true);
    }

    public void CloseOptionsMenu3()
    {
        optionsMenu3.SetActive(false);
    }
}

