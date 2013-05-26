using UnityEngine;
using System;

public class Session
{
	const string GAME_TYPE = "VoxPopuli::OffDaRailz";
	
	const int PORT = 25002;
	
	static Session instance;
	
	bool m_leftGame;
	
	int m_levelPrefix;
	
	int m_roundCount;
	
	int m_round;
	
	bool m_sessionInProgress = false;
	
	bool m_readyToLoadGame = false;
	
	public bool ReadyToLoadGame
	{
		get { return m_readyToLoadGame; }
		set { m_readyToLoadGame = value; }
	}
	
	public bool GameInSession
	{
		get { return m_sessionInProgress; }
		set { m_sessionInProgress = value; }
	}
	
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
		m_levelPrefix = 0;
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
		Network.Disconnect();
		Players.Get().RemoveOthers();
		Connected = false;
		m_sessionInProgress = false;
		
		if (Network.isServer)
		{
			MasterServer.UnregisterHost();
			MasterServer.RequestHostList(GAME_TYPE);
		}
	}
	
	public void EndGame()
	{
		LoadLevel("Lobby");
	}
	
	public void EndRound()
	{
		LoadLevel("Score");
		
		foreach (Player player in Players.Get().GetAll())
		{
			player.Score += player.RoundScore;
			player.Train = null;
		}
		m_sessionInProgress = true;
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
	
	void LoadLevel(string level)
	{
		m_levelPrefix++;
		
		// There is no reason to send any more data over the network on the default channel,
		// because we are about to load the level, thus all those objects will get deleted anyway
		Network.SetSendingEnabled(0, false);	

		// We need to stop receiving because first the level must be loaded first.
		// Once the level is loaded, rpc's and other state update attached to objects in the level are allowed to fire
		Network.isMessageQueueRunning = false;

		// All network views loaded from a level will get a prefix into their NetworkViewID.
		// This will prevent old updates from clients leaking into a newly created scene.
		Network.SetLevelPrefix(m_levelPrefix);
		Application.LoadLevel(level);

		// Allow receiving data again
		Network.isMessageQueueRunning = true;
		// Now the level has been loaded and we can start sending out data to clients
		Network.SetSendingEnabled(0, true);
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
		m_sessionInProgress = true;
		m_leftGame = false;
		m_round = 0;
		//Players.Get().
	}
	
	public void StartRound()
	{		
		m_round++;
		LoadLevel("Game");
	}
	
	
}
