using UnityEngine;

public class HideEnemyButtons : MonoBehaviour
{
    public GameObject hostButton;
    public GameObject clientButton;

    void Start()
    {
        var isHost = FindObjectOfType<NetworkConnector>().isHost;
        
        hostButton.SetActive(isHost);   
        clientButton.SetActive(!isHost);   
    }
}
