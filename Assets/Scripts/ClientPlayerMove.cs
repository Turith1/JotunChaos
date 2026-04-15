using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using StarterAssets;
using Unity.Cinemachine;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
    [SerializeField] private ThirdPersonController m_ThirdPersonController;
    [SerializeField] private GameObject m_cineCam;
    [SerializeField] private GameObject _mainCamera;

    private void Awake()
    {
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_PlayerInput.enabled = true;
            m_StarterAssetsInputs.enabled = true;
            m_cineCam.SetActive(true);
            m_ThirdPersonController.enabled = true;
        }
        else if(!IsOwner && !IsServer)
        {
            m_PlayerInput.enabled = false;
            m_StarterAssetsInputs.enabled = false;
        }
    }

    [ServerRpc]
    public void UpdateInputServerRpc(Vector2 move, Vector2 look, bool jump, bool sprint, float camRotation)
    {
        m_ThirdPersonController.SetServerInput(move, look, jump, sprint, camRotation);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        UpdateInputServerRpc(m_StarterAssetsInputs.move, m_StarterAssetsInputs.look, m_StarterAssetsInputs.jump, m_StarterAssetsInputs.sprint, _mainCamera.transform.eulerAngles.y);
    }
}
