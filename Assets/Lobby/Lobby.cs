using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{	
	bool m_connected;
	
	static string GAME_TYPE = "VoxPopuli::OffDaRailz";
	
	float m_lastPlayerNameUpdateTime;
	
	Player m_me;
	
	Players m_players;
	
	public Transform m_playersPrefab;
	
	static int PORT = 25002;
	
	void Awake()
	{
		Application.runInBackground = true;
		
		// Enable cursor visibility
		Screen.showCursor = true;
		
		MasterServer.RequestHostList(GAME_TYPE);
		
		m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
		m_me = new Player();
		m_me.Me = true;
		m_me.Name = "John Doe";
		m_me.Ready = false;
		m_players = Players.Get();
		m_players.Add(m_me);
		m_me.NetworkPlayer = Network.player;
	}
	
	public void Connect(HostData host)
	{
		// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
		if (Network.Connect(host) == NetworkConnectionError.NoError)
		{
			m_me.Ready = false;
			m_connected = true;
		}
		else // The host list is probably out of date.
		{
			MasterServer.RequestHostList(GAME_TYPE);
		}
	}
	
	public void Disconnect()
	{
		Network.Disconnect();
		m_players.RemoveOthers();
		m_connected = false;
	}
	
	public HostData[] GetHostList()
	{
		return MasterServer.PollHostList();	
	}
	
	public void GO()
	{
		networkView.RPC("OnGO", RPCMode.Others);
		OnGO();
	}
	
	public bool IsConnected()
	{
		return m_connected;	
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
		m_players.Add(player);
		networkView.RPC("OnRequestPlayerName", networkPlayer);
	}
	
	void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		m_players.Remove(networkPlayer);
	}
	
	[RPC]
	void OnPlayerReady(NetworkMessageInfo info)
	{
		m_players.Get(info.sender).Ready = true;
	}
	
	[RPC]
	void OnRequestPlayerName()
	{
		networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player, m_me.Name);
	}
	
	[RPC]
	void OnUpdatePlayerName(NetworkPlayer networkPlayer, string playerName)
	{
		Player firstPlayer = m_players.GetAll().ToArray()[0];
		if (firstPlayer.NetworkPlayer.port == 0)
		{
			firstPlayer.NetworkPlayer = Network.player;
		}
		
		// For clients this may be the first time a player has been seen.
		if (m_players.Get(networkPlayer) == null)
		{
			Player player = new Player();
			player.Name = playerName;
			player.NetworkPlayer = networkPlayer;
			m_players.Add(player);
		}
		else
		{
			m_players.Get(networkPlayer).Name = playerName;
		}
	}
	
	public void PlayerReady()
	{
		m_me.Ready = true;

		if (Network.isServer)
		{
			m_me.Ready = true;
		}
		else
		{
			networkView.RPC("OnPlayerReady", RPCMode.Server);
		}	
	}
	
	void Start()
	{
	}
	
	public void StartServer(string name, string description)
	{
		// Use NAT punchthrough if no public IP present
		Network.InitializeServer(32, Lobby.PORT, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GAME_TYPE, name, description);
		m_me.NetworkPlayer = Network.player;
		m_me.Ready = false;
		m_connected = true;
	}
	
	public void StopServer()
	{
		Network.Disconnect();
		MasterServer.UnregisterHost();
		MasterServer.RequestHostList(GAME_TYPE);
		m_connected = false;
	}
	
	void Update()
	{
		if (Network.isServer && Time.timeSinceLevelLoad - m_lastPlayerNameUpdateTime > 1.0f)
		{
			m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
			
			foreach (Player player in m_players.GetAll())
			{
				networkView.RPC("OnUpdatePlayerName", RPCMode.Others, player.NetworkPlayer, player.Name);
			}
		}
	}
	
	public void UpdateHostList()
	{
		MasterServer.RequestHostList(GAME_TYPE);
	}
	
	public void UpdatePlayerName(string playerName)
	{
		m_me.Name = playerName;

		if (!Network.isServer && m_connected)
		{
			networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player, playerName);
		}
	}
}
