using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {


public Transform target;
public float distance = 1.0f;
public float rotationDamping = 25.0f;
public int NumWaypoints = 10;	
public bool DebugMode = false;
protected GameObject[] DummyWaypoints;

	void Start()
	{
		if(DebugMode == true)
		{	
			DummyWaypoints = new GameObject[NumWaypoints];
			
			for(int i = 0; i < NumWaypoints; i++)
			{
				DummyWaypoints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				Destroy(DummyWaypoints[i].GetComponent<SphereCollider>());		
			}	
		}
		else
		{
			DummyWaypoints = new GameObject[NumWaypoints];
			
			for(int i = 0; i < NumWaypoints; i++)
			{
				DummyWaypoints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				Destroy(DummyWaypoints[i].GetComponent<SphereCollider>());
				Destroy(DummyWaypoints[i].GetComponent<Renderer>());				
			}	
		}
	}
	
	void Update()
	{
		if (!target)
		{
			return;
		}
			
		//Set first dummy object to follow target	
		Quaternion currentRotation = LerpToRotation(DummyWaypoints[0].transform.eulerAngles, target.eulerAngles);
		
		DummyWaypoints[0].transform.position = target.position;
		DummyWaypoints[0].transform.position -= currentRotation * Vector3.forward * distance;	
		
		//Look at target
		DummyWaypoints[0].transform.LookAt (target);
		
		//Make other dummy objects follow the first		
		for(int i = 1; i < NumWaypoints; i++)
		{
			currentRotation = LerpToRotation(DummyWaypoints[i].transform.eulerAngles, DummyWaypoints[i - 1].transform.eulerAngles);
			
			DummyWaypoints[i].transform.position = DummyWaypoints[i - 1].transform.position;
			DummyWaypoints[i].transform.position -= currentRotation * Vector3.forward * distance;
			
			DummyWaypoints[i].transform.LookAt (DummyWaypoints[i - 1].transform.position);
		}
		
		//Now fo' realz, make the object follow the second dummy object. This makes the object appear to follow it's targets path,
		//rather than the target itself, which looks weird.
		
		currentRotation = LerpToRotation(transform.eulerAngles, DummyWaypoints[NumWaypoints - 1].transform.eulerAngles);	
	
		transform.position = DummyWaypoints[NumWaypoints - 1].transform.position;
		transform.position -= currentRotation * Vector3.forward * 10;
		
		transform.LookAt (DummyWaypoints[NumWaypoints - 1].transform.position);		
	}
	
	void SetTarget(Transform _Target)
	{
		target = _Target;
	}
	
	Quaternion LerpToRotation (Vector3 CurrentRotation, Vector3 TargetRotation)
	{
		float wantedRotationAngleX = TargetRotation.x;
		float wantedRotationAngleY = TargetRotation.y;
		float wantedRotationAngleZ = TargetRotation.z;
		float currentRotationAngleX = CurrentRotation.x;
		float currentRotationAngleY = CurrentRotation.y;
		float currentRotationAngleZ = CurrentRotation.z;
		
		currentRotationAngleX = Mathf.LerpAngle (currentRotationAngleX, wantedRotationAngleX, rotationDamping * Time.deltaTime);
		currentRotationAngleY = Mathf.LerpAngle (currentRotationAngleY, wantedRotationAngleY, rotationDamping * Time.deltaTime);
		currentRotationAngleZ = Mathf.LerpAngle (currentRotationAngleZ, wantedRotationAngleZ, rotationDamping * Time.deltaTime);
		
		Quaternion currentRotation = Quaternion.Euler (currentRotationAngleX, currentRotationAngleY, currentRotationAngleZ);
		return(currentRotation);
	}
	
	bool HasTarget()
	{
		if (!target)
		{
			return (false);
		}
		else
		{
			return(true);
		}
	}
}

