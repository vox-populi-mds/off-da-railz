using UnityEngine;
using System.Collections.Generic;

public class Players
{
	static Players instance;
	
	Player m_me;
	
	List<Player> m_players;
	
	private Players()
	{
		m_me = new Player();
		m_me.Me = true;
		m_me.Name = "John Doe";
		m_me.Ready = false;
		m_players = new List<Player>();
		m_players.Add(m_me);
		
		if(PlayerPrefs.HasKey("PlayerName"))
		{
			m_me.Name = PlayerPrefs.GetString("PlayerName");
		}
	}
	
	public void Add(Player player)
	{
		m_players.Add(player);
	}
	
	public bool AllReady()
	{
		foreach (Player player in m_players)
		{
			if (!player.Ready)
			{
				return false;
			}
		}
		
		return true;
	}
	
	public void PingAll()
	{
		foreach (Player player in m_players)
		{
			if (player.Pinger == null)
			{
				player.Pinger = new Ping(player.IPAddress);
			}
			if (player.Pinger.isDone)
			{
				player.LastPing = player.Pinger.time;
				player.Pinger = new Ping(player.IPAddress);
			}
		}
	}
	
	public Player Get(NetworkPlayer networkPlayer)
	{
		return Get(networkPlayer.ipAddress, networkPlayer.port);
	}
	
	public Player Get(string ipAddress, int port)
	{
		foreach (Player player in m_players)
		{
			if (player.IPAddress == ipAddress && player.Port == port)
			{
				return player;
			}
		}
		
		return null;
	}
	
	public static Players Get()
	{
		if (instance == null)
		{
			instance = new Players();
		}
		
		return instance;
	}
	
	public List<Player> GetAll()
	{
		return m_players;
	}
	
	public Player GetMe()
	{
		return m_me;
	}
	
	public void Remove(NetworkPlayer networkPlayer)
	{
		List<Player> players = new List<Player>(m_players);
		foreach (Player player in players)
		{
			if (player.Matches(networkPlayer))
			{
				//player.Pinger.DestroyPing();
				m_players.Remove(player);
			}
		}
	}
	
	public void RemoveOthers()
	{
		List<Player> players = new List<Player>(m_players);
		foreach (Player player in players)
		{
			if (!player.Me)
			{
				m_players.Remove(player);
			}
		}
	}
}
