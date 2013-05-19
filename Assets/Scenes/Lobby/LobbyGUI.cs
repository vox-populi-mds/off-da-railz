using UnityEngine;

public class LobbyGUI : MonoBehaviour
{
	const float PLAYER_NAME_BOX_WIDTH = 600.0f;
	
	const float SERVER_BOX_WIDTH = 600.0f;
	
	float m_abovePlayerListBoxHeight;
	
	float m_aboveServerBoxHeight;
	
	float m_aboveServerListBoxHeight;
	
	public Font m_font;
	
	float m_listBoxHeight;
	
	float m_listBoxWidth;
	
	float m_listBoxWidthInternal;
	
	void Awake()
	{		
		m_aboveServerBoxHeight = GUIConstants.GAP_SIZE_DOUBLE + GUIConstants.ONE_LINE_BOX_HEIGHT;
		m_aboveServerListBoxHeight = GUIConstants.GAP_SIZE * 3.0f + GUIConstants.ONE_LINE_BOX_HEIGHT * 2.0f;
	}
	
	void DrawPlayerListBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_abovePlayerListBoxHeight, m_listBoxWidth, m_listBoxHeight), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, m_abovePlayerListBoxHeight +
			GUIConstants.BOX_PADDING, m_listBoxWidthInternal, m_listBoxHeight - GUIConstants.BOX_PADDING_DOUBLE));
		
		float column0width = (m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.7f;
		//float column1width = (m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.15f;
		float column2width = (m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.3f;
		
		if (Session.Get().Connected)
		{			
			GUILayout.BeginHorizontal();
			GUILayout.Label("Player", GUILayout.Width(column0width));
			//GUILayout.Label("Ready", GUILayout.Width(column1width));
			GUILayout.Label("Ping", GUILayout.Width(column2width));
			GUILayout.EndHorizontal();
			
			foreach (Player player in Players.Get().GetAll())
			{
				GUILayout.BeginHorizontal();
				GUILayout.Label(player.Name, GUILayout.Width(column0width));
				//GUILayout.Label(player.Ready ? "Yes" : "No", GUILayout.Width(column1width));
				GUILayout.Label(player.LastPing.ToString(), GUILayout.Width(column2width));
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
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, Screen.height - GUIConstants.ONE_LINE_BOX_HEIGHT -
			GUIConstants.GAP_SIZE, m_listBoxWidth, GUIConstants.ONE_LINE_BOX_HEIGHT), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING, Screen.height -
			GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING,
			m_listBoxWidthInternal, GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
		GUILayoutOption widthOption = GUILayout.Width(m_listBoxWidthInternal / 3.0f);
		GUIStyle labelStyle = GUI.skin.GetStyle("Label");
    	labelStyle.alignment = TextAnchor.UpperCenter;
		
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Quit", widthOption))
		{
			Session.Get().Quit();
		}		
		if (Session.Get().Connected)
		{
			if (Players.Get().GetMe().Ready)
			{
				GUILayout.Label("Ready!", labelStyle, widthOption);
			}
			else if (GUILayout.Button("Ready!", widthOption))
			{
				GetComponent<Lobby>().PlayerReady();
			}
			
			if (Network.isServer && Players.Get().AllReady())
			{
				if (GUILayout.Button("GO!", widthOption))
				{
					GetComponent<Lobby>().GO();
				}
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		
		// Revert the label style.
		labelStyle.alignment = TextAnchor.UpperLeft;
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
		Session.Get().Name = GUILayout.TextField(Session.Get().Name);
		GUILayout.Label("Description");
		Session.Get().Description = GUILayout.TextField(Session.Get().Description);
		if (!Session.Get().Connected)
		{
			if (GUILayout.Button("Start Server"))
			{
				GetComponent<Lobby>().Host();
			}
		}
		else if (Network.isServer)
		{
			if (GUILayout.Button("Stop Server"))
			{
				Session.Get().Disconnect();
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
			Session.Get().FindHosts();
		}
		
		HostData[] hosts = Session.Get().GetHosts();
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
							Session.Get().Disconnect();
						}
					}
				}
				else
				{
					if (GUILayout.Button("Connect"))
					{
						Session.Get().Connect(host);
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
		GUI.skin.box.fontSize = GUIConstants.FONT_SIZE;
		GUI.skin.button.fontSize = GUIConstants.FONT_SIZE;
		GUI.skin.label.fontSize = GUIConstants.FONT_SIZE;
		GUI.skin.textField.fontSize = GUIConstants.FONT_SIZE;
		
		float gapsHeight = GUIConstants.GAP_SIZE * 6.0f;
		float oneLineBoxesHeight = GUIConstants.ONE_LINE_BOX_HEIGHT * 3.0f;
		m_listBoxHeight = (Screen.height - oneLineBoxesHeight - gapsHeight) / 2.0f;
		m_listBoxWidth = Screen.width - GUIConstants.GAP_SIZE_DOUBLE;
		m_listBoxWidthInternal = m_listBoxWidth - GUIConstants.BOX_PADDING_DOUBLE;
		m_abovePlayerListBoxHeight = GUIConstants.GAP_SIZE * 4.0f + GUIConstants.ONE_LINE_BOX_HEIGHT * 2.0f +
			m_listBoxHeight;
		
		Players.Get().PingAll();
		
		DrawPlayerListBox();
		DrawPlayerNameBox();
		DrawReadyGoBox();
		DrawServerBox();
		DrawServerListBox();
	}
}
