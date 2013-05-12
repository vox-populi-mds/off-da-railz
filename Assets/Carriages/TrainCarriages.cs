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
	int GetNumberOfCarrages()
	{
		return (m_listCarriages.Count);
	}
	
	Transform GetActiveCarriage() 
	{
		return m_ActiveCarriage;
	}
	
	int AddCarriage(Transform _carriage) 
	{
		m_listCarriages.Add(_carriage);
		
		return(0);
	}
	
	void RemCarriage(Transform _carriage) 
	{
		int iCarNum = m_listCarriages.Count;
		
		if ((!m_listCarriages.Contains(_carriage)) || iCarNum == 0) 
		{
			// attempting to remove carriage that does not exist
			return;
		}
		
		int index = m_listCarriages.IndexOf(_carriage) ;
						
		if (_carriage == m_ActiveCarriage)
		{
			
			if ( index > 0 )
			{
					m_ActiveCarriage = m_listCarriages[index-1];			
			}
			else
			{
				m_ActiveCarriage = null;
			}
		}
				
		m_listCarriages.Remove(_carriage);
		
		Destroy (_carriage);
		
		iCarNum = m_listCarriages.Count;
		
		/*if (index >= iCarNum)
		{
			for (; index < m_listCarriages.Count
		}*/
	}
	
	void CreateNewWaypoint(Vector3 _Position, Quaternion _Rotation)
	{
		GameObject newWaypoint = GameObject.CreatePrimitive(PrimitiveType.Sphere);
		newWaypoint.transform.position = transform.FindChild("BackLatch").transform.position;
		newWaypoint.transform.rotation = transform.FindChild("BackLatch").transform.rotation;
		Destroy(newWaypoint.collider);
		
		m_listWaypoints.Add(newWaypoint.transform);
	}
	
	// Use this for initialization
	void Start() 
	{
		if(!GetComponent<Train>().IsMine())
		{
			return;
		}
		
		m_listCarriages = new List<Transform>();
		m_listWaypoints = new List<Transform>();
		m_ActiveCarriage = null;
		
		// Set up some test carriages
		SetupTestBoxcar();
	}
	
	// Update is called once per frame
	void Update() 
	{
		if(!GetComponent<Train>().IsMine())
		{
			return;
		}
		
		/*if (m_listCarriages.Count > 1) 
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				if (m_listCarriages.Count > m_ActiveCarriage + 1)
				{
					m_ActiveCarriage++;
				}			
				else
				{
					m_ActiveCarriage = 0;
				}
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (m_ActiveCarriage > 0)
				{
					m_ActiveCarriage--;
				}
				else
				{
					m_ActiveCarriage = m_listCarriages.Count-1;
				}
			}
		}*/
		
		// Make a new waypoint every 60 meters the train travels
		m_AccumulatedDistance += Vector3.Distance(transform.position, m_LastPosition);
		m_LastPosition = transform.position;
		
		if(m_AccumulatedDistance > 10.0f)
		{
			CreateNewWaypoint(transform.position, transform.rotation);
		
			Debug.Log(m_AccumulatedDistance);
			m_AccumulatedDistance = 0;
		}
		
		foreach(Transform t in m_listCarriages)
		{
			//t.GetComponent<Carriage>().SetForcePostion();
		}
	}
	
	void SetupTestBoxcar()
	{	
		Transform frontBodyTransform = transform;
		
		for(int i = 0; i < 1; ++i)
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
			//sjlXlow.spring = 10000.0f * rigidbody.mass;
			
			SoftJointLimit sjlXhigh = new SoftJointLimit();
			sjlXhigh.limit = m_CarriageAngularFreedom.x;
			//sjlXlow.spring = 10000.0f * rigidbody.mass;
			
			SoftJointLimit sjlY = new SoftJointLimit();
			sjlY.limit = m_CarriageAngularFreedom.y;
			//sjlY.spring = 10000.0f * rigidbody.mass;
			
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
			
			m_listCarriages.Add(CarriageGO.transform);
			
			frontBodyTransform = CarriageGO.transform;
		}
	}
	
	private List<Transform> 	m_listCarriages;
	private Transform			m_ActiveCarriage;
	
	float 						m_AccumulatedDistance;
	Vector3						m_LastPosition;
	List<Transform>				m_listWaypoints;
	
	public Transform		m_TrainBoxCarTransform;
	public Vector3			m_CarriageAngularFreedom = new Vector3(10.0f, 45.0f, 0.5f);
	public float 			m_CarriageMovementFreedom = 0.25f;
}
