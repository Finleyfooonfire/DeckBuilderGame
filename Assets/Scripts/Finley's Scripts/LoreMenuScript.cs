using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;

public class LoreMenuTrigger : MonoBehaviour
{
    public GameObject loreCanvas; 
    public GameObject currentCanvas;  
    public GameObject cube;       
    public Animator cubeAnimator;  
    public GameObject optionsMenu;
    public GameObject cardMenu;
    public GameObject loreHolder;
    public GameObject creditsMenu;
    public Volume volume;


    [SerializeField] public Camera targetCamera;

    public void Start()
    {
        volume = targetCamera.GetComponent<Volume>();
        if (volume == null)
        {
          //  Debug.LogError("No Volume component found on the target camera!");
        }

    }
    public string cubeAnimationTrigger = "MoveCube"; 
    public GameObject loreCamera;     
    public Animator cameraAnimator;
    public string cameraAnimationTrigger = "MoveCamera";

    public void OpenLoreMenu()
    {
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }
        cubeAnimator.SetTrigger(cubeAnimationTrigger);
        cameraAnimator.SetTrigger(cameraAnimationTrigger);
        StartCoroutine(ShowLoreScreenAfterAnimation());
    }

    private System.Collections.IEnumerator ShowLoreScreenAfterAnimation()
    {
        float cubeAnimationDuration = cubeAnimator.GetCurrentAnimatorStateInfo(0).length;
        float cameraAnimationDuration = cameraAnimator.GetCurrentAnimatorStateInfo(0).length;
        float waitTime = Mathf.Max(cubeAnimationDuration, cameraAnimationDuration);
        yield return new WaitForSeconds(waitTime);
        loreCanvas.SetActive(true);
    }

    public void ExitLoreMenu()
    {
  
        if (loreCanvas != null)
        {
            loreCanvas.SetActive(false);
        }
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(true);
        }
    }

    public void OpenOptionsMenu()
    {
        optionsMenu.SetActive(true);  
        currentCanvas.SetActive(false); 
    }

    public void CloseOptionsMenu()
    {
        optionsMenu.SetActive(false);
        currentCanvas.SetActive(true); 
    }

    public void OpenCardMenu()
    {
        loreHolder.SetActive(false);
        cardMenu.SetActive(true); 
    }

    public void CloseCardMenu()
    {
        loreHolder.SetActive(true); 
        cardMenu.SetActive(false); 
    }

    public void OpenCreditsMenu()
    {
        currentCanvas.SetActive(false); 
        creditsMenu.SetActive(true); 
    }

    public void CloseCreditsMenu()
    {
        currentCanvas.SetActive(true); 
        creditsMenu.SetActive(false); 
    }

    public void ToggleVolume()
    {
        if (volume != null)
        {
            volume.enabled = !volume.enabled;
        }
    }
}
