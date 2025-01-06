using UnityEngine;

public class TutorialPopUp : MonoBehaviour
{

    public GameObject TutorialWidget;

    public void OnDealsnap()
    {
        TutorialWidget.SetActive(false);
    } 
}
