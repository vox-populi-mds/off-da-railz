using UnityEngine;
using System;

public class Player
{
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
	
	public NetworkPlayer NetworkPlayer
	{
		get;
		set;
	}
	
	public Ping Pinger
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
		Me = false;
		Name = "Unknown";
		Ready = false;
		RoundScore = 0;
		Score = 0;
		Train = null;
		//ping = new Ping(NetworkPlayer.ipAddress);
	}
}
