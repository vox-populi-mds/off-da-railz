using UnityEngine;
using System;

public class Session
{
	const string GAME_TYPE = "VoxPopuli::OffDaRailz";
	
	const int PORT = 25002;
	
	static Session instance;
	
	bool m_leftGame;
	
	int m_roundCount;
	
	int m_round;
	
	public bool Connected
	{
		get;
		private set;
	}
	
	public string Description
	{
		get;
		set;
	}
	
	public string Name
	{
		get;
		set;
	}
	
	private Session()
	{
		Description = "Thy train shall be wreckethed.";
		Connected = false;
		Name = "Train wreck!";
		
		m_leftGame = false;
		m_round = 0;
		m_roundCount = 0;
		
		Application.runInBackground = true;
		MasterServer.RequestHostList(GAME_TYPE);
		Network.sendRate = 100;
	}
	
	public void Connect(HostData host)
	{
		// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
		if (Network.Connect(host) == NetworkConnectionError.NoError)
		{
			Players.Get().GetMe().Ready = false;
			Connected = true;
		}
		else // The host list is probably out of date.
		{
			MasterServer.RequestHostList(GAME_TYPE);
		}
	}
	
	public void Disconnect()
	{
		if (Network.isServer)
		{
			Network.Disconnect();
			MasterServer.UnregisterHost();
			MasterServer.RequestHostList(GAME_TYPE);
			Connected = false;
		}
		else if (Network.isClient)
		{
			Network.Disconnect();
			Players.Get().RemoveOthers();
			Connected = false;
		}
	}
	
	public void EndRound()
	{
		foreach (Player player in Players.Get().GetAll())
		{
			player.Score += player.RoundScore;
		}
		
		Application.LoadLevel("Score");
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
	
	public void Host()
	{
		// Use NAT punchthrough if no public IP present
		// Limit max connections to 16
		Network.InitializeServer(15, PORT, !Network.HavePublicAddress());
		MasterServer.RegisterHost(GAME_TYPE, Name, Description);
		Player me = Players.Get().GetMe();
		me.Apply(Network.player); // I think at this stage the network player isn't even initialised so this is probably a waste of time...
		me.Color = Color.red;
		me.Ready = false;
		Connected = true;
	}
	
	public void EndGame()
	{
		Application.LoadLevel("Lobby");
	}
	
	public void FindHosts()
	{
		MasterServer.RequestHostList(GAME_TYPE);
	}
	
	public HostData[] GetHosts()
	{
		return MasterServer.PollHostList();	
	}
	
	public void LeaveGame()
	{
		m_leftGame = true;
		EndGame();
	}
	
	public bool LeftGame()
	{
		return m_leftGame;
	}
	
	public void Quit()
	{
		Application.Quit();
	}
	
	public void SetRoundCount(int roundCount)
	{
		m_roundCount = roundCount;
	}
	
	public void StartGame()
	{
		m_leftGame = false;
		m_round = 0;
	}
	
	public void StartRound()
	{
		m_round++;	
	}
}
