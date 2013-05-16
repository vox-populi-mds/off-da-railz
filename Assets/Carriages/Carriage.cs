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
	public GameObject 			m_PowerupOrWeapon;
	
	public Transform[] 			m_WheelTransforms;
	
	public float				m_StucturalIntegrity	= 100.0f;
	public float	 			m_SuspensionRange 		= 0.5f;
	public float 				m_SuspensionDamper 		= 0.0f;
	public float 				m_SuspensionSpring 		= 0.0f;
	
	public Vector3 				m_GroundDragMultiplier = new Vector3(2.0f, 5.0f, 1.0f);
	public Vector3 				m_AirDragMultiplier = new Vector3(0.0f, 0.0f, 1.0f);
	
	private bool 				m_IsOnGround = false;
	private	float				m_GroundedTime = 0.0f;
	
	private CarriageWheel[] 	m_Wheels;
	private float 				m_WheelRadius;
	private WheelFrictionCurve 	m_WheelFrictionCurve;
	
	private Transform			m_Train;
	private bool 				m_Dying = false;
	
	private ConnectionState		m_ConnectionState;
	
	private Vector3 			m_BackSplinePosition;
	private Vector3				m_FrontSplinePosition;	
	private Vector3				m_midPointSpinePosition;
	private Quaternion			m_SplineRotation;
	
	public enum ConnectionState
	{
		NOT_CONNECTED,
		CONNECTED_VIA_JOINT,
		CONNECTED_VIA_SPLINE,
		CONNECTION_AWAITING_JOINT,
	}
	
	// Use this for initialization
	void Start () 
	{
		SetupWheelColliders();
		
		Vector3 v3NewCenter = (transform.FindChild("FrontLatch").transform.localPosition + transform.FindChild("BackLatch").transform.localPosition) * 0.5f;
		rigidbody.centerOfMass = v3NewCenter;
			
		//DisableDebugWheelRendering();
	}
	
	// Update is called once per frame
	void Update() 
	{	
		//Mesh i;
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessWheelGraphics(RelativeVelocity);
		
		ProcessDebugInfo();
		
		if (GetComponent<MeshFilter>() != null)
		//i = GetComponent<MeshFilter>().mesh;
		
		if (!m_Dying)
		{
			//if (m_StucturalIntegrity < 50)
			{		
				m_Dying = true;

				GetComponent<MeshFilter>().mesh = Instantiate(Resources.Load ("train_boxwreck")) as Mesh;

			}
		}
	}
	
	void ProcessDebugInfo()
	{
		if(m_BackSplinePosition.magnitude == 0.0f || m_FrontSplinePosition.magnitude == 0.0f)
		{
			return;
		}
		
		Debug.DrawLine(rigidbody.worldCenterOfMass, m_midPointSpinePosition, Color.yellow);
	}

	
	void FixedUpdate()
	{	
		// Transform rigidbody velocity to local co-ordinate space
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessFriction(RelativeVelocity);
		
		ProcessDrag(RelativeVelocity);
		
		ProcessSplineAndJointForces();	
	}
	
	void ProcessSplineAndJointForces()
	{
		// Only process if there is a connection.
		if(m_ConnectionState == ConnectionState.NOT_CONNECTED)
		{
			return;
		}
		
		m_midPointSpinePosition = (m_BackSplinePosition + m_FrontSplinePosition) * 0.5f;
		
		// Move the carriage to the right position if it is close enough
		if(m_ConnectionState == ConnectionState.CONNECTED_VIA_JOINT)
		{
			Vector3 v3Force = m_midPointSpinePosition - rigidbody.worldCenterOfMass;
			v3Force.y = 0;
			
			rigidbody.AddForce(v3Force * rigidbody.mass * 200.0f, ForceMode.Force);
		}
		else if(m_ConnectionState  == ConnectionState.CONNECTED_VIA_SPLINE)
		{
			Vector3 v3Force = m_midPointSpinePosition - rigidbody.worldCenterOfMass;
				
			rigidbody.AddForce(v3Force * rigidbody.mass * 50.0f, ForceMode.Force);
			rigidbody.transform.rotation = m_SplineRotation;
			
			if(v3Force.magnitude < 1.0f)
			{
				m_ConnectionState = ConnectionState.CONNECTION_AWAITING_JOINT;
			}
		}
	}
	
	void DisableDebugWheelRendering()
	{
		foreach(Transform t in m_WheelTransforms)	// disable debug wheel rendering
		{
			MeshRenderer Mr = t.FindChild("Wheel").GetComponent<MeshRenderer>();
			Mr.enabled = false;
		}
	}
	
	public void SetTrain(Transform _train)
	{
		m_Train = _train;
	}
	
	void SetupWheelColliders()
	{
		m_Wheels = new CarriageWheel[m_WheelTransforms.Length];
		
		SetupWheelFrictionCurve();
		
		int wheelCount = 0;
		foreach(Transform t in m_WheelTransforms)
		{
			m_Wheels[wheelCount] = SetupWheel(t);
			wheelCount++;
		}
	}
	
	void Damage(float _damage)
	{
		m_StucturalIntegrity -= _damage;
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
			
			w.m_TireGraphic.Rotate(Vector3.right * (w.m_GroundSpeed.z / m_WheelRadius) * Time.deltaTime * Mathf.Rad2Deg);
		}
	}
	
	void ProcessFriction(Vector3 _RelativeVelocity)
	{	
		foreach(CarriageWheel w in m_Wheels)
		{
			w.m_Collider.sidewaysFriction = m_WheelFrictionCurve;
			w.m_Collider.forwardFriction = m_WheelFrictionCurve;
		}
	}
	
	void ProcessDrag(Vector3 _RelativeVelocity)
	{
		// Only apply the drag if the train is on the ground
		m_IsOnGround = false;
		m_GroundedTime += Time.deltaTime;
		foreach(CarriageWheel w in m_Wheels)
		{
			WheelCollider Wc = w.m_Collider;
			WheelHit Wh = new WheelHit();
			
			// First we get the velocity at the point where the wheel meets the ground, if the wheel is touching the ground
			if(Wc.GetGroundHit(out Wh))
			{	
				m_IsOnGround = true;
				m_GroundedTime = 0.0f;
				break;
			}
		}
		
		Vector3 DragMultiplier = m_GroundDragMultiplier;
		if(!m_IsOnGround) 
		{
			DragMultiplier = m_AirDragMultiplier;
		}
		
		Vector3 RelativeDrag = new Vector3(	-_RelativeVelocity.x * Mathf.Abs(_RelativeVelocity.x), 
											-_RelativeVelocity.y * Mathf.Abs(_RelativeVelocity.y), 
											-_RelativeVelocity.z * Mathf.Abs(_RelativeVelocity.z) );
		
		Vector3 Drag = Vector3.Scale(DragMultiplier, RelativeDrag);
		Drag.x *= 80 / _RelativeVelocity.magnitude;
		
		if(Mathf.Abs(_RelativeVelocity.x) < 5)
		{
			Drag.x = -_RelativeVelocity.x * DragMultiplier.x;
		}
			
		rigidbody.AddForce(transform.TransformDirection(Drag) * rigidbody.mass * Time.deltaTime);
	}
	
	public void SetBackSplinePosition(Vector3 _v3Position)
	{
		m_BackSplinePosition = _v3Position;
	}
	
	public Vector3 GetBackSplinePosition()
	{
		return(m_BackSplinePosition);
	}
	
	public void SetFrontSplinePostion(Vector3 _v3Position)
	{
		m_FrontSplinePosition = _v3Position;
	}
	
	public void SetSplineRotation(Quaternion _qRotation)
	{
		m_SplineRotation = _qRotation;
	}	
	
	public void SetConnectionState(ConnectionState _State)
	{
		m_ConnectionState = _State;
	}
	
	public ConnectionState GetConnectionState()
	{
		return(m_ConnectionState);
	}
	
		void OnCollisionEnter(Collision CollisionInfo){
		TrainCarriages PlayerTrainCarrages;
		FollowObject TrainFollowTarget;
		
		TrainFollowTarget = this.GetComponent<FollowObject>();
		
		if (CollisionInfo.gameObject.tag == "Train"){
			PlayerTrainCarrages = CollisionInfo.gameObject.GetComponent<TrainCarriages>();
			
			if (TrainFollowTarget.HasTarget()){
				DestoryTrain();
			}else{
				PlayerTrainCarrages.AddCarriage(this);
			}
		}
	}		
		
	void DestoryTrain(){
		this.renderer.enabled = false;
	}
	
}