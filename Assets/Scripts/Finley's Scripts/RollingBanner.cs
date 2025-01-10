using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class RollingBanner : MonoBehaviour
{
    public ScrollRect scrollRect;       
    public RectTransform content;       
    public float scrollSpeed = 2f;       
    public float displayTime = 5f;       
    public float smallerScale = 0.8f;    
    public float storySpacing = 300f;   
    public Transform storyParent;        
    public float scaleTransitionDuration = 1f; 
    private GameObject[] storyPrefabs;  
    private string[] storyTitles;       
    private int currentStoryIndex = 0;  
    private GameObject leftStoryObj;    
    private GameObject mainStoryObj;    
    private GameObject rightStoryObj;   
    private GameObject fourthStoryObj;  

    void Start()
    {
        PopulateStoryData();
        content.anchoredPosition = new Vector2(0, 0);
        StartCoroutine(ScrollBanner());
    }

    void PopulateStoryData()
    {
        int prefabCount = storyParent.childCount;
        storyPrefabs = new GameObject[prefabCount];
        storyTitles = new string[prefabCount];
        for (int i = 0; i < prefabCount; i++)
        {
            storyPrefabs[i] = storyParent.GetChild(i).gameObject;
            TextMeshProUGUI titleText = storyPrefabs[i].GetComponentInChildren<TextMeshProUGUI>();
            if (titleText != null)
            {
                storyTitles[i] = titleText.text;
            }
            else
            {
                //Debug.LogWarning("Prefab at index " + i + " does not contain a TextMeshProUGUI component for the title.");
            }
        }
    }
    IEnumerator ScrollBanner()
    {
        while (true)
        {
            ShowCurrentStories();
            yield return new WaitForSeconds(displayTime);
            yield return StartCoroutine(SmoothScrollStoriesLeft());
            currentStoryIndex = (currentStoryIndex + 1) % storyPrefabs.Length;
        }
    }
    void ShowCurrentStories()
    {
        if (mainStoryObj != null) Destroy(mainStoryObj);
        if (leftStoryObj != null) Destroy(leftStoryObj);
        if (rightStoryObj != null) Destroy(rightStoryObj);
        if (fourthStoryObj != null) Destroy(fourthStoryObj);
        mainStoryObj = Instantiate(storyPrefabs[currentStoryIndex], content);
        mainStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[currentStoryIndex]; 
        mainStoryObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        mainStoryObj.SetActive(true); 
        mainStoryObj.transform.localScale = Vector3.one; 
        leftStoryObj = Instantiate(storyPrefabs[(currentStoryIndex - 1 + storyPrefabs.Length) % storyPrefabs.Length], content);
        leftStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex - 1 + storyPrefabs.Length) % storyPrefabs.Length];
        leftStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(-storySpacing, 0); 
        leftStoryObj.SetActive(true); 
        leftStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); 
        rightStoryObj = Instantiate(storyPrefabs[(currentStoryIndex + 1) % storyPrefabs.Length], content);
        rightStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex + 1) % storyPrefabs.Length]; 
        rightStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing, 0);
        rightStoryObj.SetActive(true); 
        rightStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1); 
        fourthStoryObj = Instantiate(storyPrefabs[(currentStoryIndex + 2) % storyPrefabs.Length], content);
        fourthStoryObj.GetComponentInChildren<TextMeshProUGUI>().text = storyTitles[(currentStoryIndex + 2) % storyPrefabs.Length]; 
        fourthStoryObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(storySpacing * 2, 0); 
        fourthStoryObj.SetActive(true); 
        fourthStoryObj.transform.localScale = new Vector3(smallerScale, smallerScale, 1);
    }

    IEnumerator SmoothScrollStoriesLeft()
    {
        float startPosition = content.anchoredPosition.x;
        float targetPosition = startPosition - storySpacing;

        Vector3 leftStartPos = leftStoryObj.transform.localPosition;
        Vector3 leftTargetPos = new Vector3(-storySpacing, 0, 0);
        Vector3 leftStartScale = new Vector3(smallerScale, smallerScale, 1); 
        Vector3 leftTargetScale = new Vector3(smallerScale, smallerScale, 1);

        Vector3 mainStartPos = mainStoryObj.transform.localPosition;
        Vector3 mainTargetPos = Vector3.zero;
        Vector3 mainStartScale = Vector3.one; 
        Vector3 mainTargetScale = new Vector3(smallerScale, smallerScale, 1);

        Vector3 rightStartPos = rightStoryObj.transform.localPosition;
        Vector3 rightTargetPos = new Vector3(storySpacing, 0, 0);
        Vector3 rightStartScale = new Vector3(smallerScale, smallerScale, 1); 
        Vector3 rightTargetScale = Vector3.one;

        Vector3 fourthStartPos = fourthStoryObj.transform.localPosition;
        Vector3 fourthTargetPos = new Vector3(storySpacing * 2, 0, 0);
        Vector3 fourthStartScale = new Vector3(smallerScale, smallerScale, 1);
        Vector3 fourthTargetScale = new Vector3(smallerScale, smallerScale, 1); 

        float journeyLength = Mathf.Abs(targetPosition - startPosition);
        float journeyStartTime = Time.time;

        while (Mathf.Abs(content.anchoredPosition.x - targetPosition) > 0.1f)
        {
            float journeyProgress = (Time.time - journeyStartTime) * scrollSpeed / journeyLength;
            float newPosition = Mathf.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0f, 1f, journeyProgress));
            content.anchoredPosition = new Vector2(newPosition, content.anchoredPosition.y);

            leftStoryObj.transform.localPosition = Vector3.Lerp(leftStartPos, leftTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            leftStoryObj.transform.localScale = Vector3.Lerp(leftStartScale, leftTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            mainStoryObj.transform.localPosition = Vector3.Lerp(mainStartPos, mainTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            mainStoryObj.transform.localScale = Vector3.Lerp(mainStartScale, mainTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            rightStoryObj.transform.localPosition = Vector3.Lerp(rightStartPos, rightTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localScale = Vector3.Lerp(rightStartScale, rightTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            fourthStoryObj.transform.localPosition = Vector3.Lerp(fourthStartPos, fourthTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            fourthStoryObj.transform.localScale = Vector3.Lerp(fourthStartScale, fourthTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }

        content.anchoredPosition = new Vector2(targetPosition, content.anchoredPosition.y);
        content.anchoredPosition = new Vector2(0, content.anchoredPosition.y);
        ResetPanelPositions();
    }

    void ResetPanelPositions()
    {
        StartCoroutine(SmoothPanelTransition());
    }

    IEnumerator SmoothPanelTransition()
    {
        Vector3 leftStartPos = leftStoryObj.transform.localPosition;
        Vector3 leftTargetPos = new Vector3(-storySpacing, 0, 0); 
        Vector3 leftStartScale = new Vector3(smallerScale, smallerScale, 1); 
        Vector3 leftTargetScale = new Vector3(smallerScale, smallerScale, 1); 

        Vector3 mainStartPos = mainStoryObj.transform.localPosition;
        Vector3 mainTargetPos = Vector3.zero; 
        Vector3 mainStartScale = Vector3.one; 
        Vector3 mainTargetScale = Vector3.one;

        Vector3 rightStartPos = rightStoryObj.transform.localPosition;
        Vector3 rightTargetPos = new Vector3(storySpacing, 0, 0); 
        Vector3 rightStartScale = new Vector3(smallerScale, smallerScale, 1);
        Vector3 rightTargetScale = new Vector3(smallerScale, smallerScale, 1); 

        Vector3 fourthStartPos = fourthStoryObj.transform.localPosition;
        Vector3 fourthTargetPos = new Vector3(storySpacing * 2, 0, 0); 
        Vector3 fourthStartScale = new Vector3(smallerScale, smallerScale, 1);
        Vector3 fourthTargetScale = new Vector3(smallerScale, smallerScale, 1); 

        float transitionDuration = scaleTransitionDuration;

        float journeyStartTime = Time.time;

        while (Time.time - journeyStartTime < transitionDuration)
        {
            float journeyProgress = (Time.time - journeyStartTime) / transitionDuration;

            leftStoryObj.transform.localPosition = Vector3.Lerp(leftStartPos, leftTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            leftStoryObj.transform.localScale = Vector3.Lerp(leftStartScale, leftTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            mainStoryObj.transform.localPosition = Vector3.Lerp(mainStartPos, mainTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            mainStoryObj.transform.localScale = Vector3.Lerp(mainStartScale, mainTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localPosition = Vector3.Lerp(rightStartPos, rightTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            rightStoryObj.transform.localScale = Vector3.Lerp(rightStartScale, rightTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));
            fourthStoryObj.transform.localPosition = Vector3.Lerp(fourthStartPos, fourthTargetPos, Mathf.SmoothStep(0f, 1f, journeyProgress));
            fourthStoryObj.transform.localScale = Vector3.Lerp(fourthStartScale, fourthTargetScale, Mathf.SmoothStep(0f, 1f, journeyProgress));

            yield return null;
        }
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
