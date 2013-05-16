// Scott Emery

using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	Ping m_Ping;
	
	float m_aboveScoreBoxHeight;
	
	float m_boxWidth;
	
	float m_nextRoundCountdown;
	
	float m_scoreBoxHeight;
	
	void Awake()
	{
		m_aboveScoreBoxHeight = GUIConstants.GAP_SIZE_DOUBLE + GUIConstants.ONE_LINE_BOX_HEIGHT;
		m_nextRoundCountdown = 20.0f;
	}
	
	void DrawCountdownBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, Screen.height - GUIConstants.GAP_SIZE -
			GUIConstants.ONE_LINE_BOX_HEIGHT, m_boxWidth, GUIConstants.ONE_LINE_BOX_HEIGHT),
			"Next Round Starting in " + (int) m_nextRoundCountdown);
	}
	
	void DrawScoreBox()
	{
		GUI.Box(new Rect(GUIConstants.GAP_SIZE, m_aboveScoreBoxHeight, m_boxWidth, m_scoreBoxHeight), "");
		
		GUILayout.BeginArea(new Rect(GUIConstants.GAP_SIZE + GUIConstants.BOX_PADDING,
			GUIConstants.GAP_SIZE + m_aboveScoreBoxHeight + GUIConstants.BOX_PADDING, Screen.width - GUIConstants.GAP_SIZE +
			GUIConstants.BOX_PADDING, m_scoreBoxHeight - GUIConstants.BOX_PADDING_DOUBLE));
		
		foreach (Player player in Players.Get().GetAll())
		{
			GUILayout.BeginHorizontal();
			GUILayout.Label(player.Name);
			GUILayout.Space(3);
			GUILayout.Label(player.Score.ToString());
			GUILayout.Space(3);
			//GUILayout.Label(player.NetworkPlayer.ipAddress);
			GUILayout.Label(player.ping.time.ToString());
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
	}
	
	void Update()
	{
		m_boxWidth = Screen.width - GUIConstants.GAP_SIZE_DOUBLE;
		m_scoreBoxHeight = Screen.height - m_aboveScoreBoxHeight - GUIConstants.GAP_SIZE_DOUBLE -
			GUIConstants.ONE_LINE_BOX_HEIGHT;
		
		m_nextRoundCountdown -= Time.deltaTime;
		Players.Get().PingAll();
	}
}
