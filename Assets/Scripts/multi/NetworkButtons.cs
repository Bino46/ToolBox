using UnityEngine;
using Unity.Netcode;

public class NetworkButtons : MonoBehaviour
{
    public void Hosting()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Servering()
    {
        NetworkManager.Singleton.StartServer();
    }
    
    public void Clienting()
    {
        NetworkManager.Singleton.StartClient();
    }
}
