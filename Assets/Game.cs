using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	public Transform train;
	public Transform cameras;
	
	
	void Start()
	{
		// Disable cursor visibility
		Screen.showCursor = false;
		
		Object networkTrainObject = Network.Instantiate(train, new Vector3(0.0f, 3.712008f, 0.0f), Quaternion.identity, 0);
		if (networkTrainObject != null)
		{
			
			GameObject trainObject = ((Transform) networkTrainObject).gameObject;
			if (trainObject.GetComponent<NetworkView>().isMine)
			{
				trainObject.GetComponent<Train>().SetMine(true);
			}
		}
		else // We're just playing a single player game.
		{
			GameObject trainObject = ((Transform) Instantiate(train, new Vector3(0.0f, 3.712008f, 0.0f), Quaternion.identity)).gameObject;
			trainObject.GetComponent<Train>().SetMine(true);
		}
		
		Instantiate(cameras, Vector3.zero, Quaternion.identity);
		

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
