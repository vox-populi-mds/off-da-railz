using UnityEngine;
using System.Collections;

public class Client : MonoBehaviour
{
	void OnGUI()
	{
		if (GUILayout.Button("Start Server"))
		{
			// Use NAT punchthrough if no public IP present
			Network.InitializeServer(32, 25002, !Network.HavePublicAddress());
			MasterServer.RegisterHost("VoxPopuli::OffDaRails", "JohnDoes game", "l33t game for all");
		}
		
		if (GUILayout.Button("Refresh Server List"))
		{
			MasterServer.RequestHostList("VoxPopuli::OffDaRails");
		}
		
		HostData[] hosts = MasterServer.PollHostList();
		
		// Go through all the hosts in the host list
		foreach (HostData host in hosts)
		{
			GUILayout.BeginHorizontal();	
			string name = host.gameName + " " + host.connectedPlayers + " / " + host.playerLimit;
			GUILayout.Label(name);	
			GUILayout.Space(5);
			string hostInfo;
			hostInfo = "[";
			foreach (string ip in host.ip)
			{
				hostInfo = hostInfo + ip + ":" + host.port + " ";
			}
			hostInfo = hostInfo + "]";
			GUILayout.Label(hostInfo);	
			GUILayout.Space(5);
			GUILayout.Label(host.comment);
			GUILayout.Space(5);
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Connect"))
			{
				// Connect to HostData struct, internally the correct method is used (GUID when using NAT).
				Network.Connect(host);			
			}
			GUILayout.EndHorizontal();	
		}
	}
	
	void Start()
	{
	}
	
	void Update()
	{
	}
}
