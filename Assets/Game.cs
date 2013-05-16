using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	const int COUNTDOWN_START = 3;
	public Transform train;
	public Transform cameras;
	public Transform userInterface;
	public Transform[] levelObstacles = new Transform[10];

	public float m_roundStartDelay;
	
	float m_roundStartTime;

	public float m_roundTimeLimit = 90;
	
	void Awake()
	{
		m_roundStartTime = -1.0f;
	}
	
	void CreateTrain()
	{
		if(Network.isClient || Network.isServer)
		{
			Object networkTrainObject = Network.Instantiate(train, new Vector3(Random.Range(-300.0f, 300.0f), 5.0f, Random.Range(-300.0f, 300.0f)), Quaternion.identity, 0);

			GameObject trainObject = ((Transform) networkTrainObject).gameObject;
			if (trainObject.GetComponent<NetworkView>().isMine)
			{
				trainObject.GetComponent<Train>().SetMine(true);
			}
		}
		else 
		{
			GameObject trainObject = ((Transform) Instantiate(train, new Vector3(300.0f, 5.0f, 0.0f), Quaternion.identity)).gameObject;
			trainObject.GetComponent<Train>().SetMine(true);
		}
	}
	
	void OnGUI()
	{
		if(m_roundStartTime == -1.0f)
		{
			if (Time.timeSinceLevelLoad < m_roundStartDelay)
			{
					GUI.Label(new Rect(10, 10, 50, 20), "Round 1");
			}
			else
			{
				if (Time.timeSinceLevelLoad < m_roundStartDelay + COUNTDOWN_START)
				{
					GUI.Label(new Rect(10, 10, 20, 20), ((int) (m_roundStartDelay + COUNTDOWN_START + 1.0f - Time.timeSinceLevelLoad)).ToString());
				}
				else if (Time.timeSinceLevelLoad < m_roundStartDelay + COUNTDOWN_START + 1.0f)
				{
					GUI.Label(new Rect(10, 10, 50, 20), "GO!");
				}
				else
				{
					m_roundStartTime = Time.timeSinceLevelLoad;
				}
			}
		}
		else if (Time.timeSinceLevelLoad - m_roundStartTime > m_roundTimeLimit)
		{
			GUI.Label(new Rect(10, 10, 100, 20), "Round Finished!");
		}
		else
		{
			GUI.Label(new Rect(10, 10, 100, 20), "Round Timer: " + ((int) (m_roundTimeLimit - Time.timeSinceLevelLoad + m_roundStartTime)).ToString());
		}
	}
	
	void Start()
	{
		// Disable cursor visibility
		Screen.showCursor = false;
		Session.Get().StartRound();
		
		CreateTrain();
		Network.sendRate = 100;
		
		Instantiate(cameras, Vector3.zero, Quaternion.identity);
		
		// Instantiate UI
		GameObject ui = ((Transform) Instantiate(userInterface, Vector3.zero, Quaternion.identity)).gameObject;
		
		// Instantiate Obstacles
		foreach (Transform obstacle in levelObstacles) {
			//Instantiate(obstacle, obstacle.localPosition, obstacle.localRotation);
		}
	}

	void Update()
	{
		if (Time.timeSinceLevelLoad - m_roundStartTime > m_roundTimeLimit + 3.0f)
		{
			Application.LoadLevel("Score");
		}
	}
}
