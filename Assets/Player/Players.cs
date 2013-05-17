using UnityEngine;
using System.Collections.Generic;

public class Players
{
	static Players instance;
	
	List<Player> m_players;
	
	private Players()
	{
		m_players = new List<Player>();
		Player me = new Player();
		me.Me = true;
		me.Name = "John Doe";
		me.NetworkPlayer = Network.player;
		me.Ready = false;
		m_players.Add(me);
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
			/*if (player.pinger.isDone)
			{
				player.LastPing = player.Pinger.time;
				player.Pinger = new Ping(player.NetworkPlayer.ipAddress);
			}*/
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
		foreach (Player player in m_players)
		{
			if (player.Me)
			{
				return player;
			}
		}
		
		return null;
	}
	
	public void Remove(NetworkPlayer networkPlayer)
	{
		List<Player> players = new List<Player>(m_players);
		foreach (Player player in players)
		{
			if (new NetworkPlayerComparer().Equals(networkPlayer, player.NetworkPlayer))
			{
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
