using UnityEngine;
using System.Collections;
using OffTheRailz;

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
	public enum ConnectionState
	{
		NOT_CONNECTED,
		CONNECTED_JOINT,
		CONNECTION_FIND_JOINT,
		CONNECTION_AWAITING_FIND_JOINT,
		CONNECTION_AWAITING_JOINT,
	}
	
	// Use this for initialization
	void Start () 
	{
		SetupWheelColliders();
		
		Vector3 v3NewCenter = (transform.FindChild("FrontLatch").transform.localPosition + transform.FindChild("BackLatch").transform.localPosition) * 0.5f;
		rigidbody.centerOfMass = v3NewCenter;
		m_InitAngularDrag = rigidbody.angularDrag;
			
		// test the weapons
		//m_PowerupOrWeapon = Instantiate(Resources.LoadAssetAtPath("Assets/Weapons/Shotgun/Shotgun.prefab", typeof(GameObject))) as GameObject;
		//DisableDebugWheelRendering();
		m_Train = null;
	}
	
	// Update is called once per frame
	void Update() 
	{	
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessWheelGraphics(RelativeVelocity);
		
		if(Network.isServer)
		{
			ProcessDebugInfo();
			
			/*GetComponent<Health>().SetDamage(Time.deltaTime);
			
			if (!m_Dying)
			{
				if (GetComponent<Health>().GetHealth() < 50)
				{		
	
					Dying();
	
				}
			}
			if (GetComponent<Health>().GetHealth() <= 0)
			{
				DestroyTrain();
			}
			*/
		}
	}
	
	void ProcessDebugInfo()
	{
		Color DrawColor = Color.green;
		if(!m_FollowSpline)
		{
			DrawColor = Color.yellow;
		}
		
		if(m_ConnectionState == ConnectionState.CONNECTED_JOINT)
		{
			Debug.DrawLine(rigidbody.worldCenterOfMass, m_midPointSpinePosition, DrawColor);
		}
		else if(m_ConnectionState == ConnectionState.CONNECTION_FIND_JOINT)
		{
			Vector3 v3ToBodyPosition = m_FrontBackLatchTransform.position + m_FrontBackLatchTransform.rotation * transform.FindChild("BackLatch").transform.localPosition;
			Debug.DrawLine(rigidbody.worldCenterOfMass, v3ToBodyPosition, Color.cyan);
		}	
	}

	
	void FixedUpdate()
	{	
		if(!Network.isServer)
		{
			return;
		}
		
		// Transform rigidbody velocity to local co-ordinate space
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessFriction(RelativeVelocity);
		
		ProcessDrag(RelativeVelocity);
		
		ProcessSplineAndJointForces();	
	}
	
	void ProcessSplineAndJointForces()
	{
		// Enable values as a default
		rigidbody.angularDrag = m_InitAngularDrag;
		
		// Only process if there is a connection.
		if(m_ConnectionState == ConnectionState.NOT_CONNECTED)
		{
			m_TimeSinceCollision = 0.0f;
			return;
		}
		
		// Calculate the mid position for the carriage to be on the spline.
		m_midPointSpinePosition = (m_BackSplinePosition + m_FrontSplinePosition) * 0.5f;
		
		// Move the carriage towards its desired position on the spline.
		if(m_ConnectionState == ConnectionState.CONNECTED_JOINT)
		{
			if(!m_FollowSpline)
			{
				return;
			}
			
			Vector3 v3Force = (m_midPointSpinePosition - rigidbody.worldCenterOfMass) * rigidbody.mass * 20.0f;
			
			rigidbody.AddForce(v3Force, ForceMode.Force);

		}
		// Move the carriage towards the desired spline position and rotation so that it can get close enough to the joint connection.
		else if(m_ConnectionState == ConnectionState.CONNECTION_FIND_JOINT)
		{
			m_TimeSinceCollision += Time.deltaTime;
			Vector3 v3ToBodyPosition = m_FrontBackLatchTransform.position + m_FrontBackLatchTransform.rotation * transform.FindChild("BackLatch").transform.localPosition;
			
			Vector3 v3Distance = v3ToBodyPosition - rigidbody.worldCenterOfMass;
			float fDistance = v3Distance.magnitude;
			float fTrainSpeed = m_Train.GetComponent<Train>().GetSpeed();
				
			rigidbody.velocity = (v3Distance.normalized * (m_InitDistanceToLatch / 1.0f + fTrainSpeed));
			
			Vector3 v3CurrentLook = rigidbody.transform.rotation * Vector3.forward;
			Vector3 v3ToLook = m_FrontBackLatchTransform.rotation * Vector3.forward;
			
			Vector3 v3CurrentUp = rigidbody.transform.rotation * Vector3.up;
			Vector3 v3ToUp = m_FrontBackLatchTransform.rotation * Vector3.up;
			
			Vector3 v3CurrentRight = rigidbody.transform.rotation * Vector3.right;
			Vector3 v3ToRight = m_FrontBackLatchTransform.rotation * Vector3.right;
			
			Vector3 X = Vector3.Cross(v3CurrentLook.normalized, v3ToLook.normalized);
			float fThetaX = Mathf.Asin(X.magnitude);
			Vector3 WX = X.normalized * fThetaX * Time.fixedDeltaTime;
						
			Vector3 Y = Vector3.Cross(v3CurrentUp.normalized, v3ToUp.normalized);
			float fThetaY = Mathf.Asin(Y.magnitude);
			Vector3 WY = Y.normalized * fThetaY * Time.fixedDeltaTime;
			
			Vector3 Z = Vector3.Cross(v3CurrentRight.normalized, v3ToRight.normalized);
			float fThetaZ = Mathf.Asin(Z.magnitude);
			Vector3 WZ = Z.normalized * fThetaZ * Time.fixedDeltaTime;
			
			Vector3 W = (WX + WY + WZ) * rigidbody.mass * 100.0f * m_InitDistanceToLatch/fDistance;
		
			Quaternion q = rigidbody.transform.rotation * rigidbody.inertiaTensorRotation;
			Vector3 T = q * Vector3.Scale(rigidbody.inertiaTensor, (Quaternion.Inverse(q) * W));
			
			rigidbody.AddTorque(T, ForceMode.Force);

			// Set the connection state to wait for a joint connection from the train carriage if close enough.
			float fLatchDistance = Vector3.Distance(m_FrontBackLatchTransform.position, transform.FindChild("FrontLatch").transform.position);
			if(fLatchDistance < 5.0f)
			{
				m_ConnectionState = ConnectionState.CONNECTION_AWAITING_JOINT;
			}
		}
		else if(m_ConnectionState == ConnectionState.CONNECTION_AWAITING_FIND_JOINT)
		{
			rigidbody.angularDrag = 0;
			m_TimeSinceCollision += Time.deltaTime;
			
			if(m_TimeSinceCollision > 2.0f)
			{
				Vector3 v3ToBodyPosition = m_FrontBackLatchTransform.position + m_FrontBackLatchTransform.rotation * transform.FindChild("FrontLatch").transform.localPosition;
				m_InitDistanceToLatch = Vector3.Distance(v3ToBodyPosition, rigidbody.worldCenterOfMass);
				
				m_ConnectionState = ConnectionState.CONNECTION_FIND_JOINT;
			}
		}
		else if(m_ConnectionState == ConnectionState.CONNECTION_AWAITING_JOINT)
		{
			rigidbody.angularDrag = 0;
			rigidbody.angularVelocity = Vector3.zero;
			rigidbody.velocity = Vector3.zero;
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
		GetComponent<Health>().SetDamage(_damage);
		//m_StucturalIntegrity -= _damage;
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
		
		m_WheelFrictionCurve.extremumSlip = 0.6f;
		m_WheelFrictionCurve.extremumValue = 0.0f;
		m_WheelFrictionCurve.asymptoteSlip = 2.0f;
		m_WheelFrictionCurve.asymptoteValue = 0.0f;
		m_WheelFrictionCurve.stiffness = 200;
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
			
		rigidbody.AddForce(transform.TransformDirection(Drag) * rigidbody.mass * Time.fixedDeltaTime);
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
		
		if(m_ConnectionState == ConnectionState.NOT_CONNECTED)
		{
			ConfigurableJoint configJoint = GetComponent<ConfigurableJoint>();
			if(configJoint)
			{
				Destroy(configJoint);
			}
			
			m_Train = null;
		}
	}
	
	public ConnectionState GetConnectionState()
	{
		return(m_ConnectionState);
	}
	
	public void SetFrontBackLatchTransform(Transform _transform)
	{
		m_FrontBackLatchTransform = _transform;
	}
	
	public void AddExtraCollisionForce()
	{
		// Add a force upwards to fly the carriage over the train
		Vector3 v3RandomForce = new Vector3(Random.Range(-0.2f, 0.2f), 1.0f, Random.Range(-0.2f, 0.2f)).normalized * rigidbody.mass * 50.0f; 
		rigidbody.AddForce(v3RandomForce, ForceMode.Impulse);
	}
	
	void OnCollisionEnter(Collision CollisionInfo)
	{	
		if(!Network.isServer)
		{
			return;
		}
		
		TrainCarriages PlayerTrainCarrages;
		//Health CarriageHealth;
		
		if (CollisionInfo.gameObject.name == "Train(Clone)")
		{
			if (m_Train == null)
			{
				PlayerTrainCarrages = CollisionInfo.gameObject.GetComponent<TrainCarriages>();
				PlayerTrainCarrages.AddCarriage(this);
			}
			else
			{
				//CarriageHealth = GetComponent<Health>();
				
				//if (CarriageHealth != null){
					//CarriageHealth.OnDeath();
				//}
			}
		}
	}		
	
	void Dying()
	{
		m_Dying = true;
		
       	GameObject meshy = Instantiate(Resources.LoadAssetAtPath("Assets/Carriages/train_boxwreck.obj", typeof(GameObject))) as GameObject;
		
		GetComponentInChildren<MeshFilter>().mesh = meshy.GetComponentInChildren<MeshFilter>().mesh;
	}
	
	void DestroyTrain(){
		this.renderer.enabled = false;
	}
	
	public void SetSplineFollowState(bool _State)
	{
		m_FollowSpline = _State;
	}
	
	
	//private GameObject 			m_ObjectWeaponPowerUp;
	private GameObject			m_PowerupOrWeapon;
	
	public Transform[] 			m_WheelTransforms;
	
	//public float				m_StucturalIntegrity	= 100.0f;
	public float	 			m_SuspensionRange 		= 0.5f;
	public float 				m_SuspensionDamper 		= 0.0f;
	public float 				m_SuspensionSpring 		= 0.0f;
	
	public Vector3 				m_GroundDragMultiplier = new Vector3(2.0f, 5.0f, 1.0f);
	public Vector3 				m_AirDragMultiplier = new Vector3(0.0f, 0.0f, 1.0f);
	
	private bool 				m_IsOnGround = false;
	private	float				m_GroundedTime = 0.0f;
	private bool				m_FollowSpline = false;
	
	private float 				m_InitAngularDrag;
	
	private CarriageWheel[] 	m_Wheels;
	private float 				m_WheelRadius;
	private WheelFrictionCurve 	m_WheelFrictionCurve;
	
	private Transform			m_Train;
	private bool 				m_Dying = false;
	
	private ConnectionState		m_ConnectionState;
	private	float 				m_InitDistanceToLatch;
	private	float				m_TimeSinceCollision;
	private	Transform			m_FrontBackLatchTransform;
	
	private Vector3 			m_BackSplinePosition;
	private Vector3				m_FrontSplinePosition;	
	private Vector3				m_midPointSpinePosition;
	private Quaternion			m_SplineRotation;
	
}
