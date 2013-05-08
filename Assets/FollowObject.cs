using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {


public Transform target;
public float distance = 3.0f;
public float rotationDamping = 3.0f;
public float heightDamping = 2.0f;
public GameObject[] DummyWaypoints = new GameObject[2];

	void Start()
	{
		DummyWaypoints[0] = new GameObject();
		//DummyWaypoints[0].collider.isTrigger = true;
		DummyWaypoints[1] = new GameObject();
		//DummyWaypoints[1].collider.isTrigger = true;
	}
	
	void Update()
	{
		if (!target)
			return;
		
		//Set dummy object to follow target
		
		float wantedRotationAngle = target.eulerAngles.y;
		float currentRotationAngle = DummyWaypoints[0].transform.eulerAngles.y;
			
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		Quaternion currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		DummyWaypoints[0].transform.position = target.position;
		DummyWaypoints[0].transform.position -= currentRotation * Vector3.forward * distance / 3;
		
		DummyWaypoints[0].transform.LookAt (target);
		
		//Make second dummy object follow the first
		
		wantedRotationAngle = DummyWaypoints[0].transform.eulerAngles.y;
		currentRotationAngle = DummyWaypoints[1].transform.eulerAngles.y;		
		
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		DummyWaypoints[1].transform.position = DummyWaypoints[0].transform.position;
		DummyWaypoints[1].transform.position -= currentRotation * Vector3.forward * distance / 3;
		
		DummyWaypoints[1].transform.LookAt (DummyWaypoints[0].transform.position);
		
		//Now fo' realz, make the object follow the second dummy object. This makes the object appear to follow it's targets path,
		//rather than the target itself, which looks weird.
		
		wantedRotationAngle = DummyWaypoints[1].transform.eulerAngles.y;
		currentRotationAngle = transform.eulerAngles.y;		
		
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		//Save height for later before setting it.
		float currentHeight = transform.position.y;		
		transform.position = DummyWaypoints[1].transform.position;
		transform.position -= currentRotation * Vector3.forward * distance / 3;
		
		//Lerp to target height
		
		float wantedHeight = target.position.y;
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, heightDamping * Time.deltaTime);
		
		Vector3 NewPosition = transform.position;
		NewPosition.y = currentHeight;
		transform.position = NewPosition;
		
		transform.LookAt (DummyWaypoints[1].transform.position);		
	}
}

