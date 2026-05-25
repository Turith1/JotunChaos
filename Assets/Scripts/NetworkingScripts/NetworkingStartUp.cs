using UnityEngine;
using Unity.Netcode;

public class NetworkingStartUp : MonoBehaviour
{
    private async void Start()
    {
        // HOST should NEVER call JoinRelay()
        if (LobbyManager.Instance.IsHost)
            return;

        var lobby = LobbyManager.Instance.currentLobby;

        if (lobby.Data.TryGetValue("relayCode", out var relayData))
        {
            string relayCode = relayData.Value;

            await RelayManager.Instance.JoinRelay(relayCode);
        }
    }
}
