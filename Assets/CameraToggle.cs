using UnityEngine;
using System.Collections;

public class CameraToggle : MonoBehaviour {
	public Camera[] Cameras;
	private int cameraIndex = 0;
	public Transform m_target;
	
	// Use this for initialization
	void Start () {
		cameraIndex = 0;
		
		foreach (Camera Cam in Camera.allCameras) 
		{
			Cam.gameObject.SetActive (false);
		}
		
		Cameras[cameraIndex].gameObject.SetActive(true);
	}
	
	void Awake () {
		
		if (!m_target)
			m_target = GameObject.Find("CenterOfMass").transform;
		
		foreach (PositionSmoothFollow psf in GetComponentsInChildren<PositionSmoothFollow>(true))
		{
			psf.m_target = m_target;
		}
		
		foreach (SmoothFollow sf in GetComponentsInChildren<SmoothFollow>(true))
		{
			sf.target = m_target;
		}
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
