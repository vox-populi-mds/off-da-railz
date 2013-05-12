using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	public Transform train;
	public Transform cameras;
	public Transform userInterface;
	public Transform[] levelObstacles = new Transform[10];
	void Start()
	{
		// Disable cursor visibility
		Screen.showCursor = false;
		
		Object BoxObj = new Object();
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
			GameObject trainObject = ((Transform) Instantiate(train, new Vector3(Random.Range(-300.0f, 300.0f), 5.0f, Random.Range(-300.0f, 300.0f)), Quaternion.identity)).gameObject;
			trainObject.GetComponent<Train>().SetMine(true);
		}
		
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
		/*if (Input.GetKey(KeyCode.Escape))
		{
			Application.Quit();
			Debug.Break();
		}*/
	}
}
