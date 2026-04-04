using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using StarterAssets;
using Unity.Cinemachine;
using Unity.VisualScripting;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
    [SerializeField] private ThirdPersonController m_ThirdPersonController;
    [SerializeField] private CinemachineVirtualCamera m_VC;

    private void Awake()
    {
        m_StarterAssetsInputs.enabled = false;
        m_PlayerInput.enabled = false;
        m_ThirdPersonController.enabled = false;
        GameObject Camera = GameObject.FindGameObjectWithTag("CameraPlayer");
        m_VC = Camera.GetComponent<CinemachineVirtualCamera>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            m_VC.Follow = this.gameObject.transform;
            m_StarterAssetsInputs.enabled = true;
            m_PlayerInput.enabled = true;
            m_ThirdPersonController.enabled = true;
        }
    }
}
