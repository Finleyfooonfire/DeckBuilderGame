using UnityEngine;

public class NetworkManager : MonoBehaviour
{
    bool isHost;
    [SerializeField] GameObject server;
    [SerializeField] GameObject serverUI;
    [SerializeField] GameObject clientUI;
    [SerializeField] GameObject startButtons;
    

    public void OnStartGame(bool setHost)
    {
        startButtons.SetActive(false);
        isHost = setHost;
        StartUpNetwork();
    }

    void StartUpNetwork()
    {
        if (isHost)
        {
            server.SetActive(true);
            serverUI.SetActive(true);
            clientUI.SetActive(true);
        }
        else
        {
            server.SetActive(false);
            serverUI.SetActive(false);
            clientUI.SetActive(true);
        }
    }
}
