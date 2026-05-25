using Unity.Netcode;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    [SerializeField]
    private float raioDaEsfera = .3f;
    [SerializeField]
    private string oponentTag;
    private bool canAttack = true;

    private void Start()
    {
        NetworkHelper.instance.players.Add(GetComponent<NetworkObject>());
    }
    public void PerformAttack()
    {
        var origin = transform.position;
        var direction = transform.forward;
        Collider[] colisoresEncontrados = Physics.OverlapSphere(origin + direction + transform.up, raioDaEsfera);

        // 2. Passa por cada colisor detectado
        foreach (Collider colisor in colisoresEncontrados)
        {
            Debug.Log(colisor.name);
            // 3. Verifica se o objeto detectado possui a tag específica
            if (colisor.tag == oponentTag && colisor.gameObject != this.gameObject)
            {
                NetworkObject targeNetWorkObject = colisor.GetComponent<NetworkObject>();
                if(targeNetWorkObject != null && canAttack)
                {
                    Debug.Log("Objeto detectado com a tag: " + colisor.gameObject.name);
                    NetworkHelper.instance.RequestAttackServerRpc(NetworkManager.Singleton.LocalClientId, targeNetWorkObject.OwnerClientId);
                    canAttack = false;
                    Invoke("RefreshAttack", .5f);
                }
                // Faça a sua ação aqui (ex: causar dano, ativar animação)
            }
        }
    }

    private void RefreshAttack()
    {
        canAttack = true;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        var origin = transform.position;
        var direction = transform.forward;
        Gizmos.DrawWireSphere(origin + direction + transform.up, raioDaEsfera);
    }

    private void OnDisable()
    {
        NetworkHelper.instance.players.Remove(GetComponent<NetworkObject>());
    }
}
