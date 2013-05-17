using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{
	float m_lastPlayerNameUpdateTime;
	
	void Awake()
	{
		// Enable cursor visibility
		Screen.showCursor = true;
		
		m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
	}
	
	public void GO()
	{
		networkView.RPC("OnGO", RPCMode.Others);
		OnGO();
	}
	
	[RPC]
	void OnGO()
	{
		Session.Get().StartGame();
		Session.Get().SetRoundCount(3);
		Application.LoadLevel("Level0");
	}
	
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		Player player = new Player();
		player.NetworkPlayer = networkPlayer;
		Players.Get().Add(player);
		networkView.RPC("OnRequestPlayerName", networkPlayer);
	}
	
	void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		Players.Get().Remove(networkPlayer);
	}
	
	[RPC]
	void OnPlayerReady(NetworkMessageInfo info)
	{
		Players.Get().Get(info.sender).Ready = true;
	}
	
	[RPC]
	void OnRequestPlayerName()
	{
		networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player, Players.Get().GetMe().Name);
	}
	
	[RPC]
	void OnUpdatePlayerName(NetworkPlayer networkPlayer, string playerName)
	{
		// If the server's network player has not been updated, update it.
		Player me = Players.Get().GetMe();
		if (me.NetworkPlayer.port == 0)
		{
			me.NetworkPlayer = Network.player;
		}
		
		// For clients this may be the first time a player has been seen.
		if (Players.Get().Get(networkPlayer) == null)
		{
			Player player = new Player();
			player.Name = playerName;
			player.NetworkPlayer = networkPlayer;
			Players.Get().Add(player);
		}
		else
		{
			Players.Get().Get(networkPlayer).Name = playerName;
		}
	}
	
	public void PlayerReady()
	{
		Players.Get().GetMe().Ready = true;

		if (Network.isClient)
		{
			networkView.RPC("OnPlayerReady", RPCMode.Server);
		}	
	}
	
	void Start()
	{
		if (Session.Get().LeftGame())
		{
			Session.Get().Disconnect();
		}
	}
	
	void Update()
	{
		if (Network.isServer && Time.timeSinceLevelLoad - m_lastPlayerNameUpdateTime > 1.0f)
		{
			m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
			
			foreach (Player player in Players.Get().GetAll())
			{
				networkView.RPC("OnUpdatePlayerName", RPCMode.Others, player.NetworkPlayer, player.Name);
			}
		}
	}
	
	public void UpdatePlayerName(string playerName)
	{
		Players.Get().GetMe().Name = playerName;

		if (Network.isClient)
		{
			networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player, playerName);
		}
	}
}
