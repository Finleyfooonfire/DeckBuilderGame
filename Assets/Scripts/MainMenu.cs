using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        SceneManager.LoadScene("LoadingScreen");
    }
    //Keenan addition.
    public void OpenLore()
    {
        SceneManager.LoadScene("LoreScreen");
    }

    public void OpenOptions()
    {
        SceneManager.LoadScene("Settings");
    }

    public void OpenSnap()
    {
        SceneManager.LoadScene("Millie");
    }
    //End

    public void ExitGame()
    {
        Application.Quit();
    }
}
