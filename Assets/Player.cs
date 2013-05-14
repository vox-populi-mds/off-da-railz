using UnityEngine;
using System;

public class Player
{
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
	
	public bool Ready
	{
		get;
		set;
	}
	
	public int Score
	{
		get;
		set;
	}
	
	public Player()
	{
		Me = false;
		Name = "Unknown";
		Ready = false;
		Score = 0;
	}
}
