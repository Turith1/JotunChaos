using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Multiplayer;
using Unity.Services.Lobbies.Models;
using Unity.Services.Lobbies;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;

public class LobbyManager : MonoBehaviour
{

    private Lobby _joinedLobby;
    private Lobby _hostLobby;
    private QueryResponse response;
    private int CurrentLobbyIndex;

    [SerializeField]
    private GameObject _scrollViewContent;
    [SerializeField]
    private List<LobbyInstance> _instantiatedRooms;
    [SerializeField]
    private GameObject _roomPrefab;


    [ContextMenu("Start Host")]
    public async void StartHost()
    {
        await InitializeServices();

        // 🔹 RELAY
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(4);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        RelayServerData relayData = AllocationUtils.ToRelayServerData(allocation, "dtls");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(relayData);

        // 🔹 LOBBY (guarda o joinCode)
        _hostLobby = await LobbyService.Instance.CreateLobbyAsync(
            "Sala do Host",
            8,
            new CreateLobbyOptions
            {
                Data = new Dictionary<string, DataObject>
                {
                {
                    "joinCode",
                    new DataObject(DataObject.VisibilityOptions.Public, joinCode)
                }
                }
            });

        Debug.Log("Lobby criado: " + _hostLobby.Id);
        Debug.Log("JoinCode: " + joinCode);

        NetworkManager.Singleton.StartHost();
    }

    private async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public async void JoinRoom()
    {
        Lobby lobby = response.Results[CurrentLobbyIndex];

        // 🔹 entra no lobby
        _joinedLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id);

        Debug.Log("Entrou no lobby: " + lobby.Id);

        // 🔹 pega dados
        string joinCode = lobby.Data["joinCode"].Value;

        // 🔹 RELAY
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

        RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");

        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(relayData);

        NetworkManager.Singleton.StartClient();
    }

    public async void UpdateShownLobbies()
    {
        Debug.Log("INSIDE UPDATE SHOWN LOBBIES");

        if (_instantiatedRooms != null)
        {
            var wipeList = new List<LobbyInstance>();

            foreach (var item in _instantiatedRooms)
            {
                wipeList.Add(item);
            }
            foreach (var item in wipeList)
            {
                Destroy(item.gameObject);
            }
        }
        _instantiatedRooms = new();

        await InitializeServices();

        // 🔹 Busca lobbies disponíveis
        response = await LobbyService.Instance.QueryLobbiesAsync();

        if (response.Results.Count == 0)
        {
            Debug.Log("Nenhum lobby encontrado");
            return;
        }

        //_availableLobbies.options = new();

        for (int i = 0; i < response.Results.Count; i++)
        {
            Debug.Log("RESULTS COUNT IS " + response.Results.Count);
            var room = Instantiate(_roomPrefab, _scrollViewContent.transform);
            var roomComponent = room.GetComponent<LobbyInstance>();
            roomComponent._lobbyIndex = i;
            roomComponent.LobbyName = response.Results[i].Name;
            roomComponent.PlayerCount = response.Results[i].MaxPlayers;
            roomComponent.PlayersLogged = response.Results[i].Players.Count;
            roomComponent.LobbyManager = this;
            _instantiatedRooms.Add(roomComponent);
        }
    }
}
