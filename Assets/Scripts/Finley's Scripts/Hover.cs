using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;  // Add this line

public class HoverPanelEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 enlargedSize = new Vector3(1.2f, 1.2f, 1);  // Size when hovered
    public Vector3 normalSize = new Vector3(1f, 1f, 1);       // Normal size when not hovered
    public float scaleSpeed = 0.2f;  // Speed of scaling

    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = normalSize;  // Ensure the panel starts at normal size
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();  // Stop any ongoing scaling animation
        StartCoroutine(ScalePanel(enlargedSize));  // Scale to enlarged size
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();  // Stop any ongoing scaling animation
        StartCoroutine(ScalePanel(normalSize));  // Scale back to normal size
    }

    // Coroutine to smoothly scale the panel with quadratic easing-in
    private IEnumerator ScalePanel(Vector3 targetSize)
    {
        Vector3 startScale = rectTransform.localScale;
        float timeElapsed = 0f;

        while (timeElapsed < scaleSpeed)
        {
            float t = timeElapsed / scaleSpeed;
            t = t * t;  // Apply quadratic easing-in (t^2)

            // Interpolate between the start and target scale with easing
            rectTransform.localScale = Vector3.Lerp(startScale, targetSize, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        rectTransform.localScale = targetSize; // Ensure the final scale is exact
    }
}
