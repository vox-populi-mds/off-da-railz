using UnityEngine;
using System.Collections;

public class CameraToggle : MonoBehaviour {
	public Camera[] Cameras;
	private int cameraIndex = 1;
	public Transform m_target;
	public float distance = 60;
	public float height = 15;
	
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
	void Update () 
	{
		int iDistanceScale = m_target.parent.GetComponent<TrainCarriages>().GetNumCarriages();
		
		if(iDistanceScale > 10)
		{
			iDistanceScale = 10;	
		}
		
		if(cameraIndex == 0 || cameraIndex == 1)
		{
			float NewDistance = distance + 30 * iDistanceScale;
			float NewHeight = height + 5 * iDistanceScale;
			
			NewDistance = Mathf.Lerp(Cameras[cameraIndex].GetComponent<SmoothFollow>().GetDistance(), NewDistance, 2.0f * Time.deltaTime);
			NewHeight = Mathf.Lerp(Cameras[cameraIndex].GetComponent<SmoothFollow>().height, NewHeight, 2.0f * Time.deltaTime);
			
			Cameras[cameraIndex].GetComponent<SmoothFollow>().SetDistance(NewDistance);
			Cameras[cameraIndex].GetComponent<SmoothFollow>().height = NewHeight;
		}		
		
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
		
		//Change FOV depending on speed		
		float Speed = m_target.parent.GetComponent<Train>().GetSpeed();
		float MaxSpeed = m_target.parent.GetComponent<Train>().m_MaximumVelocity;		
		float SpeedScale = 1.0f;
		
		if(Speed > 0)
		{
			SpeedScale = Speed * 2 / MaxSpeed;			
		}
		
		float newFOV = SpeedScale * 75;
		if(newFOV > 60)
		{
			newFOV = Mathf.Lerp(Cameras[cameraIndex].fov, newFOV, Time.deltaTime);
			Cameras[cameraIndex].fov = newFOV;
		}
		if(newFOV > 85)
		{
			newFOV = 85;
		}
	}
	
	public Transform GetActiveCamera()
	{
		return(Cameras[cameraIndex].transform);
	}
}


