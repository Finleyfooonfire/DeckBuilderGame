using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RollingBanner : MonoBehaviour
{
    public ScrollRect scrollRect;       // The ScrollRect component controlling the scrolling
    public RectTransform content;       // The Content of the ScrollRect (the container of stories)
    public float scrollSpeed = 200f;    // How fast the banner scrolls
    public float resetPosition = -1000f;  // Position to reset content once it scrolls off-screen
    public float displayTime = 5f;      // Time to display each story
    public float smallerScale = 0.8f;   // Scale factor for the side stories
    public float storySpacing = 300f;   // Space between stories

    public GameObject storyButtonPrefab;   // The prefab for story buttons
    public Transform buttonParent;         // Where to place the story buttons

    // Example stories
    private string[] stories = {
        "The Rise of the Ancient Empire",
        "The Lost Temple of the Gods",
        "The Battle of Darkwater",
        "A Hero's Journey: Chronicles of the First Hero"
    };

    private int currentStoryIndex = 0;  // Index of the current story being displayed
    private GameObject currentStoryObj;  // The currently displayed story button
    private GameObject leftStoryObj;     // The story to the left
    private GameObject rightStoryObj;    // The story to the right

    void Start()
    {
        // Setup the banner's initial position
        content.anchoredPosition = new Vector2(0, 0);

        // Start the scrolling and story display process
        StartCoroutine(ScrollBanner());
    }

    // Coroutine that handles the scrolling of the content, showing one story at a time
    IEnumerator ScrollBanner()
    {
        while (true)
        {
            // Instantiate and display the current story, left story, and right story
            ShowCurrentStory();

            // Wait for the display time
            yield return new WaitForSeconds(displayTime);

            // Move the current story out of the screen (scroll left)
            StartCoroutine(ScrollStoryOutOfView());

            // Wait until the story has moved off-screen, then reset
            yield return new WaitForSeconds(scrollSpeed / 100f); // Adjust based on your scroll speed

            // Move to the next story
            currentStoryIndex = (currentStoryIndex + 1) % stories.Length;

            // Reset content position for the next story to start in view
            content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
        }
    }

    // Instantiate and display the current story, left story, and right story
    void ShowCurrentStory()
    {
        // Destroy previous stories if they exist
        if (currentStoryObj != null) Destroy(currentStoryObj);
        if (leftStoryObj != null) Destroy(leftStoryObj);
        if (rightStoryObj != null) Destroy(rightStoryObj);

        // Instantiate the current story button (main story)
        currentStoryObj = Instantiate(storyButtonPrefab, content);
        currentStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = stories[currentStoryIndex];
        currentStoryObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Center
        currentStoryObj.transform.localScale = Vector3.one; // Full size

        // Instantiate the left story button (side story on the left)
        leftStoryObj = Instantiate(storyButtonPrefab, content);
        leftStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = stories[(currentStoryIndex - 1 + stories.Length) % stories.Length]; // Previous story
        leftStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-storySpacing, 0); // Position to the left
        leftStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size

        // Instantiate the right story button (side story on the right)
        rightStoryObj = Instantiate(storyButtonPrefab, content);
        rightStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = stories[(currentStoryIndex + 1) % stories.Length]; // Next story
        rightStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing, 0); // Position to the right
        rightStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size
    }

    // Coroutine to handle scrolling the story out of the screen (to the left)
    IEnumerator ScrollStoryOutOfView()
    {
        // Start moving the content to the left (scroll it)
        float targetPosition = content.anchoredPosition.x - 1000f; // Move the content off the screen

        // Move the content gradually
        while (content.anchoredPosition.x > targetPosition)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x - scrollSpeed * Time.deltaTime, content.anchoredPosition.y);
            yield return null;
        }

        // After scrolling out of view, reset the content position for the next story
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
    }
}
