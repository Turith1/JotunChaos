using UnityEngine;
using TMPro;

public class LobbyInstance : MonoBehaviour
{
    public string LobbyName;
    public int PlayerCount;
    public int PlayersLogged;
    public int _lobbyIndex;
    public LobbyManager LobbyManager;

    [SerializeField]
    private TextMeshProUGUI _nameText;
    [SerializeField]
    private TextMeshProUGUI _playerCountText;

    private void Start()
    {
        _nameText.text = LobbyName;
        _playerCountText.text = PlayerCount + "/5";
    }
}
