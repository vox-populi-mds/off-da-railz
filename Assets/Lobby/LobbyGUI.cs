using UnityEngine;

public class LobbyGUI : MonoBehaviour
{
	float m_abovePlayerListBoxHeight;
	
	float m_aboveServerBoxHeight;
	
	float m_aboveServerListBoxHeight;
	
	float m_boxPadding;
	
	float m_gapSize;
	
	float m_listBoxHeight;
	
	float m_listBoxWidth;
	
	float m_listBoxWidthInternal;
	
	float m_oneLineBoxHeight;
	
	string m_serverDescription;
	
	string m_serverName;
	
	void Awake()
	{
		m_boxPadding = 5.0f;
		m_gapSize = 10.0f;
		m_oneLineBoxHeight = 31.0f;
		m_serverDescription = "Thy train shall be wreckethed.";
		m_serverName = "Train wreck!";
		
		m_aboveServerBoxHeight = m_gapSize * 2.0f + m_oneLineBoxHeight;
		m_aboveServerListBoxHeight = m_gapSize * 3.0f + m_oneLineBoxHeight * 2.0f;
	}
	
	void DrawPlayerListBox()
	{
		GUI.Box(new Rect(m_gapSize, m_abovePlayerListBoxHeight, m_listBoxWidth, m_listBoxHeight), "");
		GUILayout.BeginArea(new Rect(m_gapSize + m_boxPadding, m_abovePlayerListBoxHeight + m_boxPadding, m_listBoxWidthInternal, m_listBoxHeight - 2.0f * m_boxPadding));
		if (GetComponent<Lobby>().IsConnected())
		{			
			foreach (Player player in Players.Get().GetAll())
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(player.Name);
				GUILayout.EndHorizontal();
			}
		}
		GUILayout.EndArea();
	}
	
	void DrawPlayerNameBox()
	{
		GUI.Box(new Rect(m_gapSize, m_gapSize, 300.0f, m_oneLineBoxHeight), "");
		GUILayout.BeginArea(new Rect(m_gapSize + m_boxPadding, m_gapSize + m_boxPadding, 300.0f - 2.0f * m_boxPadding, m_oneLineBoxHeight - 2.0f * m_boxPadding));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Your Name");
		string tempPlayerName = GUILayout.TextField(Players.Get().GetMe().Name);
		if (tempPlayerName != Players.Get().GetMe().Name)
		{
			GetComponent<Lobby>().UpdatePlayerName(tempPlayerName);
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void DrawReadyGoBox()
	{
		GUI.Box(new Rect(Screen.width - 200.0f - m_gapSize, Screen.height - m_oneLineBoxHeight - m_gapSize, 200.0f, m_oneLineBoxHeight), "");
		GUILayout.BeginArea(new Rect(Screen.width - 200.0f - m_gapSize + m_boxPadding, Screen.height - m_oneLineBoxHeight - m_gapSize + m_boxPadding, 200.0f - 2.0f * m_boxPadding, m_oneLineBoxHeight - 2.0f * m_boxPadding));
		GUILayout.BeginHorizontal();
		if (GetComponent<Lobby>().IsConnected())
		{
			if (Players.Get().GetMe().Ready)
			{
				GUILayout.Label("Ready!");
			}
			else if (GUILayout.Button("Ready!"))
			{
				GetComponent<Lobby>().PlayerReady();
			}
			
			if (Network.isServer && Players.Get().AllReady())
			{
				GetComponent<Lobby>().GO();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();	
	}
	
	void DrawServerBox()
	{
		GUI.Box(new Rect(m_gapSize, m_aboveServerBoxHeight, 600.0f, m_oneLineBoxHeight), "");
		GUILayout.BeginArea(new Rect(m_gapSize + m_boxPadding, m_aboveServerBoxHeight + m_boxPadding, 600.0f - 2.0f * m_boxPadding, m_oneLineBoxHeight - 2.0f * m_boxPadding));
		GUILayout.BeginHorizontal();
		GUILayout.Label("Server Name");
		m_serverName = GUILayout.TextField(m_serverName);
		GUILayout.Label("Description");
		m_serverDescription = GUILayout.TextField(m_serverDescription, 50);
		if (!GetComponent<Lobby>().IsConnected())
		{
			if (GUILayout.Button("Start Server"))
			{
				GetComponent<Lobby>().StartServer(m_serverName, m_serverDescription);
			}
		}
		else if (Network.isServer)
		{
			if (GUILayout.Button("Stop Server"))
			{
				GetComponent<Lobby>().StopServer();
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void DrawServerListBox()
	{
		GUI.Box(new Rect(m_gapSize, m_aboveServerListBoxHeight, Screen.width - 20.0f, m_listBoxHeight), "");
		GUILayout.BeginArea(new Rect(m_gapSize + m_boxPadding, m_aboveServerListBoxHeight + m_boxPadding, Screen.width - 30.0f, m_listBoxHeight - 5.0f));
		if (GUILayout.Button("Refresh Server List"))
		{
			GetComponent<Lobby>().UpdateHostList();
		}
		
		HostData[] hosts = GetComponent<Lobby>().GetHostList();
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
				// If we are connected to a server, there will be 1 connection.
				if (Network.connections.Length == 1)
				{
					// If this host is the server we are currently connected to.
					if (Network.connections[0].ipAddress == host.ip[0])
					{
						if (GUILayout.Button("Disconnect"))
						{
							GetComponent<Lobby>().Disconnect();
						}
					}
				}
				else
				{
					if (GUILayout.Button("Connect"))
					{
						GetComponent<Lobby>().Connect(host);
					}
				}
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
	}
	
	void OnGUI()
	{
		float gapsHeight = m_gapSize * 6.0f;
		float oneLineBoxesHeight = m_oneLineBoxHeight * 3.0f;
		m_listBoxHeight = (Screen.height - oneLineBoxesHeight - gapsHeight) / 2.0f;
		m_listBoxWidth = Screen.width - 2.0f * m_gapSize;
		m_listBoxWidthInternal = m_listBoxWidth - 2.0f * m_boxPadding;
		m_abovePlayerListBoxHeight = m_gapSize * 4.0f + m_oneLineBoxHeight * 2.0f + m_listBoxHeight;

		DrawPlayerListBox();
		DrawPlayerNameBox();
		DrawReadyGoBox();
		DrawServerBox();
		DrawServerListBox();
	}
}
