using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RollingBanner : MonoBehaviour
{
    public ScrollRect scrollRect;    // The ScrollRect component controlling the scrolling
    public RectTransform content;    // The Content of the ScrollRect (the container of stories)
    public float scrollSpeed = 50f;  // How fast the banner scrolls
    public float resetPosition = -1000f;  // Position to reset content once it scrolls off-screen

    public GameObject storyButtonPrefab;   // The prefab for story buttons
    public Transform buttonParent;  // Where to place the story buttons

    // Example stories
    private string[] stories = {
        "The Rise of the Ancient Empire",
        "The Lost Temple of the Gods",
        "The Battle of Darkwater",
        "A Hero's Journey: Chronicles of the First Hero"
    };

    void Start()
    {
        // Setup the banner's initial position
        content.anchoredPosition = new Vector2(0, 0);

        // Create story buttons dynamically
        CreateStoryButtons();

        // Start scrolling the banner
        StartCoroutine(ScrollBanner());
    }

    // Create buttons dynamically for each story
    void CreateStoryButtons()
    {
        foreach (string story in stories)
        {
            GameObject button = Instantiate(storyButtonPrefab, buttonParent);
            button.GetComponentInChildren<TextMeshProUGUI>().text = story;

            // Add functionality to button click (You can replace this with your own logic)
            button.GetComponent<Button>().onClick.AddListener(() => OpenStory(story));
        }
    }

    // Method to open a specific story (You can modify this to show a full lore page, etc.)
    void OpenStory(string story)
    {
        Debug.Log("Opening story: " + story);
        // Replace with your logic to open the specific story or lore panel
    }

    // Coroutine that handles the scrolling of the content
    IEnumerator ScrollBanner()
    {
        while (true)
        {
            // Move the content to the left continuously
            content.anchoredPosition += new Vector2(-scrollSpeed * Time.deltaTime, 0);

            // If the content has completely moved off the screen, reset its position
            if (content.anchoredPosition.x <= resetPosition)
            {
                content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
            }

            yield return null;
        }
    }
}
