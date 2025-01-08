using UnityEngine;

public class OptionsClick : MonoBehaviour
{
    public GameObject optionsMenu;
    public GameObject currentCanvas;


    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);  // Show the options menu
        currentCanvas.SetActive(false); //Hides the active canvas
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false); // Hide the options menu
        currentCanvas.SetActive(true); //Hides the active canvas
    }
}
