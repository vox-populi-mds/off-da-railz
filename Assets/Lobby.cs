using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lobby : MonoBehaviour
{
	bool m_connected;
	
	static string GAME_TYPE = "VoxPopuli::OffDaRails";
	
	float m_lastPlayerNameUpdateTime;
	
	string m_playerName;
	
	Dictionary<NetworkPlayer, string> m_playerNames;
	
	static int PORT = 25002;
	
	string m_serverDescription;
	
	string m_serverName;
	
	void Awake()
	{
		MasterServer.RequestHostList(GAME_TYPE);
	}
	
	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player Name:");
		string tempPlayerName = GUILayout.TextField(m_playerName);
		if (tempPlayerName != m_playerName)
		{
			m_playerName = tempPlayerName;
			if (m_connected)
			{
				networkView.RPC("OnUpdatePlayerName", RPCMode.Server, m_playerName, networkView.owner);
			}
		}
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Server Name:");
		m_serverName = GUILayout.TextField(m_serverName);
		GUILayout.Label("Server Description:");
		m_serverDescription = GUILayout.TextField(m_serverDescription, 50);
		if (!m_connected)
		{
			if (GUILayout.Button("Start Server"))
			{
				// Use NAT punchthrough if no public IP present
				Network.InitializeServer(32, PORT, !Network.HavePublicAddress());
				MasterServer.RegisterHost(GAME_TYPE, m_serverName, m_serverDescription);
				m_connected = true;
			}
		}
		else if (Network.isServer)
		{
			if (GUILayout.Button("Stop Server"))
			{
				Network.Disconnect();
				MasterServer.UnregisterHost();
				m_connected = false;
			}
		}
		GUILayout.EndHorizontal();
		
		if (!Network.isServer)
		{
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
				if (m_connected)
				{
					if (GUILayout.Button("Disconnect"))
					{
						Network.Disconnect();
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
							m_connected = true;
						}
						else
						{
							Debug.Log("Failed to connect!");
						}
					}
				}
				GUILayout.EndHorizontal();
			}
		}
		
		if (m_connected)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label("Players:");
			GUILayout.EndHorizontal();
			foreach (string otherPlayerName in m_playerNames.Values)
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(otherPlayerName);
				GUILayout.EndHorizontal();
			}
		}
	}
	
	void OnPlayerConnected(NetworkPlayer player)
	{
		networkView.RPC("OnRequestPlayerName", player);
	}
	
	[RPC]
	void OnRequestPlayerName()
	{
		networkView.RPC("OnUpdatePlayerName", RPCMode.Server, m_playerName, networkView.owner);
	}
	
	[RPC]
	void OnUpdatePlayerName(string playerName, NetworkPlayer player)
	{
		m_playerNames[player] = playerName;
	}
	
	void Start()
	{
		Application.runInBackground = true;
		
		m_lastPlayerNameUpdateTime = Time.timeSinceLevelLoad;
		m_playerName = "John Doe";
		m_playerNames = new Dictionary<NetworkPlayer, string>();
		m_playerNames[networkView.owner] = m_playerName;
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
