using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class CubeTestBackToLobby : NetworkBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (!other.GetComponent<NetworkObject>())
            return;

        NetworkManager.Singleton.SceneManager.LoadScene(
            "LobbyScene",
            LoadSceneMode.Single
        );
    }

    /*[ServerRpc(RequireOwnership = false)]
    private void RequestReturnToLobbyServerRpc()
    {
        if (!IsServer)
            return;

        NetworkManager.Singleton.SceneManager.LoadScene(
            "LobbyScene",
            LoadSceneMode.Single
        );
    }*/
}