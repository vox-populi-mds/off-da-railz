using UnityEngine;
using System.Collections;

public class CarriageWheel
{
	public WheelCollider 	m_Collider;
	public Transform 		m_WheelGraphic;
	public Transform 		m_TireGraphic;
	public Vector3 			m_WheelVelo  		= Vector3.zero;
	public Vector3 			m_GroundSpeed 		= Vector3.zero;
}

public class Carriage : MonoBehaviour 
{
	public GameObject m_PowerupOrWeapon;
	
	public Transform[] m_WheelTransforms;
	
	public float	 	m_SuspensionRange 		= 0.5f;
	public float 		m_SuspensionDamper 		= 0.0f;
	public float 		m_SuspensionSpring 		= 0.0f;
	
	private CarriageWheel[] 	m_Wheels;
	private float 				m_WheelRadius;
	private WheelFrictionCurve 	m_WheelFrictionCurve;
	
	// Use this for initialization
	void Start () 
	{
		SetupWheelColliders();
			
		//DisableDebugWheelRendering();
	}
	
	// Update is called once per frame
	void Update () 
	{
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessWheelGraphics(RelativeVelocity);
	}
	
	void FixedUpdate()
	{	
		// Transform rigidbody velocity to local co-ordinate space
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessFriction(RelativeVelocity);
	}
	
	void DisableDebugWheelRendering()
	{
		foreach(Transform t in m_WheelTransforms)	// disable debug wheel rendering
		{
			MeshRenderer Mr = t.FindChild("Wheel").GetComponent<MeshRenderer>();
			Mr.enabled = false;
		}
	}
	
	void SetupWheelColliders()
	{
		m_Wheels = new CarriageWheel[m_WheelTransforms.Length];
		
		SetupWheelFrictionCurve();
		
		int wheelCount = 0;
		foreach(Transform t in m_WheelTransforms)
		{
			
			Debug.Log("New Wheel");
			m_Wheels[wheelCount] = SetupWheel(t);
			wheelCount++;
		}
	}
	
	CarriageWheel SetupWheel(Transform _WheelTransform)
	{	
		GameObject Go = new GameObject(_WheelTransform.name + " Collider");
		Go.transform.position = _WheelTransform.position;
		Go.transform.parent = transform;
		Go.transform.rotation = _WheelTransform.rotation;
			
		WheelCollider Wc = Go.AddComponent(typeof(WheelCollider)) as WheelCollider;
		Wc.suspensionDistance = m_SuspensionRange;
		JointSpring Js = Wc.suspensionSpring;
		
		Js.spring = m_SuspensionSpring;
			
		Js.damper = m_SuspensionDamper;
		Wc.suspensionSpring = Js;
			
		CarriageWheel WheelObj = new CarriageWheel(); 
		WheelObj.m_Collider = Wc;
		Wc.sidewaysFriction = m_WheelFrictionCurve;
		WheelObj.m_WheelGraphic = _WheelTransform;
		WheelObj.m_TireGraphic = _WheelTransform.GetComponentsInChildren<Transform>()[1];
		
		m_WheelRadius = WheelObj.m_TireGraphic.renderer.bounds.size.y / 2;	
		WheelObj.m_Collider.radius = m_WheelRadius;
			
		return WheelObj;
	}
	
	void SetupWheelFrictionCurve()
	{
		m_WheelFrictionCurve = new WheelFrictionCurve();
		m_WheelFrictionCurve.extremumSlip = 1;
		m_WheelFrictionCurve.extremumValue = 50;
		m_WheelFrictionCurve.asymptoteSlip = 2;
		m_WheelFrictionCurve.asymptoteValue = 25;
		m_WheelFrictionCurve.stiffness = 1;
	}
	
	void ProcessWheelGraphics(Vector3 _RelativeVelocity)
	{		
		foreach(CarriageWheel w in m_Wheels)
		{
			WheelCollider Wc = w.m_Collider;
			WheelHit Wh = new WheelHit();
			
			// First we get the velocity at the point where the wheel meets the ground, if the wheel is touching the ground
			if(Wc.GetGroundHit(out Wh))
			{	
				//TODO: FIX THIS: w.m_WheelGraphic.localPosition = Wc.transform.up * (m_WheelRadius + Wc.transform.InverseTransformPoint(Wh.point).y);
				w.m_WheelVelo = rigidbody.GetPointVelocity(Wh.point);
				w.m_GroundSpeed = w.m_WheelGraphic.InverseTransformDirection(w.m_WheelVelo);
			}
			else
			{
				// If the wheel is not touching the ground we set the position of the wheel graphics to
				// the wheel's transform position + the range of the suspension.
				w.m_WheelGraphic.position = Wc.transform.position + (-Wc.transform.up * m_SuspensionRange);
			}
		}
	}
	
	void ProcessFriction(Vector3 _RelativeVelocity)
	{
		float SqrVel = _RelativeVelocity.x * _RelativeVelocity.x;
		
		// Add extra sideways friction based on the car's turning velocity to avoid slipping
		m_WheelFrictionCurve.extremumValue = Mathf.Clamp(300 - SqrVel, 0, 300);
		m_WheelFrictionCurve.asymptoteValue = Mathf.Clamp(150 - (SqrVel / 2), 0, 150);
			
		foreach(CarriageWheel w in m_Wheels)
		{
			w.m_Collider.sidewaysFriction = m_WheelFrictionCurve;
			w.m_Collider.forwardFriction = m_WheelFrictionCurve;
		}
	}
}
