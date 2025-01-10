using UnityEngine;
using UnityEngine.UI;

public class ScrollRectMouseWheel : MonoBehaviour
{
    public ScrollRect scrollingtransform; 
    public float scrollspeed = 1f;

    void Update()
    {
        if (scrollingtransform != null)
        {
            float scrollDelta = Input.mouseScrollDelta.y;
            if (Mathf.Abs(scrollDelta) > 0.01f)
            {
                scrollingtransform.verticalNormalizedPosition += scrollDelta * scrollspeed * Time.deltaTime;
                scrollingtransform.verticalNormalizedPosition = Mathf.Clamp01(scrollingtransform.verticalNormalizedPosition);
            }
        }
    }
}
