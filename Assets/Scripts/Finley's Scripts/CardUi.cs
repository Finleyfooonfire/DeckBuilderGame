using UnityEngine;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour
{
    public Image cardsimage; 
    public Button cardbutton; 

    private void Start()
    {
        CardDetails cardDetails = GetComponent<CardDetails>();

        if (cardDetails == null)
        {
           // Debug.LogError($"no card details");
            return;
        }

        if (cardsimage == null)
        {
           // Debug.LogError($"no images");
            return;
        }
        if (cardDetails.cardImage != null)
        {
            cardsimage.sprite = cardDetails.cardImage;
        }
        else
        {
           // Debug.LogWarning($"no card image");
        }
        if (cardbutton == null)
        {
           // Debug.LogError($"no button here");
            return;
        }
        cardbutton.onClick.AddListener(() => OpenLargeView(cardDetails));
    }

    private void OpenLargeView(CardDetails cardDetails)
    {
        if (LargeViewController.Instance != null)
        {
            LargeViewController.Instance.ShowCardDetails(cardDetails);
        }
        else
        {
           // Debug.LogError("");
        }
    }
}
