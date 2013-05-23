using UnityEngine;
using System;

public class Player
{
	public Color Color
	{
		get;
		set;
	}
	
	public string IPAddress
	{
		get;
		set;
	}
	
	public int LastPing
	{
		get;
		set;
	}
	
	public bool Me
	{
		get;
		set;
	}
	
	public string Name
	{
		get;
		set;
	}
	
	public Ping Pinger
	{
		get;
		set;
	}
	
	public int Port
	{
		get;
		set;
	}
	
	public bool Ready
	{
		get;
		set;
	}
	
	public int RoundScore
	{
		get;
		set;
	}
	
	public int Score
	{
		get;
		set;
	}
	
	public GameObject Train
	{
		get;
		set;
	}
	
	public Player()
	{
		IPAddress = "";
		Me = false;
		Name = "Unknown";
		Port = 0;
		Ready = false;
		RoundScore = 0;
		Score = 0;
		Train = null;
	}
	
	public void Apply(NetworkPlayer networkPlayer)
	{
		IPAddress = networkPlayer.ipAddress;
		Port = networkPlayer.port;
	}
	
	public bool Matches(NetworkPlayer networkPlayer)
	{
		return IPAddress == networkPlayer.ipAddress && Port == networkPlayer.port;
	}
	
	public static bool operator==(Player lhs, Player rhs)
	{
	    // If both are null, or both are same instance, return true.
	    if (System.Object.ReferenceEquals(lhs, rhs))
	    {
	        return true;
	    }
		
	    // If one is null, but not both, return false.
	    if (((object)lhs == null) || ((object)rhs == null))
	    {
	        return false;
	    }
		
		return lhs.IPAddress == rhs.IPAddress && lhs.Port == rhs.Port;
	}
	
	public static bool operator!=(Player lhs, Player rhs)
	{
		return !(lhs == rhs);
	}
}
