using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LargeViewController : MonoBehaviour
{
    public static LargeViewController Instance { get; private set; } // Singleton instance

    public GameObject largeViewCanvas; // The canvas displaying the large view
    public Image largeViewImage; // The Image component for displaying the card image
    public TextMeshProUGUI cardNameText; // Text for the card name
    public TextMeshProUGUI cardDescriptionText; // Text for the card description
    public GameObject inspectCanvas;  // The canvas with lore content
    public GameObject CardCanvas;

    private void Awake()
    {
        // Ensure there's only one instance of this controller
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("Multiple instances of LargeViewController detected. Destroying duplicate.");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // Ensure the large view canvas starts hidden
        if (largeViewCanvas != null)
        {
            largeViewCanvas.SetActive(false);
        }
        else
        {
            Debug.LogError("LargeViewCanvas is not assigned in LargeViewController.");
        }
    }

    // Method to display card details on the large view canvas
    public void ShowCardDetails(CardDetails cardDetails)
    {
        if (cardDetails == null)
        {
            Debug.LogError("CardDetails is null. Cannot display in large view.");
            return;
        }

        // Show the large view canvas
        if (largeViewCanvas != null)
        {
            largeViewCanvas.SetActive(true);
        }

        // Update the large view with the card details
        if (largeViewImage != null && cardDetails.cardImage != null)
        {
            largeViewImage.sprite = cardDetails.cardImage;
        }

        if (cardNameText != null)
        {
            cardNameText.text = cardDetails.cardName;
        }

        if (cardDescriptionText != null)
        {
            cardDescriptionText.text = cardDetails.cardDescription;
        }
    }

    // Optional: Method to hide the large view
    public void HideLargeView()
    {
        if (largeViewCanvas != null)
        {
            largeViewCanvas.SetActive(false);
        }
    }

    public void CloseInspectCanvas()
    {
        CardCanvas.SetActive(true); // enables the card canvas
        inspectCanvas.SetActive(false); //Hides the inspect canvas
    }
}
