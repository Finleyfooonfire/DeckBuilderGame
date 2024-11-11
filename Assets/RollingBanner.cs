using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RollingBanner : MonoBehaviour
{
    public ScrollRect scrollRect;        // The ScrollRect component controlling the scrolling
    public RectTransform content;        // The Content of the ScrollRect (the container of stories)
    public float scrollSpeed = 200f;     // How fast the banner scrolls
    public float displayTime = 5f;       // Time to display each story
    public float smallerScale = 0.8f;    // Scale factor for the side stories
    public float storySpacing = 300f;    // Space between stories

    public Transform storyParent;        // The parent object that holds all the story prefabs

    private GameObject[] storyPrefabs;   // Array to store the actual story prefabs
    private string[] storyTitles;        // Array to store the titles of the stories

    private int currentStoryIndex = 0;   // Index of the current main story
    private GameObject leftStoryObj;     // The story to the left
    private GameObject mainStoryObj;     // The currently displayed main story canvas panel
    private GameObject rightStoryObj;    // The story to the right

    void Start()
    {
        // Dynamically populate the storyPrefabs and storyTitles arrays
        PopulateStoryData();

        // Setup the banner's initial position
        content.anchoredPosition = new Vector2(0, 0);

        // Start the banner loop (story scrolling)
        StartCoroutine(ScrollBanner());
    }

    // Method to populate storyPrefabs and storyTitles arrays from the prefabs in the scene
    void PopulateStoryData()
    {
        // Get all the children of the storyParent
        int prefabCount = storyParent.childCount;
        storyPrefabs = new GameObject[prefabCount];
        storyTitles = new string[prefabCount];

        // Populate the arrays with the prefabs and their titles
        for (int i = 0; i < prefabCount; i++)
        {
            // Store each prefab in the storyPrefabs array
            storyPrefabs[i] = storyParent.GetChild(i).gameObject;

            // Assuming each prefab has a TextMeshProUGUI component for the title, store that in the storyTitles array
            TextMeshProUGUI titleText = storyPrefabs[i].GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
            {
                storyTitles[i] = titleText.text;
            }
            else
            {
                Debug.LogWarning("Prefab at index " + i + " does not contain a TextMeshProUGUI component for the title.");
            }
        }
    }

    // Coroutine that handles the scrolling of the content, showing one story at a time
    IEnumerator ScrollBanner()
    {
        while (true)
        {
            // Show the current stories (left, main, right)
            ShowCurrentStories();

            // Wait for the display time (let the user read the current story)
            yield return new WaitForSeconds(displayTime);

            // Scroll the current stories to the left
            yield return StartCoroutine(ScrollStoriesLeft());

            // Move to the next story
            currentStoryIndex = (currentStoryIndex + 1) % storyPrefabs.Length; // Loop back after the last story
        }
    }

    // Instantiate and display the current stories: left, main, right
    void ShowCurrentStories()
    {
        // Destroy previous stories if they exist
        if (mainStoryObj != null) Destroy(mainStoryObj);
        if (leftStoryObj != null) Destroy(leftStoryObj);
        if (rightStoryObj != null) Destroy(rightStoryObj);

        // Instantiate the current main story panel (from the prefab array)
        mainStoryObj = Instantiate(storyPrefabs[currentStoryIndex], content);
        mainStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[currentStoryIndex]; // Set the story name or title
        mainStoryObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero; // Center
        mainStoryObj.SetActive(true); // Ensure the prefab is set to active
        mainStoryObj.transform.localScale = Vector3.one; // Full size

        // Instantiate the left story panel (side story on the left) - previous story
        leftStoryObj = Instantiate(storyPrefabs[(currentStoryIndex - 1 + storyPrefabs.Length) % storyPrefabs.Length], content);
        leftStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex - 1 + storyPrefabs.Length) % storyPrefabs.Length]; // Previous story
        leftStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-storySpacing, 0); // Position to the left
        leftStoryObj.SetActive(true); // Ensure the prefab is set to active
        leftStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size

        // Instantiate the right story panel (side story on the right) - next story
        rightStoryObj = Instantiate(storyPrefabs[(currentStoryIndex + 1) % storyPrefabs.Length], content);
        rightStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex + 1) % storyPrefabs.Length]; // Next story
        rightStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing, 0); // Position to the right
        rightStoryObj.SetActive(true); // Ensure the prefab is set to active
        rightStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size
    }

    // Coroutine to handle scrolling the stories to the left
    IEnumerator ScrollStoriesLeft()
    {
        // Move the content to the left (scroll it)
        float targetPosition = content.anchoredPosition.x - storySpacing; // Move the content off the screen by the width of one story

        // Scroll the content gradually to the left
        while (content.anchoredPosition.x > targetPosition)
        {
            content.anchoredPosition = new Vector2(content.anchoredPosition.x - scrollSpeed * Time.deltaTime, content.anchoredPosition.y);
            yield return null;
        }

        // After scrolling out of view, reset the content position for the next set of stories
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y); // Reset to the center

        // Reset the positions of the panels so that the next panel can show up correctly
        ResetPanelPositions();
    }

    // Reset the panel positions for the next scroll
    void ResetPanelPositions()
    {
        // Move the left panel to the far left
        leftStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-storySpacing, 0);
        // Move the right panel to the far right
        rightStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing, 0);

        // Scale down the left and right panels for smaller size
        leftStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1);
        rightStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1);
    }
}
