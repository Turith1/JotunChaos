using UnityEngine;
using Unity.Netcode;
using StarterAssets;
using System.Collections;

public class PlayerHealth : NetworkBehaviour
{

    [SerializeField]
    private GameObject playerMesh;
    [SerializeField]
    private float totalHealth;
    [SerializeField]
    private bool startsAsJotun;

    //public bool isDead = false;
    public NetworkVariable<bool> isJotun = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public NetworkVariable<float> health = new NetworkVariable<float>(100, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    public override void OnNetworkSpawn()
    {
        if (IsServer)
        {
            isJotun.Value = startsAsJotun;
        }
    }



    private void Start()
    {
        health.OnValueChanged += OnHealthChanged;
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer)
            return;

        Debug.Log($"{OwnerClientId} health: {health.Value}");

        if (health.Value <= 0 || isDead.Value)
            return;

        health.Value -= damage;

        if (health.Value <= 0)
        {
            isDead.Value = true;

            Die();

            NetworkHelper.instance.CheckForGameEnd();
        }
    }

    public void HealthMe(float healAmount)
    {
        Debug.Log("HealMe called");
        if (!IsServer)
            return;

        if (health.Value <= 0)
            return;

        float healValue = totalHealth * healAmount;

        Debug.Log("heal called");
        health.Value = Mathf.Min(health.Value + healValue, totalHealth);
    }

    private void Die()
    {
        Debug.Log($"{OwnerClientId} died");

        if (IsServer)
        {
            HidePlayerClientRpc();
        }
    }

    [ClientRpc]
    private void HidePlayerClientRpc()
    {
        playerMesh.SetActive(false);
        GetComponent<ThirdPersonControls>().enabled = false;
    }

    private void OnHealthChanged(float previous, float current)
    {
        Debug.Log("New Health: " + current);

        // update hp bar
    }
}
