using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	float m_pingCooldown;
	
	float m_aboveScoreBoxHeight;
	
	float m_boxWidth;
	
	float m_countdown;
	
	float m_scoreBoxHeight;
	
	public Font m_font;
	
	void Awake()
	{
		m_aboveScoreBoxHeight = GUIConstants.GAP_SIZE_DOUBLE + GUIConstants.ONE_LINE_BOX_HEIGHT;
		m_countdown = 20.0f;
		
		// Enable cursor visibility
		Screen.showCursor = true;
	}
	
	void DrawCountdownBox()
	{
		string countdownText = "";
		if (Session.Get().GetRound() < Session.Get().GetRoundCount())
		{
			countdownText = "Next Round Starting in " + (int) m_countdown;
		}
		else
		{
			countdownText = "Returning to Lobby in " + (int) m_countdown;
		}
		
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, Screen.height - GUIConstants.GAP_SIZE -
			GUIConstants.ONE_LINE_BOX_HEIGHT, m_boxWidth, GUIConstants.ONE_LINE_BOX_HEIGHT),
			countdownText);
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING,
			Screen.height - GUIConstants.GAP_SIZE - GUIConstants.ONE_LINE_BOX_HEIGHT + GUIConstants.BOX_PADDING,
			200.0f, GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
		if (GUILayout.Button("Quit"))
		{
			Session.Get().Quit();
		}
		
		GUILayout.EndArea();
		
		GUILayout.BeginArea(new Rect(Screen.width - GUIConstants.GAP_SIZE - GUIConstants.BOX_PADDING - 200.0f,
			Screen.height - GUIConstants.GAP_SIZE - GUIConstants.ONE_LINE_BOX_HEIGHT + GUIConstants.BOX_PADDING,
			200.0f, GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
		if (GUILayout.Button("Leave Game"))
		{
			Session.Get().LeaveGame();
		}
		
		GUILayout.EndArea();
	}
	
	void DrawScoreBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_aboveScoreBoxHeight, m_boxWidth, m_scoreBoxHeight), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING,
			GUIConstants.GAP_SIZE + m_aboveScoreBoxHeight + GUIConstants.BOX_PADDING, m_boxWidth -
			GUIConstants.BOX_PADDING_DOUBLE, m_scoreBoxHeight -
			GUIConstants.BOX_PADDING_DOUBLE));
		
		float column0width = (m_boxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.7f;
		float column1width = (m_boxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.15f;
		float column2width = (m_boxWidth - GUIConstants.BOX_PADDING_DOUBLE) * 0.15f;
		
		GUILayout.BeginHorizontal();
		GUILayout.Label("Player", GUILayout.Width(column0width));
		GUILayout.Label("Score", GUILayout.Width(column1width));
		GUILayout.Label("Ping", GUILayout.Width(column2width));
		GUILayout.EndHorizontal();
		
		foreach (Player player in Players.Get().GetAll())
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(player.Name, GUILayout.Width(column0width));
			GUILayout.Label(player.Score.ToString(), GUILayout.Width(column1width));
			GUILayout.Label(Network.GetAveragePing(player.NetworkPlayer).ToString(), GUILayout.Width(column2width));
			//GUILayout.Label(player.LastPing.ToString(), GUILayout.Width(column2width));
			GUILayout.EndHorizontal();
		}
		
		GUILayout.EndArea();	
	}
	
	void DrawTitleBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, GUIConstants.GAP_SIZE, m_boxWidth, GUIConstants.ONE_LINE_BOX_HEIGHT),
			"Score After Round " + Session.Get().GetRound());
	}
	
	void OnGUI()
	{
		GUI.skin.font = m_font;
		
		DrawCountdownBox();
		DrawScoreBox();
		DrawTitleBox();
	}
	
	void Start()
	{ 
		m_pingCooldown = 0.0f;
	}
	
	void Update()
	{
		m_pingCooldown += Time.deltaTime;
		m_boxWidth = Screen.width - GUIConstants.GAP_SIZE_DOUBLE;
		m_scoreBoxHeight = Screen.height - m_aboveScoreBoxHeight - GUIConstants.GAP_SIZE_DOUBLE -
			GUIConstants.ONE_LINE_BOX_HEIGHT;
		
		if (m_pingCooldown > 1)
		{
			m_pingCooldown = 0;
		}
		
		m_countdown -= Time.deltaTime;
		
		if (m_countdown < 0.0f)
		{
			if (Session.Get().GetRound() < Session.Get().GetRoundCount())
			{
				Application.LoadLevel("Game");
			}
			else
			{
				Session.Get().EndGame();
			}
		}
	}
}
