using UnityEngine;
using UnityEngine.UI;

public class ScrollRectMouseWheel : MonoBehaviour
{
    public ScrollRect scrollRect; // Reference to the ScrollRect component
    public float scrollSpeed = 1f; // Speed at which to scroll

    void Update()
    {
        if (scrollRect != null)
        {
            // Get the scroll input from the mouse wheel (scrolling vertically)
            float scrollDelta = Input.mouseScrollDelta.y;

            // If the wheel was scrolled, adjust the ScrollRect's vertical position
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                // Scroll the content up or down based on the mouse wheel input
                scrollRect.verticalNormalizedPosition += scrollDelta * scrollSpeed * Time.deltaTime;

                // Clamp the scroll position to stay within bounds
                scrollRect.verticalNormalizedPosition = Mathf.Clamp01(scrollRect.verticalNormalizedPosition);
            }
        }
    }
}
