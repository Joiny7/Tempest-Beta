using UnityEngine;
using UnityEngine.Networking;
using System.Collections;



namespace Prototype.NetworkLobby
{
    public abstract class LobbyHook : MonoBehaviour
    {
        public virtual void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
        {
            gamePlayer.GetComponent<Player>().PlayerNick = lobbyPlayer.GetComponent<LobbyPlayer>().playerName;
            gamePlayer.GetComponent<Player>().PlayerColor = lobbyPlayer.GetComponent<LobbyPlayer>().playerColor;
        }
    }

}
