using UnityEngine;
using System.Collections;
using System.Collections.Generic;
/*
public enum ECarriageType {
	Rocket,
	Shotgun,
	Gatling,
};*/



public class TrainCarriages : MonoBehaviour {
	private List<Carriage> 	m_listCarriages;
	private Carriage		m_ActiveCarriage;
	public Transform		m_TrainBoxCarTransform;
	
	int GetNumberOfCarrages(){
		return (m_listCarriages.Count);
	}
	
	Carriage GetActiveCarriage() 
	{
		return m_ActiveCarriage;
	}
	
	int AddCarriage(Carriage _carriage) {
		
		m_listCarriages.Add(_carriage);
		
		return(0);
	}
	
	void RemCarriage(Carriage _carriage) 
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
	
	// Use this for initialization
	void Start() {
		m_listCarriages = new List<Carriage>();
		m_ActiveCarriage = null;
		
		SetupTestBoxcar();
	}
	
	// Update is called once per frame
	void Update() {
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
	}
	
	void SetupTestBoxcar()
	{
		Vector3 vPositionOffset = new Vector3(0, 0, -33.0f);
		
		vPositionOffset = transform.rotation * vPositionOffset;
		Vector3 vPosition = transform.position + vPositionOffset;
		
		Object BoxObj =  Network.Instantiate(m_TrainBoxCarTransform, vPosition, transform.rotation, 0);
			
		// We're just playing a single player game.
		if(!BoxObj)
		{
			BoxObj = Instantiate(m_TrainBoxCarTransform, vPosition, transform.rotation);
		}
		
		GameObject networkBoxGO = ((Transform) BoxObj).gameObject;
		
		// Setup the follow script
		//FollowObject followScript = networkBoxGO.GetComponent<FollowObject>();
		//followScript.target = LatchTransform;
		//followScript.distance = 1;
		
		HingeJoint joint = networkBoxGO.AddComponent<HingeJoint>();
		joint.connectedBody = GetComponent<Rigidbody>();
		
		JointLimits jl = new JointLimits();
		jl.min = -45;
		jl.max = 45;
		joint.limits = jl;
		
		joint.useLimits = true;
		
		joint.axis = Vector3.up;
		joint.anchor = networkBoxGO.transform.FindChild("FrontLatch").transform.localPosition;
		
		vPositionOffset.z = -30;
		vPositionOffset = networkBoxGO.transform.rotation * vPositionOffset;
		vPosition = networkBoxGO.transform.position + vPositionOffset;
		
		Object BoxObj2 =  Network.Instantiate(m_TrainBoxCarTransform, vPosition, transform.rotation, 0);
			
		// We're just playing a single player game.
		if(!BoxObj2)
		{
			BoxObj2 = Instantiate(m_TrainBoxCarTransform, vPosition, transform.rotation);
		}
		
		GameObject networkBoxGO2 = ((Transform) BoxObj2).gameObject;
		
		// Setup the follow script
		//FollowObject followScript = networkBoxGO.GetComponent<FollowObject>();
		//followScript.target = LatchTransform;
		//followScript.distance = 1;
		
		HingeJoint joint2 = networkBoxGO2.AddComponent<HingeJoint>();
		joint2.connectedBody = networkBoxGO.GetComponent<Rigidbody>();
		
		joint2.limits = jl;
		
		joint2.useLimits = true;
		
		joint2.axis = Vector3.up;
		joint2.anchor = networkBoxGO2.transform.FindChild("FrontLatch").transform.localPosition;
	}
		
}
