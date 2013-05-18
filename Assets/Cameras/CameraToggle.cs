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
		if (Input.GetKeyDown(KeyCode.C))
		{
			int iDistanceScale = m_target.parent.GetComponent<TrainCarriages>().GetNumCarriages();
						
			int prev = cameraIndex;
			cameraIndex++;			
			if (cameraIndex	>= Cameras.Length) {
				cameraIndex = 0;
			}
			Cameras[cameraIndex].gameObject.SetActive(true);
			Cameras[prev].gameObject.SetActive(false);			
			
			if(cameraIndex == 0 || cameraIndex == 1)
			{
				Cameras[cameraIndex].GetComponent<SmoothFollow>().distance = distance + 10 * iDistanceScale;
				Cameras[cameraIndex].GetComponent<SmoothFollow>().height = height + 5 * iDistanceScale;
			}				
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
	}
	
	public Transform GetActiveCamera()
	{
		return(Cameras[cameraIndex].transform);
	}
}


