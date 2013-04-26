using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour
{
	public Transform train;
	public Transform cameras;
	
	void Start()
	{
		// Diable cursor visibility
		Screen.showCursor = false;
		
		Network.Instantiate(train, new Vector3(0.0f, 3.712008f, 0.0f), Quaternion.identity, 0);
		Network.Instantiate(cameras, new Vector3(0.0f, 3.712008f, 0.0f), Quaternion.identity, 0);
	}

	void Update()
	{
	}
}
