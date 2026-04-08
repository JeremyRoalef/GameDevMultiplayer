using Unity.Netcode;
using UnityEngine;

public class OverlayCanvas : MonoBehaviour
{
    public void OnStartClientClicked()
    {
        NetworkManager.Singleton.StartClient();
    }

    public void OnStartHostClicked()
    {
        NetworkManager.Singleton.StartHost();
    }
}
