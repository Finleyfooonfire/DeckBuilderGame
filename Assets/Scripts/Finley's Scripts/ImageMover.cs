using UnityEngine;

public class ScrollImage : MonoBehaviour
{
    public RectTransform imageTransform;
    public float scrollingspeed = 10f; 
    public float maxheight = 500f; 
    public float minumumheight = -500f; 

    [Header("Content Panel Setup")]
    public RectTransform contentPanel; 
    public float resetPositionY = 0f; 

    private void Start() 
    {
        if (contentPanel != null) //panels are jumpting to the top so this sorts it
        {
            Vector2 position = contentPanel.anchoredPosition;
            position.y = resetPositionY;
            contentPanel.anchoredPosition = position;

            //Debug.Log($"panel is reset to the start");
        }
        else
        {
           // Debug.LogWarning("no panel there.");
        }
    }

    private void Update()
    {
        if (imageTransform == null)
        {
            //Debug.LogError("the transform isn't there");
            return;
        }
        float scrollDelta = Input.GetAxis("Mouse ScrollWheel");

        if (scrollDelta != 0)
        {
            Vector2 position = imageTransform.anchoredPosition;
            position.y -= scrollDelta * scrollingspeed;
            position.y = Mathf.Clamp(position.y, minumumheight, maxheight);

            imageTransform.anchoredPosition = position;
        }
    }
}
