using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RollingBanner : MonoBehaviour
{
    public ScrollRect scrollRect;        // The ScrollRect component controlling the scrolling
    public RectTransform content;        // The Content of the ScrollRect (the container of stories)
    public float scrollSpeed = 2f;       // Speed at which the banner scrolls (lower value = slower)
    public float displayTime = 5f;       // Time to display each story
    public float smallerScale = 0.8f;    // Scale factor for the side stories
    public float storySpacing = 300f;    // Space between stories

    public Transform storyParent;        // The parent object that holds all the story prefabs

    public float scaleTransitionDuration = 1f; // Customizable duration for scaling transition

    private GameObject[] storyPrefabs;   // Array to store the actual story prefabs
    private string[] storyTitles;        // Array to store the titles of the stories

    private int currentStoryIndex = 0;   // Index of the current main story
    private GameObject leftStoryObj;     // The story to the left
    private GameObject mainStoryObj;     // The currently displayed main story canvas panel
    private GameObject rightStoryObj;    // The story to the right

    private float currentScrollTime = 0f; // Track current scroll time for smooth animation

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

            // Scroll the current stories to the left with smooth animation
            yield return StartCoroutine(SmoothScrollStoriesLeft());

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

    // Coroutine to handle smooth scrolling of the stories to the left
    IEnumerator SmoothScrollStoriesLeft()
    {
        float startPosition = content.anchoredPosition.x;
        float targetPosition = startPosition - storySpacing;

        float journeyLength = Mathf.Abs(targetPosition - startPosition);
        float journeyStartTime = Time.time;

        // Smoothly animate the content position
        while (Mathf.Abs(content.anchoredPosition.x - targetPosition) > 0.1f)
        {
            float journeyProgress = (Time.time - journeyStartTime) * scrollSpeed / journeyLength;
            float newPosition = Mathf.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0f, 1f, journeyProgress));

            content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

            yield return null;
        }

        // Ensure content ends exactly at the target position
        content.anchoredPosition = new Vector2(targetPosition, content.anchoredPosition.y);

        // Reset the content position for the next set of stories
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y); // Reset to the center

        // Reset the positions and scaling of the panels so that the next panel can show up correctly
        ResetPanelPositions();
    }

    // Reset the panel positions for the next scroll and scale them smoothly
    void ResetPanelPositions()
    {
        // Smoothly transition the left and right panels to their positions and scales
        StartCoroutine(SmoothPanelTransition());
    }

    // Smoothly transition the left and right panels to their positions and scales
    IEnumerator SmoothPanelTransition()
    {
        // Transition the left panel to its target position and scale
        Vector3 leftStartPos = leftStoryObj.transform.localPosition;
        Vector3 leftTargetPos = new Vector3(-storySpacing, 0, 0);
        Vector3 leftStartScale = new Vector3(smallerScale, smallerScale, 1);
        Vector3 leftTargetScale = Vector3.one;

        float leftJourneyStartTime = Time.time;
        float leftJourneyLength = Vector3.Distance(leftStartPos, leftTargetPos);

        // Transition the left panel position and scale
        while (Vector3.Distance(leftStoryObj.transform.localPosition, leftTargetPos) > 0.1f)
        {
            float journeyProgress = (Time.time - leftJourneyStartTime) * scrollSpeed / leftJourneyLength;
            leftStoryObj.transform.localPosition = Vector3.Lerp(leftStartPos, leftTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            leftStoryObj.transform.localScale = Vector3.Lerp(leftStartScale, leftTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }

        // Ensure the left panel ends at the exact target position and scale
        leftStoryObj.transform.localPosition = leftTargetPos;
        leftStoryObj.transform.localScale = leftTargetScale;

        // Transition the right panel to its target position and scale
        Vector3 rightStartPos = rightStoryObj.transform.localPosition;
        Vector3 rightTargetPos = new Vector3(storySpacing, 0, 0);
        Vector3 rightStartScale = new Vector3(smallerScale, smallerScale, 1);
        Vector3 rightTargetScale = Vector3.one;

        float rightJourneyStartTime = Time.time;
        float rightJourneyLength = Vector3.Distance(rightStartPos, rightTargetPos);

        // Transition the right panel position
        while (Vector3.Distance(rightStoryObj.transform.localPosition, rightTargetPos) > 0.1f)
        {
            float journeyProgress = (Time.time - rightJourneyStartTime) * scrollSpeed / rightJourneyLength;
            rightStoryObj.transform.localPosition = Vector3.Lerp(rightStartPos, rightTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localScale = Vector3.Lerp(rightStartScale, rightTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }

        // Ensure the right panel ends at the exact target position and scale
        rightStoryObj.transform.localPosition = rightTargetPos;
        rightStoryObj.transform.localScale = rightTargetScale;
    }
}
