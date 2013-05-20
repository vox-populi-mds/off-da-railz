using UnityEngine;
using System;
using System.Collections;

public class Game : MonoBehaviour
{
	public Transform 	train;
	public Transform 	cameras;
	public Transform 	userInterface;
	public Transform[] 	levelObstacles = new Transform[10];
	private Transform 	m_clientsCameras = null;
			
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
		m_trainsLinked = false;
	}
	
	void CreateTrain()
	{
		GameObject trainObject = null;
		
		if(Network.isClient || Network.isServer)
		{
			trainObject = ((Transform) Network.Instantiate(train, new Vector3(UnityEngine.Random.Range(-300.0f, 300.0f),
				5.0f, UnityEngine.Random.Range(-300.0f, 300.0f)), Quaternion.identity, 0)).gameObject;
			if (trainObject.GetComponent<NetworkView>().isMine)
			{
				trainObject.GetComponent<Train>().SetMine(true);
			}
		}
		else 
		{
			trainObject = ((Transform) Instantiate(train, new Vector3(0.0f, 20.0f, 0.0f), Quaternion.identity))
				.gameObject;
			trainObject.GetComponent<Train>().SetMine(true);
		}
	}
	
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
						if (trainObject.networkView.owner == player.NetworkPlayer)
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
	
	void ProcessScores()
	{	
		foreach (Player player in Players.Get().GetAll())
		{
			if (player.Train != null)
			{
				player.RoundScore = player.Train.GetComponent<TrainCarriages>().GetNumCarriages();
			}
		}
		
		// Check if the round should finish.
		if (RoundTimeElapsed > RoundTimeLimit)
		{
			Session.Get().EndRound();
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
		Session.Get().StartRound();
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
	}
}
