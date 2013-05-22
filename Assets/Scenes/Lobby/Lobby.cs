using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{	
	public Color[] m_colors = new Color[16];
	
	float m_lastPlayerUpdateTime;
	
	void Awake()
	{
		// Enable cursor visibility
		Screen.showCursor = true;
		m_lastPlayerUpdateTime = Time.timeSinceLevelLoad;
	}
	
	public void GO()
	{
		networkView.RPC("OnGO", RPCMode.Others);
		OnGO();
	}
	
	public void Host()
	{
		Players.Get().GetMe().Color = m_colors[0];
		Session.Get().Host();
	}
	
	[RPC]
	void OnGO()
	{
		Session.Get().StartGame();
		Session.Get().SetRoundCount(3);
		Application.LoadLevel("Game");
	}
	
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		Player player = new Player();
		player.Color = m_colors[Players.Get().GetAll().Count];
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
	void OnUpdatePlayerColor(NetworkPlayer networkPlayer, float r, float g, float b, float a)
	{
		// For clients the player itself might not have been seen yet.
		if (Players.Get().Get(networkPlayer) != null)
		{
			Players.Get().Get(networkPlayer).Color = new Color(r, g, b, a);
		}
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
		if (Network.isServer && Time.timeSinceLevelLoad - m_lastPlayerUpdateTime > 1.0f)
		{
			m_lastPlayerUpdateTime = Time.timeSinceLevelLoad;
			
			foreach (Player player in Players.Get().GetAll())
			{
				networkView.RPC("OnUpdatePlayerColor", RPCMode.Others, player.NetworkPlayer, player.Color.r,
					player.Color.g, player.Color.b, player.Color.a);
				networkView.RPC("OnUpdatePlayerName", RPCMode.Others, player.NetworkPlayer, player.Name);
			}
		}
	}
	
	public void UpdatePlayerColor(Color color)
	{
		Players.Get().GetMe().Color = color;

		if (Network.isClient)
		{
			networkView.RPC("OnUpdatePlayerColor", RPCMode.Server, Network.player, color.r, color.g, color.b, color.a);
		}
	}
	
	public void UpdatePlayerName(string name)
	{
		Players.Get().GetMe().Name = name;

		if (Network.isClient)
		{
			networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player, name);
		}
	}
}
