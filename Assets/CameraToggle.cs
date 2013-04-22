using UnityEngine;
using System.Collections;

public class CameraToggle : MonoBehaviour {
	public Camera[] Cameras;
	private int cameraIndex = 0;
	
	// Use this for initialization
	void Start () {
		cameraIndex = 0;
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.C))
		{
			int prev = cameraIndex;
			cameraIndex++;			
			if (cameraIndex	>= Cameras.Length) {
				cameraIndex = 0;
			}
			Cameras[cameraIndex].gameObject.SetActive(true);
			Cameras[prev].gameObject.SetActive(false);
		}				
	}
}
