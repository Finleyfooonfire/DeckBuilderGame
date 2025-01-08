using UnityEngine;

public class ScrollImage : MonoBehaviour
{
    public RectTransform imageTransform; // The RectTransform of the UI Image
    public float scrollSpeed = 10f; // Speed at which the image moves with the scroll wheel
    public float maxY = 500f; // Maximum Y position
    public float minY = -500f; // Minimum Y position

    [Header("Content Panel Setup")]
    public RectTransform contentPanel; // The RectTransform of the content panel
    public float resetPositionY = 0f; // The Y position to reset the content panel to

    private void Start()
    {
        // Reset the content panel's position if assigned
        if (contentPanel != null)
        {
            Vector2 position = contentPanel.anchoredPosition;
            position.y = resetPositionY;
            contentPanel.anchoredPosition = position;

            Debug.Log($"Content panel position reset to Y: {resetPositionY}");
        }
        else
        {
            Debug.LogWarning("No content panel assigned for resetting.");
        }
    }

    private void Update()
    {
        if (imageTransform == null)
        {
            Debug.LogError("Image Transform is not assigned.");
            return;
        }

        // Get the scroll wheel input
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        // Move the image vertically based on scroll input
        if (scrollDelta != 0)
        {
            Vector2 position = imageTransform.anchoredPosition;
            position.y -= scrollDelta * scrollSpeed;

            // Clamp the position within the defined limits
            position.y = Mathf.Clamp(position.y, minY, maxY);

            imageTransform.anchoredPosition = position;
        }
    }
}
