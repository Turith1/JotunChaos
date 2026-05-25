using UnityEngine;
using Unity.Netcode;

public class LobbySpawner : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    private void Start()
    {
        if (!NetworkManager.Singleton.IsServer)
            return;

        NetworkManager.Singleton.OnClientConnectedCallback += SpawnPlayer;

        // Spawn already connected clients (INCLUDING HOST)
        foreach (ulong clientId in NetworkManager.Singleton.ConnectedClientsIds)
        {
            SpawnPlayer(clientId);
        }
    }

    private void SpawnPlayer(ulong clientId)
    {
        // Prevent duplicate spawn
        if (NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject != null)
            return;

        GameObject player = Instantiate(
            playerPrefab,
            GetSpawnPosition(),
            Quaternion.identity
        );

        player.GetComponent<NetworkObject>()
            .SpawnAsPlayerObject(clientId);
    }

    private Vector3 GetSpawnPosition()
    {
        return new Vector3(
            Random.Range(-5, 5),
            1,
            Random.Range(-5, 5)
        );
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= SpawnPlayer;
        }
    }
}
