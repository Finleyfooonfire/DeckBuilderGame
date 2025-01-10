using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LargeViewController : MonoBehaviour
{
    public static LargeViewController Instance { get; private set; }

    public GameObject inpectioncanvas; 
    public Image inspectionimage;
    public TextMeshProUGUI cardtextstuff;
    public TextMeshProUGUI inspectcarddescription; 
    public GameObject otherinspectcanvas;  
    public GameObject thecardcanvas;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
           // Debug.LogError("two versions made, destroying them now");
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        if (inpectioncanvas != null)
        {
            inpectioncanvas.SetActive(false);
        }
        else
        {
            //Debug.LogError("zoom canvas isnt there.");
        }
    }

    public void ShowCardDetails(CardDetails cardDetails)
    {
        if (cardDetails == null)
        {
            //Debug.LogError("there is no card details");
            return;
        }

        if (inpectioncanvas != null)
        {
            inpectioncanvas.SetActive(true);
        }

        if (inspectionimage != null && cardDetails.cardImage != null)
        {
            inspectionimage.sprite = cardDetails.cardImage;
        }

        if (cardtextstuff != null)
        {
            cardtextstuff.text = cardDetails.cardName;
        }

        if (inspectcarddescription != null)
        {
            inspectcarddescription.text = cardDetails.cardDescription;
        }
    }

    public void HideLargeView()
    {
        if (inpectioncanvas != null)
        {
            inpectioncanvas.SetActive(false);
        }
    }

    public void CloseInspectCanvas()
    {
        thecardcanvas.SetActive(true);
        otherinspectcanvas.SetActive(false); 
    }
}
