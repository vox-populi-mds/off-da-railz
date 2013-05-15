using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public enum ECarriageType {
	Rocket,
	Shotgun,
	Gatling,
};*/



public class TrainCarriages : MonoBehaviour 
{
	int GetNumberOfCarriages()
	{
		return (m_listCarriages.Count);
	}
	
	Carriage GetActiveCarriage() 
	{
		return m_ActiveCarriage;
	}
	
	public bool AddCarriage(Carriage _carriage) 
	{
		if (m_listCarriages.Count < MAX_CARRIAGES)
		{
			m_listCarriages.Add(_carriage);
			
			_carriage.GetComponent<FollowObject>().target = m_listCarriages[m_listCarriages.Count].transform.FindChild("FrontLatch").transform;			
			
			_carriage.SetTrain(transform);
			
			return true;
		}
		// over capacity!
		return false;
	}
	
	void RemCarriage(Carriage _carriage) 
	{
		if ((!m_listCarriages.Contains(_carriage)) || m_listCarriages.Count == 0) 
		{
			// attempting to remove carriage that does not exist
			return;
		}
		
		int remCarIndex = m_listCarriages.IndexOf(_carriage); // index of carriage to remove
						
		
		// if attempting to remove the active carriage
		if (_carriage == m_ActiveCarriage)
		{			
			if ( remCarIndex > 0 ) // if removed carriage is not first carriage
			{
					m_ActiveCarriage = m_listCarriages[remCarIndex-1];	// move active carriage back 1 index
			}
			else
			{
				m_ActiveCarriage = null; // otherwise, there are no more carriages
			}
		}
				
		m_listCarriages.Remove(_carriage);
		
		//_carriage.Explode();
		
		Destroy (_carriage);
				
		if (remCarIndex >= m_listCarriages.Count) // if there are carriages 'after' the attacked carriage
		{
			while (remCarIndex < m_listCarriages.Count) // loop through, remove all of them from the list and make them 'loose'
			{
				m_listCarriages[remCarIndex].GetComponent<FollowObject>().target = null;
				m_listCarriages.RemoveAt(remCarIndex);
			}
		}
	}
	
	void CreateNewWaypoint(Vector3 _Position)
	{
		GameObject newWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		newWaypoint.transform.rotation = m_LatchTransform.rotation;
		newWaypoint.transform.position = _Position;
		Destroy(newWaypoint.collider);
		
		m_listWaypoints.Add(newWaypoint.transform);
		
		m_LastPosition = newWaypoint.transform.position;
		
		m_DistanceFromLastWaypoint = 0;
	}
	
	// Use this for initialization
	void Start() 
	{
		if(!GetComponent<Train>().IsMine())
		{
			return;
		}
		
		m_listCarriages = new List<Carriage>();
		m_listWaypoints = new List<Transform>();
		m_ActiveCarriage = null;
		m_LatchTransform = transform.FindChild("BackLatch").transform;
		m_CarriageLength = 30.0f;
		
		SetupTestBoxcars();
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(!GetComponent<Train>().IsMine())
		{
			return;
		}
		
		if (m_listCarriages.Count > 1) 
		{
			if (Input.GetKeyDown(KeyCode.E) || Input.GetAxis("Mouse ScrollWheel")>0)
			{
				int index = m_listCarriages.IndexOf(m_ActiveCarriage);
				
				if (m_listCarriages.Count > index + 1)
				{
					m_ActiveCarriage = m_listCarriages[index+1];
				}			
				else
				{
					m_ActiveCarriage = m_listCarriages[0];
				}
			} else
			if (Input.GetKeyDown(KeyCode.Q) || Input.GetAxis("Mouse ScrollWheel")<0)
			{
				int index = m_listCarriages.IndexOf(m_ActiveCarriage);
				if (index > 0)
				{
					m_ActiveCarriage = m_listCarriages[index-1];
				}
				else
				{
					m_ActiveCarriage = m_listCarriages[m_listCarriages.Count-1];
				}
			}
		}
		
		CleanOldWaypoints();
		
		ProcessWaypointCreation();
	
		ProcessCarriagesSpline();
	}
	
	void CleanOldWaypoints()
	{
		// Keep a few extra on incase a new carriage is added.
		if(m_listWaypoints.Count > m_listCarriages.Count + 5)
		{
			Destroy(m_listWaypoints[0].gameObject);
			
			m_listWaypoints.RemoveAt(0);
		}
	}
	
	void ProcessWaypointCreation()
	{
		// Make a new waypoint every carriage length in meters the train travels.
		Vector3 v3Position = (m_LatchTransform.position + (m_LatchTransform.rotation * new Vector3(0.0f, 0.0f, 30.0f)));
		Vector3 v3Displacement = m_LastPosition - v3Position;
		
		m_DistanceFromLastWaypoint = v3Displacement.magnitude;
		if(m_DistanceFromLastWaypoint > m_CarriageLength)
		{
			Vector3 Direction = -v3Displacement;
			Direction.Normalize();
			
			CreateNewWaypoint(m_LastPosition + Direction * 30.0f);
		}
	}
	
	void ProcessCarriagesSpline()
	{
		GameObject SplineInterpolator = new GameObject();
		SplineInterpolator interp = (SplineInterpolator)SplineInterpolator.AddComponent(typeof(SplineInterpolator)); 
		SetupSplineInterpolator(interp, m_listWaypoints.ToArray());
		interp.StartInterpolation(null, null, null, false, eWrapMode.ONCE);
		
		Vector3 prevPos = m_listWaypoints[0].position;
		for (int c = 1; c != 100; ++c)
		{
			float currTime = c * 1.0f / 100.0f;
			Vector3 currPos = interp.GetHermiteAtTime(currTime);
			float mag = (currPos-prevPos).magnitude * 2;
			Color color = new Color(mag, 0, 0, 1);
			Debug.DrawLine(prevPos, currPos, color);
			
			prevPos = currPos;	
		}
		
		DestroyImmediate(SplineInterpolator);
		
		for(int i = 0; i < m_listCarriages.Count; ++i)
		{
			float stepPerWaypoint = 1.0f / (m_listWaypoints.Count - 1);
			
			float timeBack = 1.0f - (((1.0f + ((i + 1) * 1.0f)) - (m_DistanceFromLastWaypoint/m_CarriageLength)) * stepPerWaypoint);
			Vector3 carriageBackPos = interp.GetHermiteAtTime(timeBack);
			
			Carriage carriageScript = m_listCarriages[i].GetComponent<Carriage>();
			
			if(i != 0)
			{
				carriageScript.SetFrontSplinePostion(m_listCarriages[i - 1].GetComponent<Carriage>().GetBackSplinePosition());
			}
			else
			{
				carriageScript.SetFrontSplinePostion(m_LatchTransform.position);
			}
			
			carriageScript.SetBackSplinePosition(carriageBackPos);
			
			//float times = 1.0f - (((1.0f + ((i + 0.5f) * 1.0f)) - (m_DistanceFromLastWaypoint/CarriageLength)) * stepPerWaypoint);
			//Quaternion carriageRot = interp.GetRotationAtTime(times);
			
			//carriageScript.SetSplineRotation(carriageRot);
		}
	}
	
	void SetupSplineInterpolator(SplineInterpolator interp, Transform[] trans)
	{
		interp.Reset();

		float stepPerWaypoint = 1.0f / (trans.Length - 1);

		int c;
		for (c = 0; c < trans.Length; c++)
		{
			interp.AddPoint("Waypoint", trans[c].position, trans[c].rotation, stepPerWaypoint * c, 0.0f, new Vector2(0, 1));
		}
	}
	
	void SetupTestBoxcars()
	{	
		Transform frontBodyTransform = transform;
		
		for(int i = 0; i < 10; ++i)
		{
			Object BoxObj = new Object();
			if(Network.isClient || Network.isServer)
			{
				BoxObj = Network.Instantiate(m_TrainBoxCarTransform, Vector3.zero, Quaternion.identity, 0);
			}	
			else 
			{
				BoxObj = Instantiate(m_TrainBoxCarTransform);
			}
			
			GameObject CarriageGO = ((Transform) BoxObj).gameObject;
			
			Vector3 vPosition = frontBodyTransform.FindChild("BackLatch").transform.position - 
								(frontBodyTransform.rotation * CarriageGO.transform.FindChild("FrontLatch").transform.localPosition);
			
			CarriageGO.transform.position = vPosition;
			CarriageGO.transform.rotation = transform.rotation;
			
			ConfigurableJoint joint = CarriageGO.AddComponent<ConfigurableJoint>();
			joint.connectedBody = frontBodyTransform.rigidbody;
			
			SoftJointLimit sjlXlow = new SoftJointLimit();
			sjlXlow.limit = -m_CarriageAngularFreedom.x;
			//sjlXlow.spring = 1000.0f * rigidbody.mass;
			
			SoftJointLimit sjlXhigh = new SoftJointLimit();
			sjlXhigh.limit = m_CarriageAngularFreedom.x;
			//sjlXlow.spring = 1000.0f * rigidbody.mass;
			
			SoftJointLimit sjlY = new SoftJointLimit();
			sjlY.limit = m_CarriageAngularFreedom.y;
			//sjlY.spring = 1000.0f * rigidbody.mass;
			
			SoftJointLimit sjlZ = new SoftJointLimit();
			sjlZ.limit = m_CarriageAngularFreedom.z;
			
			SoftJointLimit sjlMotion = new SoftJointLimit();
			sjlMotion.limit = m_CarriageMovementFreedom;
			
			joint.lowAngularXLimit = sjlXlow;
			joint.highAngularXLimit = sjlXhigh;
			joint.angularYLimit = sjlY;
			joint.angularZLimit = sjlZ;
			joint.linearLimit = sjlMotion;
			
			joint.xMotion = ConfigurableJointMotion.Locked;
			joint.yMotion = ConfigurableJointMotion.Limited;
			joint.zMotion = ConfigurableJointMotion.Locked;
			
			joint.angularZMotion = ConfigurableJointMotion.Limited;
			joint.angularYMotion = ConfigurableJointMotion.Limited;
			joint.angularXMotion = ConfigurableJointMotion.Limited;
			
			joint.anchor = CarriageGO.transform.FindChild("FrontLatch").transform.localPosition;
			
			//m_listCarriages.Add(CarriageGO.transform);
			
			frontBodyTransform = CarriageGO.transform;
		}
		
		// Create the required waypoints
		for(int i = m_listCarriages.Count - 1; i >= 0; --i)
		{
			// Create the first waypoint required on the end of the last carriage.
			if(i == m_listCarriages.Count - 1)
			{
				Vector3 lastWaypoint = m_listCarriages[m_listCarriages.Count - 1].transform.FindChild("BackLatch").transform.position;
				CreateNewWaypoint(lastWaypoint);
			}
			
			CreateNewWaypoint(m_listCarriages[i].transform.FindChild("FrontLatch").transform.position);
		}
	}
	
	private List<Carriage> 		m_listCarriages;
	private Carriage			m_ActiveCarriage;
	private Transform			m_LatchTransform;
	
	SplineInterpolator			m_SplineInterp;
	float 						m_DistanceFromLastWaypoint;
	float 						m_CarriageLength;
	
	Vector3						m_LastPosition;
	List<Transform>				m_listWaypoints;
	
	public Transform		m_TrainBoxCarTransform;
	public Vector3			m_CarriageAngularFreedom = new Vector3(10.0f, 45.0f, 0.5f);
	public float 			m_CarriageMovementFreedom = 0.25f;
	public const uint		MAX_CARRIAGES = 10;
}
