using UnityEngine;
using Unity.Netcode;

public class BulletBehave : NetworkBehaviour
{
    [SerializeField] Rigidbody rb;
    int _speed = 5;

    private void FixedUpdate()
    {
        if (!IsServer)
            return;

        rb.linearVelocity = transform.forward * _speed;
        Invoke("MorraServerRpc", 3);
    }

    [ServerRpc]
    public void MorraServerRpc()
    {
        GetComponent<NetworkObject>().Despawn();
    }
}
