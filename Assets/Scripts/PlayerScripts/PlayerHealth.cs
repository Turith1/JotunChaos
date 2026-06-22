using UnityEngine;
using Unity.Netcode;
using StarterAssets;

public class PlayerHealth : NetworkBehaviour
{

    [SerializeField]
    private GameObject playerMesh;
    [SerializeField]
    private float totalHealth;

    public NetworkVariable<float> health = new NetworkVariable<float>(
        100,
        NetworkVariableReadPermission.Everyone,
        NetworkVariableWritePermission.Server
    );

    private void Start()
    {
        health.OnValueChanged += OnHealthChanged;
        Invoke("HideBTN", .5f);
    }

    private void HideBTN()
    {
        GameObject btn = GameObject.Find("LobbyUIElements");
        if (btn != null)
            btn.SetActive(false);
    }

    public void TakeDamage(int damage)
    {
        if (!IsServer)
            return;

        health.Value -= damage;

        Debug.Log($"{OwnerClientId} health: {health.Value}");

        if (health.Value <= 0)
        {
            Die();
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

        if (IsOwner)
        {
            GetComponent<ThirdPersonControls>().enabled = false;
        }
    }

    private void OnHealthChanged(float previous, float current)
    {
        Debug.Log("New Health: " + current);

        // update hp bar
    }
}
