using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class NetworkPlayerComparer : IEqualityComparer<NetworkPlayer>
{
	public bool Equals(NetworkPlayer a, NetworkPlayer b)
	{
		return a.ipAddress == b.ipAddress && a.port == b.port;
	}
	
	public int GetHashCode(NetworkPlayer player)
	{
		int hashCode = 0;
		
		if (player.ipAddress.Length != 0)
		{
			string[] ipComponents = player.ipAddress.Split('.');
			foreach (string ipComponent in ipComponents)
			{
				hashCode += Convert.ToInt32(ipComponent);
			}
		}
		hashCode += player.port;
		
		return hashCode;
	}
}
