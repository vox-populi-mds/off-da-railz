using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {


public Transform target;
public float distance = 1.0f;
public float rotationDamping = 4.0f;
public float heightDamping = 2.0f;
public float PosDamping = 10;	
public int NumWaypoints = 5;	
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
				Destroy(DummyWaypoints[i].GetComponent("collider"));
				Destroy(DummyWaypoints[i].GetComponent("renderer"));
			}	
		}
		else
		{
			DummyWaypoints = new GameObject[NumWaypoints];
			
			for(int i = 0; i < NumWaypoints; i++)
			{
				DummyWaypoints[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
				Destroy(DummyWaypoints[i].GetComponent("collider"));
				Destroy(DummyWaypoints[i].GetComponent("renderer"));				
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
		float wantedRotationAngle = target.eulerAngles.y;
		float currentRotationAngle = DummyWaypoints[0].transform.eulerAngles.y;
			
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);

		
		DummyWaypoints[0].transform.position = target.position;
		DummyWaypoints[0].transform.position -= currentRotation * Vector3.forward * distance / NumWaypoints;			
		DummyWaypoints[0].transform.LookAt (target);
		
		//Make other dummy objects follow the first		
		for(int i = 1; i < NumWaypoints; i++)
		{
			wantedRotationAngle = DummyWaypoints[i - 1].transform.eulerAngles.y;
			currentRotationAngle = DummyWaypoints[i].transform.eulerAngles.y;		
			
			currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
			currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
			
			DummyWaypoints[i].transform.position = DummyWaypoints[i - 1].transform.position;
			DummyWaypoints[i].transform.position -= currentRotation * Vector3.forward * distance / NumWaypoints;
			
			DummyWaypoints[i].transform.LookAt (DummyWaypoints[i - 1].transform.position);
		}
		
		//Now fo' realz, make the object follow the second dummy object. This makes the object appear to follow it's targets path,
		//rather than the target itself, which looks weird.
		
		wantedRotationAngle = DummyWaypoints[NumWaypoints - 1].transform.eulerAngles.y;
		currentRotationAngle = transform.eulerAngles.y;		
		
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		//Save height for later before setting it.
		float currentHeight = transform.position.y;			
		transform.position = DummyWaypoints[NumWaypoints - 1].transform.position;
		transform.position -= currentRotation * Vector3.forward * distance / NumWaypoints;
		
		//Lerp to target height
		float wantedHeight = target.position.y;
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		Vector3 NewPosition = transform.position;
		NewPosition.y = currentHeight;
		transform.position = NewPosition; 
		
		transform.LookAt (DummyWaypoints[NumWaypoints - 1].transform.position);		
	}
	
	void SetTarget(Transform _Target)
	{
		target = _Target;
	}
}

