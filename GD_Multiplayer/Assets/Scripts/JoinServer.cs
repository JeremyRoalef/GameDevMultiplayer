using Unity.Netcode;
using UnityEngine;

public class JoinServer : MonoBehaviour
{
    public void OnJoinServerClicked()
    {
        NetworkManager.Singleton.StartClient();
    }
}
