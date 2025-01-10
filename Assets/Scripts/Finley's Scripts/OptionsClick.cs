using UnityEngine;

public class OptionsClick : MonoBehaviour
{
    public GameObject optionmenu;
    public GameObject maincanvas;


    public void OpenOptionsMenu()
    {
        optionmenu.SetActive(true);
        maincanvas.SetActive(false); 
    }

    public void CloseOptionsMenu()
    {
        optionmenu.SetActive(false);
        maincanvas.SetActive(true); 
    }
}
