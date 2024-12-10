using UnityEngine;
using UnityEngine.UI;

public class CardZoomPanel : MonoBehaviour
{
    private RectTransform rectTransform;
    private Image image;
    public float zoomInTime = 0.2f;
    private Vector2 targetSize;
    private Vector2 startSize;
    private float currentZoomTime;
    private bool isZooming = false;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();
        targetSize = rectTransform.sizeDelta;
        startSize = targetSize * 0.1f;
    }

    void OnEnable()
    {
        rectTransform.sizeDelta = startSize;
        currentZoomTime = 0;
        isZooming = true;
    }

    void Update()
    {
        if (isZooming)
        {
            currentZoomTime += Time.deltaTime;
            float progress = currentZoomTime / zoomInTime;

            if (progress >= 1)
            {
                rectTransform.sizeDelta = targetSize;
                isZooming = false;
            }
            else
            {
                rectTransform.sizeDelta = Vector2.Lerp(startSize, targetSize, progress);
            }
        }
    }
}