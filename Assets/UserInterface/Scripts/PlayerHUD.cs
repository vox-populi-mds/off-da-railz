using UnityEngine;
using System;
using System.Collections;

public class PlayerHUD : MonoBehaviour
{
	const int MAX_COUNTDOWN_SIZE = 60;
	
	const int ROUND_TIMER_WIDTH = 200;
	
	public Texture2D[] textureCarridges = new Texture2D[3];
	public Texture2D textureWeaponShotgun;
	
	bool m_menu;
	
	float m_menuBoxSize;
	
	float m_menuPaddingTop;
	
	void Awake()
	{
		// Disable cursor visibility
		Screen.showCursor = false;
		
		m_menu = false;
		m_menuBoxSize = 200;
		m_menuPaddingTop = 50;
	}
	
	void DrawCarriages()
	{
		TrainCarriages trainCarriages = Players.Get().GetMe().Train.GetComponent<TrainCarriages>();
		
		for (int index = 1; index <= trainCarriages.GetNumCarriages(); index++)
		{
			GUI.DrawTexture(new Rect(Screen.width - index * 100, 0, 100, 100), textureCarridges[0]);
		}
	}
	
	void DrawCountdown()
	{
		GUIStyle style = new GUIStyle();
		style.alignment = TextAnchor.MiddleCenter;
		style.fontSize = (int) (MAX_COUNTDOWN_SIZE * (Time.timeSinceLevelLoad - Mathf.Floor(Time.timeSinceLevelLoad)));
		
		Game game = GameObject.Find("The Game").GetComponent<Game>();
		float countdown = game.RoundStartTime - Time.timeSinceLevelLoad;
		string countdownText = null;
		if (countdown >= 1 && countdown < Game.COUNTDOWN_START + 1)
		{
			countdownText = ((int) countdown).ToString();
		}
		else if (countdown >= 0 && countdown < 1)
		{
			countdownText = "Fight!";
		}
		
		GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 100, 200, 200), countdownText, style);
	}
	
	void DrawMenu()
	{
		if (m_menu)
		{
			float halfMenuBoxSize = m_menuBoxSize / 2.0f;
			
			GUI.Box(new Rect(Screen.width / 2 - halfMenuBoxSize, Screen.height / 2 - halfMenuBoxSize, m_menuBoxSize,
				m_menuBoxSize), "Menu");
			
			GUILayout.BeginArea(new Rect(Screen.width / 2 - halfMenuBoxSize + GUIConstants.BOX_PADDING,
				Screen.height / 2 - halfMenuBoxSize + m_menuPaddingTop, m_menuBoxSize - GUIConstants.BOX_PADDING_DOUBLE,
				m_menuBoxSize - GUIConstants.BOX_PADDING_DOUBLE - m_menuPaddingTop));
				
			if (GUILayout.Button("Continue"))
			{
				Screen.showCursor = false;
				m_menu = false;
			}
			
			GUILayout.Button("Leave Game");
			
			GUILayout.EndArea();
		}
	}
	
	void DrawRoundTimer()
	{
		Game game = GameObject.Find("The Game").GetComponent<Game>();
		
		GUI.Label(new Rect(GUIConstants.GAP_SIZE, GUIConstants.GAP_SIZE, ROUND_TIMER_WIDTH,
			GUIConstants.ONE_LINE_BOX_HEIGHT), "Round " + Session.Get().GetRound() + " : " +
			(int) game.RoundTimeRemaining);
	}
	
	void DrawWeapons()
	{
		GUI.DrawTexture(new Rect((textureWeaponShotgun.width / 2) - 100, Screen.height - (textureWeaponShotgun.height / 2),
			(textureWeaponShotgun.width / 2), (textureWeaponShotgun.height / 2)), textureWeaponShotgun);
 	}
	
	void OnGUI()
	{
		DrawCarriages();
		DrawCountdown();
		DrawMenu();
		DrawRoundTimer();
		DrawWeapons();
	}
	
	void Start()
	{
	}
	
	void Update() 
	{
		if (Input.GetKey(KeyCode.Escape) && m_menu)
		{
			m_menu = true;
			Screen.showCursor = true;
		}
		else if (Input.GetKeyUp(KeyCode.Escape) && !m_menu)
		{
			m_menu = false;
			Screen.showCursor = false;	
		}
	}
}
