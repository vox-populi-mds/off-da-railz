using System;

public class Session
{
	static Session instance;
	
	int m_round;
	
	private Session()
	{
		m_round = 0;
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
	
	public void StartRound()
	{
		m_round++;	
	}
}
