/* ---------------------------------------------------------------------------
** This software is in the public domain, furnished "as is", without technical
** support, and with no warranty, express or implied, as to its usefulness for
** any purpose.
**
** PositionSmoothFollow.h
** Modified SmoothFollow.js to C# and removed mouselook
**
** Author: Kit Chan
** -------------------------------------------------------------------------*/

using UnityEngine;
using System.Collections;

public class PositionSmoothFollow : MonoBehaviour {
	public Transform m_target = null;
	public float m_distance = 10.0f;
	public float m_height = 5.0f;
	public float m_heightDamping = 2.0f;
	public float m_rotationDamping = 3.0f;
	public enum Emode {
		FPS,
		Top_Down,
		test
	};
	public Emode m_mode;
	
	// Use this for initialization
	void Start () 
	{	
		if(!m_target)
		{
			m_target = GameObject.Find("CenterOfMass").transform;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		// no target! Bail!
		if (!m_target)
			return;
				
		Vector3 CamPos = transform.position;
		Vector3 CamRot = transform.eulerAngles;
		
		// Calculate the current rotation angles
		float wantedRotationAngle = m_target.eulerAngles.y;
		float wantedHeight = m_target.position.y + m_height;
			
		float currentRotationAngle = CamRot.y;
		float currentHeight = CamPos.y;
		
		// Damp the rotation around the y-axis
		currentRotationAngle = Mathf.LerpAngle (currentRotationAngle, wantedRotationAngle, m_rotationDamping * Time.deltaTime);
	
		// Damp the height
		currentHeight = Mathf.Lerp (currentHeight, wantedHeight, m_heightDamping * Time.deltaTime);
		
		CamRot.y = currentRotationAngle;
		
		// Convert the angle into a rotation
		var currentRotation = Quaternion.Euler (0, currentRotationAngle, 0);
				
		// Set the position of the camera on the x-z plane to:
		// distance meters behind the target
		CamPos = m_target.position;
		CamPos -= currentRotation * Vector3.forward * m_distance;
	
		CamPos.y = currentHeight;
		// Set the height of the camera
		this.transform.position = CamPos;
		this.transform.eulerAngles = CamRot;
	}
}
