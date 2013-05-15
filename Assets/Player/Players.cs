using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Players : MonoBehaviour
{
	List<Player> m_players;
	
	Players()
	{
		m_players = new List<Player>();
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
	
	public void Clear()
	{
		m_players.Clear();
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
		return GameObject.Find("Players(Clone)").GetComponent<Players>();
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
	
	void Start()
	{
	}
	
	void Update()
	{
	}
}
