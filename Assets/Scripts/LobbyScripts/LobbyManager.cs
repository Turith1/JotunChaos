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

    [ContextMenu("Start Host")]
    public async void StartHost(int modeIndex)
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
                },
                {
                    "gameMode",
                    new DataObject(DataObject.VisibilityOptions.Public, modeIndex.ToString())
                }
                }
            });

        Debug.Log("Lobby criado: " + _hostLobby.Id);
        Debug.Log("JoinCode: " + joinCode);

        // UI / dados locais


        //BattleDynamicsData.CurrentOnlineBattleMode = (SkirmishMode)modeIndex;
        //BattleDynamicsData.MyHostJoinCode = joinCode;

        // opcional mostrar código na tela
        //_joinCodeOutput.text = joinCode;

        NetworkManager.Singleton.StartHost();

        //var canvas = Instantiate(_networkCanvas);
        //canvas.GetComponent<NetworkObject>().Spawn();
    }

    private async Task InitializeServices()
    {
        await UnityServices.InitializeAsync();
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    public void StartHost()
    {

    }

    /*public async void CreateLobby()
    {
        // 1. Cria alocação no Relay para até 4 pessoas
        var allocation = await RelayService.Instance.CreateAllocationAsync(4);
        string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        // 2. Configura o Netcode para usar o Relay
        //NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "dtls"));
        JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
        RelayServerData relayData = AllocationUtils.ToRelayServerData(joinAllocation, "dtls");
        var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
        transport.SetRelayServerData(relayData);

        // 3. Cria o Lobby na Unity Cloud
        var options = new CreateLobbyOptions
        {
            IsPrivate = false,
            Data = new Dictionary<string, LobbyDataObject> {
            { "JoinCode", new LobbyDataObject(LobbyDataObject.VisibilityOptions.Member, joinCode) }
        }
        };

        var lobby = await LobbyService.Instance.CreateLobbyAsync("NomeDoLobby", 4, options);

        // 4. Inicia como Host no Netcode
        NetworkManager.Singleton.StartHost();
    }*/
}
