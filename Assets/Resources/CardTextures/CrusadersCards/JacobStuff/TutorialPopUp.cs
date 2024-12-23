using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine.SceneManagement;

public class TutorialPopUp : MonoBehaviour
{
    public GameObject TutorialWidget;

    public void OnDealsnap()
    {
        TutorialWidget.SetActive(false);
    }
}
