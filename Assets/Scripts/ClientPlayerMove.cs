using UnityEngine;
using Unity.Netcode;
using UnityEngine.InputSystem;
using StarterAssets;
using Unity.Cinemachine;

public class ClientPlayerMove : NetworkBehaviour
{
    [SerializeField] private PlayerInput m_PlayerInput;
    [SerializeField] private StarterAssetsInputs m_StarterAssetsInputs;
    [SerializeField] private ThirdPersonControls m_ThirdPersonController;
    [SerializeField] private GameObject m_cineCam;
    [SerializeField] private GameObject _mainCamera;

    private Vector2 _lastMove;
    private Vector2 _lastLook;
    private bool _lastJump;
    private bool _lastSprint;
    private float _lastCamRot;
    private bool _lastAttack;

    private void Awake()
    {
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (IsOwner)
        {
            _mainCamera = Camera.main.gameObject;

            m_cineCam.SetActive(true);

            m_PlayerInput.enabled = true;
            m_StarterAssetsInputs.enabled = true;
            m_ThirdPersonController.enabled = true;
        }
        else
        {
            m_cineCam.SetActive(false);
            m_PlayerInput.enabled = false;
            m_StarterAssetsInputs.enabled = false;
            //m_ThirdPersonController.enabled = false;
        }
    }

    [ServerRpc]
    public void UpdateInputServerRpc(Vector2 move, Vector2 look, bool jump, bool sprint, float camRotation, bool attack)
    {
        m_ThirdPersonController.SetServerInput(move, look, jump, sprint, camRotation, attack);
    }

    private void Update()
    {
        if (!IsOwner)
            return;

        if (_mainCamera == null)
        {
            _mainCamera = Camera.main?.gameObject;

            if (_mainCamera == null)
                return;
        }

        Vector2 move = m_StarterAssetsInputs.move;
        Vector2 look = m_StarterAssetsInputs.look;
        bool jump = m_StarterAssetsInputs.jump;
        bool sprint = m_StarterAssetsInputs.sprint;
        float camRot = _mainCamera.transform.eulerAngles.y;
        bool attack = m_StarterAssetsInputs.attack;

        bool changed =
            move != _lastMove ||
            look != _lastLook ||
            jump ||
            sprint ||
            attack ||
            Mathf.Abs(camRot - _lastCamRot) > 0.1f;

        if (!changed)
            return;

        _lastMove = move;
        _lastLook = look;
        _lastJump = jump;
        _lastSprint = sprint;
        _lastCamRot = camRot;
        _lastAttack = attack;

        UpdateInputServerRpc(
            move,
            look,
            jump,
            sprint,
            camRot,
            attack
        );

        if (attack)
            m_StarterAssetsInputs.attack = false;
    }
}
