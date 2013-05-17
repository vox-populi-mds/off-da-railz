using UnityEngine;

public class LobbyGUI : MonoBehaviour
{
	const float PLAYER_NAME_BOX_WIDTH = 600.0f;
	
	const float READY_GO_BOX_WIDTH = 200.0f;
	
	const float SERVER_BOX_WIDTH = 600.0f;
	
	float m_abovePlayerListBoxHeight;
	
	float m_aboveServerBoxHeight;
	
	float m_aboveServerListBoxHeight;
	
	public Font m_font;
	
	float m_listBoxHeight;
	
	float m_listBoxWidth;
	
	float m_listBoxWidthInternal;
	
	string m_serverDescription;
	
	string m_serverName;
	
	void Awake()
	{
		m_serverDescription = "Thy train shall be wreckethed.";
		m_serverName = "Train wreck!";
		
		m_aboveServerBoxHeight = GUIConstants.GAP_SIZE_DOUBLE + GUIConstants.ONE_LINE_BOX_HEIGHT;
		m_aboveServerListBoxHeight = GUIConstants.GAP_SIZE * 3.0f + GUIConstants.ONE_LINE_BOX_HEIGHT * 2.0f;
	}
	
	void DrawPlayerListBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_abovePlayerListBoxHeight, m_listBoxWidth, m_listBoxHeight), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, m_abovePlayerListBoxHeight +
			GUIConstants.BOX_PADDING, m_listBoxWidthInternal, m_listBoxHeight - GUIConstants.BOX_PADDING_DOUBLE));
		
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
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, GUIConstants.GAP_SIZE, PLAYER_NAME_BOX_WIDTH,
			GUIConstants.ONE_LINE_BOX_HEIGHT), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, GUIConstants.GAP_SIZE +
			GUIConstants.BOX_PADDING, PLAYER_NAME_BOX_WIDTH - GUIConstants.BOX_PADDING_DOUBLE,
			GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
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
		GUI.Box(new Rect(Screen.width - READY_GO_BOX_WIDTH - GUIConstants.GAP_SIZE, Screen.height -
			GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.GAP_SIZE, READY_GO_BOX_WIDTH,
			GUIConstants.ONE_LINE_BOX_HEIGHT), "");
		
		GUILayout.BeginArea(new Rect(Screen.width - READY_GO_BOX_WIDTH - GUIConstants.GAP_SIZE +
			GUIConstants.BOX_PADDING, Screen.height - GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.GAP_SIZE +
			GUIConstants.BOX_PADDING, READY_GO_BOX_WIDTH - GUIConstants.BOX_PADDING_DOUBLE,
			GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
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
				if (GUILayout.Button("GO!"))
				{
					GetComponent<Lobby>().GO();
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();	
	}
	
	void DrawServerBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_aboveServerBoxHeight, m_listBoxWidth,
			GUIConstants.ONE_LINE_BOX_HEIGHT), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, m_aboveServerBoxHeight +
			GUIConstants.BOX_PADDING, m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE,
			GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
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
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_aboveServerListBoxHeight, Screen.width - 20.0f, m_listBoxHeight), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, m_aboveServerListBoxHeight +
			GUIConstants.BOX_PADDING, Screen.width - 30.0f, m_listBoxHeight - 5.0f));
		
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
		GUI.skin.font = m_font;
		
		float gapsHeight = GUIConstants.GAP_SIZE * 6.0f;
		float oneLineBoxesHeight = GUIConstants.ONE_LINE_BOX_HEIGHT * 3.0f;
		m_listBoxHeight = (Screen.height - oneLineBoxesHeight - gapsHeight) / 2.0f;
		m_listBoxWidth = Screen.width - GUIConstants.GAP_SIZE_DOUBLE;
		m_listBoxWidthInternal = m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE;
		m_abovePlayerListBoxHeight = GUIConstants.GAP_SIZE * 4.0f + GUIConstants.ONE_LINE_BOX_HEIGHT * 2.0f +
			m_listBoxHeight;

		DrawPlayerListBox();
		DrawPlayerNameBox();
		DrawReadyGoBox();
		DrawServerBox();
		DrawServerListBox();
	}
}
