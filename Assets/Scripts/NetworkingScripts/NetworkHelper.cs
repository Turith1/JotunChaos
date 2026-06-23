using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;
using System.Linq;

public class NetworkHelper : MonoBehaviour
{
    [SerializeField]
    private float detectionDistance = 50f;
    [SerializeField]
    private Vector2 detectionSize = new Vector2(2f, 2f);
    [SerializeField]
    private LayerMask playerLayer;
    [SerializeField]
    private string playerTag;
    [SerializeField]
    private GameObject wisper;

    public static NetworkHelper instance;
    public List<NetworkObject> players;

    private void Awake()
    {
        instance = this;
        DontDestroyOnLoad(this);
    }

    [Rpc(SendTo.Server)]
    public void RequestAttackServerRpc(ulong attackerID, ulong targetID)
    {
        foreach (var player in players.ToList())
        {
            if (player.OwnerClientId == targetID)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();

                if (health != null)
                {
                    health.TakeDamage(10);
                }

                break;
            }
        }
    }

    public void RAbilityCheck(int whosCasting)
    {
        Debug.Log("Check called");
        // Center of screen
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);

        // Ray from camera through center
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        RaycastHit[] hits = Physics.BoxCastAll(ray.origin, detectionSize / 2f, ray.direction, Quaternion.identity, detectionDistance, playerLayer);

        foreach (RaycastHit hit in hits)
        {
            Debug.Log(hit.collider.tag);
            if (hit.collider.CompareTag(playerTag))
            {
                Debug.Log(hit.collider.tag);
                PlayerHealth health = hit.collider.GetComponentInParent<PlayerHealth>();
                if (health != null)
                {
                    //health.HealthMeClientRpc(Random.Range(.15f, .2f));
                    RAbilityServerRpc(whosCasting, health);
                }
            }
        }
    }

    [Rpc(SendTo.Server)]
    public void RAbilityServerRpc(int whosCasting, PlayerHealth health)
    {
        if(whosCasting == 0)
        {
            health.HealthMe(Random.Range(.15f, .2f));
        }
    }

    public void CheckForGameEnd()
    {
        Debug.Log("check called");
        bool jotunAlive = false;
        bool playersAlive = false;

        foreach (var player in players)
        {
            if (player == null)
                continue;

            PlayerHealth health = player.GetComponent<PlayerHealth>();

            if (health == null)
                continue;

            if (!health.isDead.Value)
            {
                if (health.isJotun.Value)
                    jotunAlive = true;
                else
                    playersAlive = true;
            }
        }

        // Solo player died
        if (!jotunAlive)
        {
            Debug.Log("Jotun died - other team wins");
            ResetLobby();
            return;
        }

        // Entire team died
        if (!playersAlive)
        {
            Debug.Log("Team died - Jotun wins");
            ResetLobby();
            return;
        }
    }

    private void ResetLobby()
    {
        List<ulong> clients = NetworkManager.Singleton.ConnectedClientsIds.ToList();

        foreach (ulong clientId in clients)
        {
            NetworkObject oldPlayer = NetworkManager.Singleton.ConnectedClients[clientId].PlayerObject;

            if (oldPlayer != null)
            {
                oldPlayer.Despawn(true);
            }

            GameObject obj = Instantiate(wisper);

            obj.GetComponent<NetworkObject>()
                .SpawnAsPlayerObject(clientId, true);

            LobbyManager.Instance.ShowHideStartBTNClientRpc(true);
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        Gizmos.DrawWireCube(
            Camera.main.transform.position + Camera.main.transform.forward * 5f,
            detectionSize
        );
    }
}
