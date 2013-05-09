using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{	
	private class NetworkPlayerComparer : IEqualityComparer<NetworkPlayer>
	{
		public bool Equals(NetworkPlayer a, NetworkPlayer b)
		{
			return a.ipAddress == b.ipAddress && a.port == b.port;
		}
		
		public int GetHashCode(NetworkPlayer player)
		{
			int hashCode = 0;
			
			if (player.ipAddress.Length != 0)
			{
				string[] ipComponents = player.ipAddress.Split('.');
				foreach (string ipComponent in ipComponents)
				{
					hashCode += Convert.ToInt32(ipComponent);
				}
			}
			hashCode += player.port;
			
			return hashCode;
		}
	}
	
	bool m_connected;
	
	static string GAME_TYPE = "VoxPopuli::OffDaRails";
	
	float m_lastPlayerNameUpdateTime;
	
	string m_playerName;
	
	bool m_playerReady;
	
	Dictionary<NetworkPlayer, string> m_playerNames;
	
	Dictionary<NetworkPlayer, bool> m_playerReadyStates;
	
	static int PORT = 25002;
	
	System.Random m_random;
	
	string m_serverDescription;
	
	string m_serverName;
	
	public Transform m_train;
	
	void Awake()
	{
		MasterServer.RequestHostList(GAME_TYPE);
	}
	
	[RPC]
	void OnGO()
	{
		Application.LoadLevel("Level0");
	}
	
	void OnGUI()
	{
		GUI.Box(new Rect(10.0f, 15.0f, 300.0f, 30.0f), "");
		GUILayout.BeginArea(new Rect(15.0f, 20.0f, 290.0f, 25.0f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Your Name");
		string tempPlayerName = GUILayout.TextField(m_playerName);
		if (tempPlayerName != m_playerName)
		{
			m_playerName = tempPlayerName;
			if (m_connected)
			{
				if (Network.isServer)
				{
					OnUpdatePlayerName(m_playerName, Network.player);
				}
				else
				{
					networkView.RPC("OnUpdatePlayerName", RPCMode.Server, m_playerName, Network.player);
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		GUI.Box(new Rect(10.0f, 55.0f, 600.0f, 30.0f), "");
		GUILayout.BeginArea(new Rect(15.0f, 60.0f, 590.0f, 25.0f));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Server Name");
		m_serverName = GUILayout.TextField(m_serverName);
		GUILayout.Label("Description");
		m_serverDescription = GUILayout.TextField(m_serverDescription, 50);
		if (!m_connected)
		{
			if (GUILayout.Button("Start Server"))
			{
				// Use NAT punchthrough if no public IP present
				Network.InitializeServer(32, PORT, !Network.HavePublicAddress());
				MasterServer.RegisterHost(GAME_TYPE, m_serverName, m_serverDescription);
				OnUpdatePlayerName(m_playerName, Network.player);
				m_playerReadyStates[Network.player] = false;
				//Network.Instantiate(m_train, new Vector3(0.0f, 15.0f, 30.0f), new Quaternion(0.0f, m_random.Next(0, 7), 0.0f, 1.0f), 0);
				m_connected = true;
			}
		}
		else if (Network.isServer)
		{
			if (GUILayout.Button("Stop Server"))
			{
				Network.Disconnect();
				MasterServer.UnregisterHost();
				MasterServer.RequestHostList(GAME_TYPE);
				m_connected = false;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		GUI.Box(new Rect(10.0f, 95.0f, 830.0f, 240.0f), "");
		GUILayout.BeginArea(new Rect(15.0f, 100.0f, 820.0f, 235.0f));
		if (GUILayout.Button("Refresh Server List"))
		{
			MasterServer.RequestHostList(GAME_TYPE);
		}
		
		HostData[] hosts = MasterServer.PollHostList();
		foreach (HostData host in hosts)
		{
			GUILayout.BeginHorizontal();
			string name = host.gameName + " " + host.connectedPlayers + " / " + host.playerLimit;
			GUILayout.Label(name);
			GUILayout.Space(5);
			string hostInfo;
			hostInfo = "[";
			foreach (string ip in host.ip)
			{
				hostInfo = hostInfo + ip + ":" + host.port + " ";
			}
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);
			GUILayout.Space(5);
			GUILayout.Label(host.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			if (!Network.isServer)
			{
				if (m_connected)
				{
					if (GUILayout.Button("Disconnect"))
					{
						Network.Disconnect();
						m_playerNames.Clear();
						m_connected = false;
					}
				}
				else
				{
					if (GUILayout.Button("Connect"))
					{
						// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
						if (Network.Connect(host) == NetworkConnectionError.NoError)
						{
							m_playerReadyStates[Network.player] = false;
							//Network.Instantiate(m_train, new Vector3(0.0f, 15.0f, 30.0f), new Quaternion(0.0f, m_random.Next(0, 7), 0.0f, 1.0f), 0);
							m_connected = true;
						}
						else
						{
							MasterServer.RequestHostList(GAME_TYPE);
						}
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		GUI.Box(new Rect(10.0f, 345.0f, 830.0f, 240.0f), "");
		GUILayout.BeginArea(new Rect(15.0f, 350.0f, 820.0f, 235.0f));
		if (m_connected)
		{			
			foreach (string otherPlayerName in m_playerNames.Values)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(otherPlayerName);
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
		
		GUI.Box(new Rect(640.0f, 595.0f, 200.0f, 30.0f), "");
		GUILayout.BeginArea(new Rect(645.0f, 600.0f, 185.0f, 25.0f));
		GUILayout.BeginHorizontal();
		if (m_connected)
		{
			if (m_playerReady)
			{
				GUILayout.Label("Ready!");
			}
			else
			{
				if (GUILayout.Button("Ready!"))
				{
					m_playerReady = true;
	
					if (Network.isServer)
					{
						m_playerReadyStates[Network.player] = true;
					}
					else
					{
						networkView.RPC("OnPlayerReady", RPCMode.Server);
					}
				}
			}
			if (Network.isServer && !m_playerReadyStates.ContainsValue(false))
			{
				if (GUILayout.Button("GO!"))
				{
					networkView.RPC("OnGO", RPCMode.Others);
					OnGO();
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		m_playerReadyStates[player] = false;
		networkView.RPC("OnRequestPlayerName", player);
	}
	
	[RPC]
	void OnPlayerReady(NetworkMessageInfo info)
	{
		m_playerReadyStates[info.sender] = true;
	}
	
	[RPC]
	void OnRequestPlayerName()
	{
		networkView.RPC("OnUpdatePlayerName", RPCMode.Server, m_playerName, Network.player);
	}
	
	[RPC]
	void OnUpdatePlayerName(string playerName, NetworkPlayer player)
	{
		if (player.ipAddress.Length != 0)
		{
			m_playerNames[player] = playerName;
		}
	}
	
	void Start()
	{
		Application.runInBackground = true;
		
		m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
		m_playerName = "John Doe";
		m_playerNames = new Dictionary<NetworkPlayer, string>(new NetworkPlayerComparer());
		m_playerReady = false;
		m_playerReadyStates = new Dictionary<NetworkPlayer, bool>(new NetworkPlayerComparer());
		m_random = new System.Random();
		m_serverDescription = "Thy train shall be wreckethed.";
		m_serverName = "Train wreck!";
	}
	
	void Update()
	{
		if (Network.isServer && Time.timeSinceLevelLoad - m_lastPlayerNameUpdateTime > 1.0f)
		{
			m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
			
			foreach (KeyValuePair<NetworkPlayer, string> otherPlayerName in m_playerNames)
			{
				networkView.RPC("OnUpdatePlayerName", RPCMode.Others, otherPlayerName.Value, otherPlayerName.Key);
			}
		}
	}
}
