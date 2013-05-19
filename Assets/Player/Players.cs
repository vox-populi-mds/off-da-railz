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
		m_me.NetworkPlayer = Network.player;
		m_me.Ready = false;
		m_players = new List<Player>();
		m_players.Add(m_me);
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
				player.Pinger = new Ping(player.NetworkPlayer.ipAddress);
			}
			if (player.Pinger.isDone)
			{
				player.LastPing = player.Pinger.time;
				player.Pinger = new Ping(player.NetworkPlayer.ipAddress);
			}
		}
	}
	
	public Player Get(NetworkPlayer networkPlayer)
	{
		foreach (Player player in m_players)
		{
			if (new NetworkPlayerComparer().Equals(networkPlayer, player.NetworkPlayer))
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
			if (new NetworkPlayerComparer().Equals(networkPlayer, player.NetworkPlayer))
			{
				player.Pinger.DestroyPing();
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
