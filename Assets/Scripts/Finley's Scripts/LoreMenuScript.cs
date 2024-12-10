using UnityEngine;

public class LoreMenuTrigger : MonoBehaviour
{
    public GameObject loreCanvas;  // The canvas with lore content
    public GameObject currentCanvas;  // The current canvas (main menu, for example)
    public GameObject cube;        // The cube that will move
    public Animator cubeAnimator;  // Animator for the cube's movement
    public GameObject optionsMenu;
    public GameObject cardMenu;
    public GameObject loreHolder;

    // Reference to the animation trigger
    public string cubeAnimationTrigger = "MoveCube";  // The trigger name in the Cube Animator

    // Reference for the second camera and its animator
    public GameObject loreCamera;      // The camera that will animate
    public Animator cameraAnimator; // The Animator for the camera
    public string cameraAnimationTrigger = "MoveCamera";  // The trigger for the camera's animation

    // Called when the lore button is pressed
    public void OpenLoreMenu()
    {
        // Hide the current canvas (e.g., main menu)
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(false);
        }

        // Start the cube animation
        cubeAnimator.SetTrigger(cubeAnimationTrigger);

        // Start the camera animation
        cameraAnimator.SetTrigger(cameraAnimationTrigger);

        // After the animation ends, show the lore screen
        StartCoroutine(ShowLoreScreenAfterAnimation());
    }

    // Coroutine to wait until the animations end before showing the lore screen
    private System.Collections.IEnumerator ShowLoreScreenAfterAnimation()
    {
        // Wait for the longest animation to finish (cube or camera)
        float cubeAnimationDuration = cubeAnimator.GetCurrentAnimatorStateInfo(0).length;
        float cameraAnimationDuration = cameraAnimator.GetCurrentAnimatorStateInfo(0).length;

        // Wait for both animations to finish (whichever takes longer)
        float waitTime = Mathf.Max(cubeAnimationDuration, cameraAnimationDuration);
        yield return new WaitForSeconds(waitTime);

        // Now that both animations have finished, show the lore canvas
        loreCanvas.SetActive(true);
    }

    // This method is called when the "Exit" button in the lore canvas is pressed
    public void ExitLoreMenu()
    {
        // Hide the lore canvas
        if (loreCanvas != null)
        {
            loreCanvas.SetActive(false);
        }

        // Show the current canvas again (the original screen, like main menu)
        if (currentCanvas != null)
        {
            currentCanvas.SetActive(true);
        }
    }

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

    public void OpenCardMenu()
    {
        loreHolder.SetActive(false); // Hide the options menu
        cardMenu.SetActive(true); //Hides the active canvas
    }

    public void CloseCardMenu()
    {
        loreHolder.SetActive(true); // Hide the options menu
        cardMenu.SetActive(false); //Hides the active canvas
    }
}
