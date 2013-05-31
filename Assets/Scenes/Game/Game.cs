using UnityEngine;
using System;
using System.Collections;

public class Game : MonoBehaviour
{
	public Transform 	train;
	public Transform 	cameras;
	public Transform 	userInterface;
	public Transform[] 	levelObstacles = new Transform[100];
	private Transform 	m_clientsCameras = null;
	
	bool m_roundTimeExpired;
	
	bool m_trainsLinked;
	
	public bool debug_mode
	{
		get;
		set;
	}
	
	public bool RoundStarted
	{
		get
		{
			return RoundTimeElapsed > 0.0f;
		}
	}
	
	public float RoundStartTime
	{
		get
		{
			return 6.0f;
		}
	}
	
	public float RoundTimeElapsed
	{
		get
		{
			return Time.timeSinceLevelLoad - RoundStartTime;	
		}
	}

	public float RoundTimeLimit
	{
		get
		{
			return 90.0f;
		}
	}
	
	public float RoundTimeRemaining
	{
		get
		{
			return Math.Min(RoundTimeLimit - RoundTimeElapsed, RoundTimeLimit);	
		}
	}
	
	void Awake()
	{
		m_roundTimeExpired = false;
		m_trainsLinked = false;
	}
	
	void CreateTrain()
	{
		GameObject trainObject = null;
		
		if(Network.isClient || Network.isServer)
		{
			trainObject = ((Transform) Network.Instantiate(train, Players.Get().GetMe().SpawnPostion, Players.Get().GetMe().SpawnRotation, 0)).gameObject;
			trainObject.GetComponent<Train>().SetMine(true);
			networkView.RPC("OnCreateTrain", RPCMode.Others, Network.player.ipAddress, Network.player.port,
				trainObject.networkView.viewID.ToString());
			OnCreateTrain(Network.player.ipAddress, Network.player.port, trainObject.networkView.viewID.ToString());
		}
		else 
		{
			trainObject = ((Transform) Instantiate(train, Players.Get().GetMe().SpawnPostion, Players.Get().GetMe().SpawnRotation)).gameObject;			
			trainObject.GetComponent<Train>().SetMine(true);
		}
	}
	
	// This whole madness is not in the OnCreateTrain method because I'm assuming the order of network messages is not guaranteed
	// i.e. we might get told about the creation of a train before it has actully been created on this client.
	void LinkTrains()
	{
		if (!m_trainsLinked)
		{
			bool unlinkedTrainExists = false;
			foreach (Player player in Players.Get().GetAll())
			{
				if (player.Train == null)
				{
					GameObject[] trainObjects = GameObject.FindGameObjectsWithTag("Player");
					foreach (GameObject trainObject in trainObjects)
					{
						if (trainObject.networkView != null &&
							player.TrainViewID == trainObject.networkView.viewID.ToString())
						{
							player.Train = trainObject;
							Transform playerMarker = trainObject.transform.FindChild("PlayerMarker");
							playerMarker.renderer.material.color = player.Color;
							playerMarker.GetComponent<Light>().color = player.Color;
							playerMarker.GetComponent<TextMesh>().text = player.Name;
							
							if (player.Me)
							{
								playerMarker.GetComponent<TextMesh>().text = null;
							}
							
							break;
						}
					}
					
					if (player.Train == null)
					{
						unlinkedTrainExists = true;
					}
				}
			}
			
			m_trainsLinked = !unlinkedTrainExists;
		}
	}
	
	[RPC]
	void OnCreateTrain(string ipAddress, int port, string id)
	{
		Players.Get().Get(ipAddress, port).TrainViewID = id;
	}
	
	void ProcessScores()
	{	
		foreach (Player player in Players.Get().GetAll())
		{
			if (player.Train != null)
			{
				player.RoundScore = player.Train.GetComponent<TrainCarriages>().GetNumCarriages();
			}
		}
	}
	
	void ProcessPlayerMarkerText()
	{
		foreach (Player player in Players.Get().GetAll())
		{
			if (player.Train != null && !player.Me)
			{
				GameObject PM = player.Train.transform.FindChild("PlayerMarker").gameObject;
				TextMesh playerText = PM.GetComponent<TextMesh>();
				
				float fDistanceToPlayer = Vector3.Distance(player.Train.transform.position, Players.Get().GetMe().Train.transform.position);
				float fMaxDistance = 1000.0f;
				float fMinCharSize = 1.0f;
				float fMaxCharSize = 5.0f;
				
				playerText.characterSize = Mathf.Clamp(fMinCharSize + (fMaxCharSize - fMinCharSize) * (fDistanceToPlayer/fMaxDistance), fMinCharSize, fMaxCharSize);
				
				Vector3 v3Look = (player.Train.transform.position - m_clientsCameras.GetComponent<CameraToggle>().GetActiveCamera().position);
				
				PM.transform.rotation = Quaternion.LookRotation(v3Look);
			}
		}
	}
	
	void Start()
	{		
		CreateTrain();
		
		m_clientsCameras = ((Transform) Instantiate(cameras, Vector3.zero, Quaternion.identity));
		
		// Instantiate UI
		GameObject ui = ((Transform) Instantiate(userInterface, Vector3.zero, Quaternion.identity)).gameObject;
		
		debug_mode = false;
		
		// Instantiate Obstacles
		foreach (Transform obstacle in levelObstacles) {
			Instantiate(obstacle, obstacle.localPosition, obstacle.localRotation);
		}
	}

	void Update()
	{		
		LinkTrains();
		
		ProcessScores();
		
		ProcessPlayerMarkerText();
		
		if (!m_roundTimeExpired &&
			RoundTimeElapsed > RoundTimeLimit)
		{
			m_roundTimeExpired = true;
			Session.Get().EndRound();
		}
	}
}
