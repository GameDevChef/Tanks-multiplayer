using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Prototype.NetworkLobby;
using UnityEngine.Networking;

public class NetworkLobbyHook1 : LobbyHook {

    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobbyP = lobbyPlayer.GetComponent<LobbyPlayer>();

        PlayerSetup playerSetup = gamePlayer.GetComponent<PlayerSetup>();

        playerSetup.m_PlayerName = lobbyP.playerName;

        //playerSetup.m_PlayerColor = lobbyP.playerColor;

        PlayerManager playerManager = gamePlayer.GetComponent<PlayerManager>();

        if (playerManager != null)
        {
            GameManager.m_AllPlayersList.Add(playerManager);
        }
    }
}
