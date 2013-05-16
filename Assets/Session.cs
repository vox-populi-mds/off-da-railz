using System;

public class Session
{
	static Session instance;
	
	int m_roundCount;
	
	int m_round;
	
	private Session()
	{
		m_round = 0;
		m_roundCount = 0;
	}
	
	public static Session Get()
	{
		if (instance == null)
		{
			instance = new Session();
		}
		
		return instance;
	}
	
	public int GetRound()
	{
		return m_round;
	}
	
	public int GetRoundCount()
	{
		return m_roundCount;
	}
	
	public void SetRoundCount(int roundCount)
	{
		m_roundCount = roundCount;
	}
	
	public void StartGame()
	{
		m_round = 0;
	}
	
	public void StartRound()
	{
		m_round++;	
	}
}
