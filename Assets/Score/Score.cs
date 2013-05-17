using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	float m_pingCooldown;
	
	float m_aboveScoreBoxHeight;
	
	float m_boxWidth;
	
	float m_nextRoundCountdown;
	
	float m_scoreBoxHeight;
	
	void Awake()
	{
		m_aboveScoreBoxHeight = GUIConstants.GAP_SIZE_DOUBLE + GUIConstants.ONE_LINE_BOX_HEIGHT;
		m_nextRoundCountdown = 20.0f;
		
		// Enable cursor visibility
		Screen.showCursor = true;
	}
	
	void DrawCountdownBox()
	{
		string countdownText = "";
		if (Session.Get().GetRound() < Session.Get().GetRoundCount())
		{
			countdownText = "Next Round Starting in " + (int) m_nextRoundCountdown;
		}
		
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, Screen.height - GUIConstants.GAP_SIZE -
			GUIConstants.ONE_LINE_BOX_HEIGHT, m_boxWidth, GUIConstants.ONE_LINE_BOX_HEIGHT),
			countdownText);
		
		GUILayout.BeginArea(new Rect(Screen.width - GUIConstants.GAP_SIZE - GUIConstants.BOX_PADDING - 200.0f,
			Screen.height - GUIConstants.GAP_SIZE - GUIConstants.ONE_LINE_BOX_HEIGHT + GUIConstants.BOX_PADDING,
			200.0f, GUIConstants.ONE_LINE_BOX_HEIGHT - GUIConstants.BOX_PADDING_DOUBLE));
		
		if (GUILayout.Button("Leave Game"))
		{
			Application.Quit();
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
			GUILayout.Label(player.LastPing.ToString(), GUILayout.Width(column2width));
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
			Players.Get().PingAll();
		}
		if (Session.Get().GetRound() < Session.Get().GetRoundCount())
		{
			m_nextRoundCountdown -= Time.deltaTime;

			if (m_nextRoundCountdown < 0.0f)
			{
				Application.LoadLevel("Level0");
			}
		}
	}
}
