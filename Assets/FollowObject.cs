using UnityEngine;
using System.Collections;

public class FollowObject : MonoBehaviour {


public Transform target;
public float distance = 3.0f;
public float rotationDamping = 3.0f;

	void Start()
	{
		//Empty.
	}
	
	void Update()
	{
		if (!target)
			return;
		
		float wantedRotationAngle = target.eulerAngles.y;
		float currentRotationAngle = transform.eulerAngles.y;
		
	
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
	
		transform.position = target.position;
		transform.position -= currentRotation * Vector3.forward * distance;
		
		transform.LookAt (target);
	}
}

