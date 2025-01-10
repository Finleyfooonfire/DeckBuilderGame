using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;  

public class HoverPanelEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Vector3 enlargedSize = new Vector3(1.2f, 1.2f, 1); 
    public Vector3 normalSize = new Vector3(1f, 1f, 1);      
    public float scaleSpeed = 0.2f;  

    private RectTransform rectTransform;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        rectTransform.localScale = normalSize;  
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();  
        StartCoroutine(ScalePanel(enlargedSize));  
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines(); 
        StartCoroutine(ScalePanel(normalSize));  
    }
    private IEnumerator ScalePanel(Vector3 targetSize)
    {
        Vector3 startScale = rectTransform.localScale;
        float timeElapsed = 0f;
        while (timeElapsed < scaleSpeed)
        {
            float t = timeElapsed / scaleSpeed;
            t = t * t;  
            rectTransform.localScale = Vector3.Lerp(startScale, targetSize, t);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        rectTransform.localScale = targetSize; 
    }
}
