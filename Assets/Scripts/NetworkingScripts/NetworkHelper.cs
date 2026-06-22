using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

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
        foreach (var player in players)
        {
            if (player.OwnerClientId == targetID)
            {
                PlayerHealth health = player.GetComponent<PlayerHealth>();

                if (health != null)
                {
                    health.TakeDamage(10);
                }
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
            Debug.Log("serverrpc");
            health.HealthMe(Random.Range(.15f, .2f));
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
