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
    private GameObject fourthStoryObj;  // The story that will appear next to the right story

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
            // Show the current stories (left, main, right, upcoming 4th)
            ShowCurrentStories();

            // Wait for the display time (let the user read the current story)
            yield return new WaitForSeconds(displayTime);

            // Scroll the current stories to the left with smooth animation
            yield return StartCoroutine(SmoothScrollStoriesLeft());

            // Move to the next story
            currentStoryIndex = (currentStoryIndex + 1) % storyPrefabs.Length; // Loop back after the last story
        }
    }

    // Instantiate and display the current stories: left, main, right, and 4th upcoming story
    void ShowCurrentStories()
    {
        // Destroy previous stories if they exist
        if (mainStoryObj != null) Destroy(mainStoryObj);
        if (leftStoryObj != null) Destroy(leftStoryObj);
        if (rightStoryObj != null) Destroy(rightStoryObj);
        if (fourthStoryObj != null) Destroy(fourthStoryObj);

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
        leftStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size (keeps small)

        // Instantiate the right story panel (side story on the right) - next story
        rightStoryObj = Instantiate(storyPrefabs[(currentStoryIndex + 1) % storyPrefabs.Length], content);
        rightStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex + 1) % storyPrefabs.Length]; // Next story
        rightStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing, 0); // Position to the right
        rightStoryObj.SetActive(true); // Ensure the prefab is set to active
        rightStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size

        // Instantiate the upcoming 4th story panel (next to the right)
        fourthStoryObj = Instantiate(storyPrefabs[(currentStoryIndex + 2) % storyPrefabs.Length], content);
        fourthStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex + 2) % storyPrefabs.Length]; // Next upcoming story
        fourthStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing * 2, 0); // Position next to the right
        fourthStoryObj.SetActive(true); // Ensure the prefab is set to active
        fourthStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); // Smaller size
    }

    // Coroutine to handle smooth scrolling of the stories to the left
    IEnumerator SmoothScrollStoriesLeft()
    {
        // Starting positions and scales
        float startPosition = content.anchoredPosition.x;
        float targetPosition = startPosition - storySpacing;

        // Starting positions and scales for the story panels
        Vector3 leftStartPos = leftStoryObj.transform.localPosition;
        Vector3 leftTargetPos = new Vector3(-storySpacing, 0, 0);
        Vector3 leftStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale (keeps it small)
        Vector3 leftTargetScale = new Vector3(smallerScale, smallerScale, 1); // keeps smaller scale

        Vector3 mainStartPos = mainStoryObj.transform.localPosition;
        Vector3 mainTargetPos = Vector3.zero;
        Vector3 mainStartScale = Vector3.one; // normal scale (full size)
        Vector3 mainTargetScale = new Vector3(smallerScale, smallerScale, 1); // transition to smaller scale

        Vector3 rightStartPos = rightStoryObj.transform.localPosition;
        Vector3 rightTargetPos = new Vector3(storySpacing, 0, 0);
        Vector3 rightStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale
        Vector3 rightTargetScale = Vector3.one; // transition to full size

        Vector3 fourthStartPos = fourthStoryObj.transform.localPosition;
        Vector3 fourthTargetPos = new Vector3(storySpacing * 2, 0, 0);
        Vector3 fourthStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale
        Vector3 fourthTargetScale = new Vector3(smallerScale, smallerScale, 1); // keep smaller scale

        // Transition duration for scrolling (content scroll)
        float journeyLength = Mathf.Abs(targetPosition - startPosition);
        float journeyStartTime = Time.time;

        // Smoothly animate both the content's position and story panels' positions & scales
        while (Mathf.Abs(content.anchoredPosition.x - targetPosition) > 0.1f)
        {
            float journeyProgress = (Time.time - journeyStartTime) * scrollSpeed / journeyLength;

            // Smoothly transition the content position (scrolling effect)
            float newPosition = Mathf.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0f, 1f, journeyProgress));
            content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

            // Smoothly transition the left story position and scale (stays smaller)
            leftStoryObj.transform.localPosition = Vector3.Lerp(leftStartPos, leftTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            leftStoryObj.transform.localScale = Vector3.Lerp(leftStartScale, leftTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the main story position and scale (scales down to smaller)
            mainStoryObj.transform.localPosition = Vector3.Lerp(mainStartPos, mainTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            mainStoryObj.transform.localScale = Vector3.Lerp(mainStartScale, mainTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the right story position and scale (scales up to full size)
            rightStoryObj.transform.localPosition = Vector3.Lerp(rightStartPos, rightTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localScale = Vector3.Lerp(rightStartScale, rightTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the 4th story position and scale (keeps smaller)
            fourthStoryObj.transform.localPosition = Vector3.Lerp(fourthStartPos, fourthTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            fourthStoryObj.transform.localScale = Vector3.Lerp(fourthStartScale, fourthTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }

        // Ensure content ends exactly at the target position
        content.anchoredPosition = new Vector2(targetPosition, content.anchoredPosition.y);

        // Reset the content position for the next set of stories
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y); // Reset to the center

        // After scrolling, reset the positions and scales of the panels for the next set
        ResetPanelPositions();
    }

    // Reset the panel positions for the next scroll and scale them smoothly
    void ResetPanelPositions()
    {
        StartCoroutine(SmoothPanelTransition());
    }

    // Smoothly transition the left and right panels to their positions and scales
    // Smoothly transition the left, right, and 4th panels to their positions and scales
    // Smoothly transition the left, right, and 4th panels to their positions and scales
    IEnumerator SmoothPanelTransition()
    {
        // Start positions and scales of the panels
        Vector3 leftStartPos = leftStoryObj.transform.localPosition;
        Vector3 leftTargetPos = new Vector3(-storySpacing, 0, 0); // The left position stays at -storySpacing
        Vector3 leftStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale
        Vector3 leftTargetScale = new Vector3(smallerScale, smallerScale, 1); // stays smaller

        Vector3 mainStartPos = mainStoryObj.transform.localPosition;
        Vector3 mainTargetPos = Vector3.zero; // Main story stays at center (0)
        Vector3 mainStartScale = Vector3.one; // Starts as larger scale
        Vector3 mainTargetScale = Vector3.one; // Main story stays at normal size

        Vector3 rightStartPos = rightStoryObj.transform.localPosition;
        Vector3 rightTargetPos = new Vector3(storySpacing, 0, 0); // The right position stays at storySpacing
        Vector3 rightStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale
        Vector3 rightTargetScale = new Vector3(smallerScale, smallerScale, 1); // stays smaller

        Vector3 fourthStartPos = fourthStoryObj.transform.localPosition;
        Vector3 fourthTargetPos = new Vector3(storySpacing * 2, 0, 0); // 4th story at storySpacing * 2
        Vector3 fourthStartScale = new Vector3(smallerScale, smallerScale, 1); // smaller scale
        Vector3 fourthTargetScale = new Vector3(smallerScale, smallerScale, 1); // stays smaller

        // Transition duration for smooth animation
        float transitionDuration = scaleTransitionDuration;

        // Time tracking for the transition
        float journeyStartTime = Time.time;

        // Transition all four panels' positions and scales smoothly
        while (Time.time - journeyStartTime < transitionDuration)
        {
            float journeyProgress = (Time.time - journeyStartTime) / transitionDuration;

            // Smoothly transition the position and scale for the left story (small scale)
            leftStoryObj.transform.localPosition = Vector3.Lerp(leftStartPos, leftTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            leftStoryObj.transform.localScale = Vector3.Lerp(leftStartScale, leftTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the position and scale for the main story (larger scale)
            mainStoryObj.transform.localPosition = Vector3.Lerp(mainStartPos, mainTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            mainStoryObj.transform.localScale = Vector3.Lerp(mainStartScale, mainTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the position and scale for the right story (small scale)
            rightStoryObj.transform.localPosition = Vector3.Lerp(rightStartPos, rightTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localScale = Vector3.Lerp(rightStartScale, rightTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            // Smoothly transition the position and scale for the 4th story (stay smaller, no scale change)
            fourthStoryObj.transform.localPosition = Vector3.Lerp(fourthStartPos, fourthTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            fourthStoryObj.transform.localScale = Vector3.Lerp(fourthStartScale, fourthTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }

        // Ensure all panels end at their exact target positions and scales
        leftStoryObj.transform.localPosition = leftTargetPos;
        leftStoryObj.transform.localScale = leftTargetScale;

        mainStoryObj.transform.localPosition = mainTargetPos;
        mainStoryObj.transform.localScale = mainTargetScale;

        rightStoryObj.transform.localPosition = rightTargetPos;
        rightStoryObj.transform.localScale = rightTargetScale;

        fourthStoryObj.transform.localPosition = fourthTargetPos;
        fourthStoryObj.transform.localScale = fourthTargetScale;
    }


}
