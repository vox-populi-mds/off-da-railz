using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{	
	public Color[] m_colors;
	
	public Transform[] m_SpawnLocations = new Transform[16];
	
	private Stack<Transform> m_SpawnStack = new Stack<Transform>();
	
	float m_lastPlayerUpdateTime;
	
	void Awake()
	{
		// Enable cursor visibility
		Screen.showCursor = true;
		m_lastPlayerUpdateTime = Time.timeSinceLevelLoad;
		
		foreach(Transform t in m_SpawnLocations)
		{
			m_SpawnStack.Push(t);
		}
	}
	
	public void GO()
	{
		networkView.RPC("OnGO", RPCMode.Others);
		OnGO();
	}
	
	public void Host()
	{
		Transform SpawnLocation = m_SpawnStack.Pop();
		Players.Get().GetMe().Color = m_colors[0];
		Players.Get().GetMe().SpawnPostion = SpawnLocation.position;
		Players.Get().GetMe().SpawnRotation = SpawnLocation.rotation;
		Session.Get().Host();
	}
	
	[RPC]
	void OnCreatePlayer(string ipAddress, int port)
	{
		if (port == 0)
		{
			return;
		}
		
		if (Players.Get().Get(ipAddress, port) == null)
		{
			Player player = new Player();
			player.IPAddress = ipAddress;
			player.Port = port;
			Players.Get().Add(player);
		}
	}
	
	[RPC]
	void OnGO()
	{
		Session.Get().SetRoundCount(3);
		Session.Get().StartGame();
		Session.Get().StartRound();
	}
	
	void OnPlayerConnected(NetworkPlayer networkPlayer)
	{
		Transform SpawnLocation = m_SpawnStack.Pop();
		Player player = new Player();
		player.Apply(networkPlayer);
		player.Color = m_colors[Players.Get().GetAll().Count];
		player.SpawnPostion = SpawnLocation.position;
		player.SpawnRotation = SpawnLocation.rotation;
		Players.Get().Add(player);
		
		networkView.RPC("OnRequestPlayerName", networkPlayer);
	}
	
	void OnPlayerDisconnected(NetworkPlayer networkPlayer)
	{
		Transform SpawnLocation = new GameObject().transform;
		
		SpawnLocation.position = Players.Get().Get(networkPlayer).SpawnPostion;
		SpawnLocation.rotation = Players.Get().Get(networkPlayer).SpawnRotation;
		
		m_SpawnStack.Push(SpawnLocation);
	}
	
	[RPC]
	void OnPlayerReady(NetworkMessageInfo info)
	{
		Players.Get().Get(info.sender).Ready = true;
	}
	
	[RPC]
	void OnRequestPlayerName()
	{
		networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player.ipAddress, Network.player.port,
			Players.Get().GetMe().Name);
	}
	
	[RPC]
	void OnUpdatePlayerColor(string ipAddress, int port, float r, float g, float b, float a)
	{
		// The player itself might not have been seen yet.
		if (Players.Get().Get(ipAddress, port) != null)
		{
			Players.Get().Get(ipAddress, port).Color = new Color(r, g, b, a);
		}
	}
	
	[RPC]
	void OnUpdatePlayerName(string ipAddress, int port, string playerName)
	{
		// The player itself might not have been seen yet.
		if (Players.Get().Get(ipAddress, port) != null)
		{
			Players.Get().Get(ipAddress, port).Name = playerName;
		}
	}
	
	[RPC]
	void OnUpdatePlayerSpawn(string ipAddress, int port, Vector3 spawnPosition, Quaternion spawnRotation)
	{
		// The player itself might not have been seen yet.
		if (Players.Get().Get(ipAddress, port) != null)
		{
			Players.Get().Get(ipAddress, port).SpawnPostion = spawnPosition;
			Players.Get().Get(ipAddress, port).SpawnRotation = spawnRotation;
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
		// Make sure my player is up to date.
		Player me = Players.Get().GetMe();
		me.Apply(Network.player);
		
		if (Network.isServer && Time.timeSinceLevelLoad - m_lastPlayerUpdateTime > 1.0f)
		{
			m_lastPlayerUpdateTime = Time.timeSinceLevelLoad;
			
			foreach (Player player in Players.Get().GetAll())
			{
				networkView.RPC("OnCreatePlayer", RPCMode.Others, player.IPAddress, player.Port);
				networkView.RPC("OnUpdatePlayerColor", RPCMode.Others, player.IPAddress, player.Port, player.Color.r,
					player.Color.g, player.Color.b, player.Color.a);
				networkView.RPC("OnUpdatePlayerName", RPCMode.Others, player.IPAddress, player.Port, player.Name);
				networkView.RPC("OnUpdatePlayerSpawn", RPCMode.All, player.IPAddress, player.Port, player.SpawnPostion, player.SpawnRotation);
			}
		}
	}
	
	public void UpdatePlayerColor(Color color)
	{
		Players.Get().GetMe().Color = color;

		if (Network.isClient)
		{
			networkView.RPC("OnUpdatePlayerColor", RPCMode.Server, Network.player.ipAddress, Network.player.port,
				color.r, color.g, color.b, color.a);
		}
	}
	
	public void UpdatePlayerName(string name)
	{
		Players.Get().GetMe().Name = name;
		PlayerPrefs.SetString("PlayerName", name);
		
		if (Network.isClient)
		{
			networkView.RPC("OnUpdatePlayerName", RPCMode.Server, Network.player.ipAddress, Network.player.port, name);
		}
	}
}
