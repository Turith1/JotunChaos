using UnityEngine;
using Unity.Netcode;
using System.Collections.Generic;

public class NetworkHelper : MonoBehaviour
{
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
}
