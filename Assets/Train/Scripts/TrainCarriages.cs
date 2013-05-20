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
	void Awake()
	{
		m_listCarriages = new List<Carriage>();	
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
			
			_carriage.SetTrain(transform);
			
			return true;
		}
		// over capacity!
		return false;
	}
	
	public void RemAllCarriages()
	{
		if(m_listCarriages.Count != 0)
		{
			RemCarriage(m_listCarriages[0]);
		}
	}
	
	public void RemCarriage(Carriage _carriage) 
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
		
		_carriage.SetConnectionState(Carriage.ConnectionState.NOT_CONNECTED);
		m_listCarriages.RemoveAt(remCarIndex);
		
		//_carriage.Explode();
		
		//Destroy (_carriage);
				
		while (remCarIndex < m_listCarriages.Count) // loop through, remove all of them from the list and make them 'loose'
		{
			m_listCarriages[remCarIndex].SetConnectionState(Carriage.ConnectionState.NOT_CONNECTED);
			m_listCarriages.RemoveAt(remCarIndex);
		}
	}
	
	void CreateNewWaypoint(Vector3 _Position)
	{
		GameObject newWaypoint = new GameObject();
		newWaypoint.transform.rotation = m_LatchTransform.rotation;
		newWaypoint.transform.position = _Position;
		
		m_listWaypoints.Add(newWaypoint.transform);
		
		m_LastPosition = newWaypoint.transform.position;
		
		m_DistanceFromLastWaypoint = 0;
	}
	
	// Use this for initialization
	void Start() 
	{
		if(!Network.isServer)
		{
			return;
		}
		
		m_listCarriagesAwaitingConnection = new List<Carriage>();
		m_listWaypoints = new List<Transform>();
		m_ActiveCarriage = null;
		m_LatchTransform = transform.FindChild("BackLatch").transform;
		m_CarriageLength = 30.0f;
		
		SetupInitialWaypoints();
				
	}
	
	void SetupInitialWaypoints()
	{
		for(int i = m_ExtraWaypoints - 1; i > 0; --i)
		{
			Vector3 posOffset = m_LatchTransform.rotation * new Vector3(0.0f, 0.0f, -m_CarriageLength) * i;
			Vector3 posWaypoint = m_LatchTransform.position + posOffset;
			CreateNewWaypoint(posWaypoint);
		}
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(!Network.isServer)
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
		
		ProcessNewCarriagesConnection();
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
		Vector3 v3Position = (m_LatchTransform.position + (m_LatchTransform.rotation * new Vector3(0.0f, 0.0f, m_CarriageLength)));
		Vector3 v3Displacement = m_LastPosition - v3Position;
		
		m_DistanceFromLastWaypoint = v3Displacement.magnitude;
		if(m_DistanceFromLastWaypoint > m_CarriageLength)
		{
			Vector3 Direction = -v3Displacement;
			Direction.Normalize();
			
			CreateNewWaypoint(m_LastPosition + Direction * m_CarriageLength);
			++m_NumWaypointWhileGrounded;
		}
		
		if(!GetComponent<Train>().IsOnGround())
		{
			m_NumWaypointWhileGrounded = 0;
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
		
		// Process the force positions for the carriages.
		for(int i = 0; i < m_listCarriages.Count; ++i)
		{
			Carriage carriageScript = m_listCarriages[i].GetComponent<Carriage>();
			
			// Tell this carriage not to follow the spline.
			if(m_NumWaypointWhileGrounded < i + 2)
			{
				carriageScript.SetSplineFollowState(false);
			}
			else
			{
				carriageScript.SetSplineFollowState(true);
			}
			
			float stepPerWaypoint = 1.0f / (m_listWaypoints.Count - 1);
			
			float timeBack = 1.0f - (((1.0f + ((i + 1) * 1.0f)) - (m_DistanceFromLastWaypoint/m_CarriageLength)) * stepPerWaypoint);
			Vector3 carriageBackPos = interp.GetHermiteAtTime(timeBack);
			
			if(i != 0)
			{
				carriageScript.SetFrontSplinePostion(m_listCarriages[i - 1].GetComponent<Carriage>().GetBackSplinePosition());
			}
			else
			{
				carriageScript.SetFrontSplinePostion(m_LatchTransform.position);
			}
			
			carriageScript.SetBackSplinePosition(carriageBackPos);
			
			float times = 1.0f - (((1.0f + ((i + 0.5f) * 1.0f)) - (m_DistanceFromLastWaypoint/m_CarriageLength)) * stepPerWaypoint);
			Quaternion carriageRot = interp.GetRotationAtTime(times);
			
			carriageScript.SetSplineRotation(carriageRot);
			
			// Set the carriage to connected via spline and this carriage was just hit and add to the waiting list for connection
			if(m_listCarriages[i].GetConnectionState() == Carriage.ConnectionState.NOT_CONNECTED)
			{
				m_listCarriagesAwaitingConnection.Add(m_listCarriages[i]);
				m_listCarriages[i].SetConnectionState(Carriage.ConnectionState.CONNECTION_AWAITING_FIND_JOINT);
				
				m_listCarriages[i].AddExtraCollisionForce();
				
				if(i != 0)
				{
					m_listCarriages[i].SetFrontBackLatchTransform(m_listCarriages[i - 1].transform.FindChild("BackLatch"));
				}
				else
				{
					m_listCarriages[i].SetFrontBackLatchTransform(m_LatchTransform);
				}
			}
		}
	}
	
	void ProcessNewCarriagesConnection()
	{
		List<int> CleanupCarriages = new List<int>();
		for(int i = 0; i < m_listCarriagesAwaitingConnection.Count; ++i)
		{
			Carriage c = m_listCarriagesAwaitingConnection[i];
			if(c.GetConnectionState() == Carriage.ConnectionState.CONNECTION_AWAITING_JOINT)
			{
				Transform transformFront;
				
				int carriageIndex = m_listCarriages.IndexOf(c);
				if(carriageIndex != 0)
				{
					transformFront = m_listCarriages[carriageIndex - 1].transform;
				}
				else
				{
					transformFront = transform;
				}

				CreateJointBetweenCarriages(c.transform, transformFront);
				CleanupCarriages.Add(i);
			}
		}
		
		// Clean up the list for carriages that are now joined.
		for(int i = 0; i < CleanupCarriages.Count; ++i)
		{
			m_listCarriagesAwaitingConnection.RemoveAt(CleanupCarriages[i]);
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
	
	void CreateJointBetweenCarriages(Transform _ConnectFrom, Transform _ConnectTo)
	{
		Audio.GetInstance.Play(m_connectionNoise, _ConnectTo, 100, false);
		
		// Move the back carriage to the right place.
		Vector3 backLatchFront = _ConnectTo.FindChild("BackLatch").transform.position;
		Vector3 newBackCarriagePosition = backLatchFront - _ConnectTo.rotation * _ConnectFrom.FindChild("FrontLatch").localPosition;
		
		_ConnectFrom.position = newBackCarriagePosition;
		_ConnectFrom.rotation = _ConnectTo.rotation;
		
		ConfigurableJoint joint = _ConnectFrom.gameObject.AddComponent<ConfigurableJoint>();
		joint.connectedBody = _ConnectTo.rigidbody;
		
		SoftJointLimit sjlXlow = new SoftJointLimit();
		sjlXlow.limit = -m_CarriageAngularFreedom.x;
		sjlXlow.spring = 1000.0f * rigidbody.mass;
		
		SoftJointLimit sjlXhigh = new SoftJointLimit();
		sjlXhigh.limit = m_CarriageAngularFreedom.x;
		sjlXlow.spring = 1000.0f * rigidbody.mass;
		
		SoftJointLimit sjlY = new SoftJointLimit();
		sjlY.limit = m_CarriageAngularFreedom.y;
		sjlY.spring = 1000.0f * rigidbody.mass;
		
		SoftJointLimit sjlZ = new SoftJointLimit();
		sjlZ.limit = m_CarriageAngularFreedom.z;
		sjlZ.spring = 1000.0f * rigidbody.mass;
		
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
		
		joint.anchor = _ConnectFrom.transform.FindChild("FrontLatch").transform.localPosition;
		
		_ConnectFrom.GetComponent<Carriage>().SetConnectionState(Carriage.ConnectionState.CONNECTED_JOINT);
	}
	
	public int GetNumCarriages()
	{
		return(m_listCarriages.Count);
	}
	
	private List<Carriage> 		m_listCarriages;
	private List<Carriage> 		m_listCarriagesAwaitingConnection;
	private Carriage			m_ActiveCarriage;
	private Transform			m_LatchTransform;
	
	SplineInterpolator			m_SplineInterp;
	float 						m_DistanceFromLastWaypoint;
	float 						m_CarriageLength;
	
	Vector3						m_LastPosition;
	List<Transform>				m_listWaypoints;
	int 						m_ExtraWaypoints = 4;
	int 						m_NumWaypointWhileGrounded;
	
	public Vector3			m_CarriageAngularFreedom = new Vector3(10.0f, 45.0f, 0.5f);
	public float 			m_CarriageMovementFreedom = 0.25f;
	public const uint		MAX_CARRIAGES = 10;
	
	public AudioClip 		m_connectionNoise;
}
