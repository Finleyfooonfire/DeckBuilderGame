using UnityEngine;
using UnityEngine.UI;

public class CardUIController : MonoBehaviour
{
    public Image cardImageUI; // The UI Image component where the card image will be displayed
    public Button cardButton; // The Button component for interaction

    private void Start()
    {
        // Get the CardDetails component from this GameObject
        CardDetails cardDetails = GetComponent<CardDetails>();

        if (cardDetails == null)
        {
            Debug.LogError($"No CardDetails component found on {gameObject.name}. Ensure it is attached to the card prefab.");
            return;
        }

        if (cardImageUI == null)
        {
            Debug.LogError($"No Image reference assigned in {gameObject.name}. Assign the UI Image in the Inspector.");
            return;
        }

        // Update the Image UI element with the card's image
        if (cardDetails.cardImage != null)
        {
            cardImageUI.sprite = cardDetails.cardImage;
        }
        else
        {
            Debug.LogWarning($"CardDetails on {gameObject.name} does not have a cardImage set.");
        }

        // Ensure the cardImageUI has a Button component
        if (cardButton == null)
        {
            Debug.LogError($"No Button reference assigned in {gameObject.name}. Assign the Button in the Inspector.");
            return;
        }

        // Add a click event to the button to trigger the large view action
        cardButton.onClick.AddListener(() => OpenLargeView(cardDetails));
    }

    private void OpenLargeView(CardDetails cardDetails)
    {
        if (LargeViewController.Instance != null)
        {
            // Send the CardDetails to the LargeViewController
            LargeViewController.Instance.ShowCardDetails(cardDetails);
        }
        else
        {
            Debug.LogError("LargeViewController is not initialized in the scene.");
        }
    }
}
