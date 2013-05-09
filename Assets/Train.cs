using UnityEngine;
using System.Collections;

public class Wheel
{
	public WheelCollider 	m_Collider;
	public Transform 		m_WheelGraphic;
	public Transform 		m_TireGraphic;
	public bool 			m_DriveWheel  		= false;
	public bool 			m_SteerWheel  		= false;
	public int 				m_LastSkidmark  	= -1;
	public Vector3 			m_LastEmitPosition  = Vector3.zero;
	public float 			m_LastEmitTime  	= Time.time;
	public Vector3 			m_WheelVelo  		= Vector3.zero;
	public Vector3 			m_GroundSpeed 		= Vector3.zero;
}

public class Train : MonoBehaviour 
{
	bool mine;
	
	Train()
	{
		mine = false;
	}
	
	public bool IsMine()
	{
		return mine;
	}
	
	public void SetMine(bool mine)
	{
		this.mine = mine;
	}
	
	void Start() 
	{
		if (!mine)
		{
			return;	
		}
		
		SetupWheelColliders();
		
		SetupCenterOfMass();
		
		SetupGears();
		
		SetupTestBoxcar();
	
		m_InitialDragMultiplierX = m_GroundDragMultiplier.x;
	}
	
	void SetupPlayerMarker()
	{
		GameObject marker = GameObject.Find("PlayerMarker");
		//int iID = int.Parse(networkView.owner.guid);
		Light light = marker.GetComponent<Light>();
		
		light.color = m_PlayerColor;
		//print("Player ID = " + iID);
		
		/*switch (iID)
		{
		case 0:
			light.color = Color.yellow;
			break;
			
		case 1:
			light.color = m_PlayerColor;
			break;
		}*/
		
	}
	
	void SetupWheelColliders()
	{
		m_Wheels = new Wheel[m_FrontWheels.Length + m_RearWheels.Length];
		
		SetupWheelFrictionCurve();
		
		int wheelCount = 0;
		foreach(Transform t in m_FrontWheels)
		{
			m_Wheels[wheelCount] = SetupWheel(t, true);
			wheelCount++;
		}
		
		foreach(Transform t in m_RearWheels)
		{
			m_Wheels[wheelCount] = SetupWheel(t, false);
			wheelCount++;
		}
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
	
	Wheel SetupWheel(Transform _WheelTransform, bool _IsFrontWheel)
	{
		GameObject Go = new GameObject(_WheelTransform.name + " Collider");
		Go.transform.position = _WheelTransform.position;
		Go.transform.parent = transform;
		Go.transform.rotation = _WheelTransform.rotation;
			
		WheelCollider Wc = Go.AddComponent(typeof(WheelCollider)) as WheelCollider;
		Wc.suspensionDistance = m_SuspensionRange;
		JointSpring Js = Wc.suspensionSpring;
		
		if(_IsFrontWheel)
		{
			Js.spring = m_SuspensionSpringFront;
		}
		else
		{
			Js.spring = m_SuspensionSpringRear;
		}
			
		Js.damper = m_SuspensionDamper;
		Wc.suspensionSpring = Js;
			
		Wheel WheelObj = new Wheel(); 
		WheelObj.m_Collider = Wc;
		Wc.sidewaysFriction = m_WheelFrictionCurve;
		WheelObj.m_WheelGraphic = _WheelTransform;
		WheelObj.m_TireGraphic = _WheelTransform.GetComponentsInChildren<Transform>()[1];
		
		m_WheelRadius = WheelObj.m_TireGraphic.renderer.bounds.size.y / 2;	
		WheelObj.m_Collider.radius = m_WheelRadius;
		
		if(_IsFrontWheel)
		{
			WheelObj.m_SteerWheel = true;
			
			Go = new GameObject(_WheelTransform.name + " Steer Column");
			Go.transform.position = _WheelTransform.position;
			Go.transform.rotation = _WheelTransform.rotation;
			Go.transform.parent = transform;
			_WheelTransform.parent = Go.transform;
		}
		else
		{
			WheelObj.m_DriveWheel = true;
		}
			
		return WheelObj;
	}
	
	void SetupCenterOfMass()
	{
		if(m_CenterOfMass != null)
		{
			rigidbody.centerOfMass = m_CenterOfMass.localPosition;
		}
	}
	
	void SetupGears()
	{
		m_EngineForceValues = new float[m_NumberOfGears];
		m_GearSpeeds = new float[m_NumberOfGears];
		
		float TempTopSpeed = m_MaximumVelocity;
			
		for(int i = 0; i < m_NumberOfGears; ++i)
		{
			if(i > 0)
			{
				m_GearSpeeds[i] = TempTopSpeed / 4 + m_GearSpeeds[i-1];
			}
			else
			{
				m_GearSpeeds[i] = TempTopSpeed / 4;
			}
			
			TempTopSpeed -= TempTopSpeed / 4;
		}
		
		float EngineFactor = m_MaximumVelocity / m_GearSpeeds[m_GearSpeeds.Length - 1];
		
		for(int i = 0; i < m_NumberOfGears; ++i)
		{
			float MaxLinearDrag = m_GearSpeeds[i] * m_GearSpeeds[i];
			m_EngineForceValues[i] = MaxLinearDrag * EngineFactor;
		}
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
		FollowObject followScript = networkBoxGO.GetComponent<FollowObject>();
		followScript.target = m_TrainLatchTransform;
		followScript.distance = 20;
		
		Debug.Log(string.Format("Box car Created at {0} ", vPosition));
	}
	
	/***************************************************************************************************/
	/*										Update Functions 									  	   */
	/***************************************************************************************************/
	
	void Update() 
	{
		if (!mine)
		{
			return;	
		}
		
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		ProcessInput();
		
		ProcessIfFlipped();
		
		ProcessWheelGraphics(RelativeVelocity);
	
		ProcessGear(RelativeVelocity);
	}
	
	void ProcessIfFlipped()
	{
		if(transform.localEulerAngles.z > 80 && transform.localEulerAngles.z < 280)
		{
			m_ResetTimer += Time.deltaTime;
		}
		else
		{
			m_ResetTimer = 0;
		}
		
		if(m_ResetTimer > m_ResetTime)
		{
			FlipCar();
		}
	}
	
	void ProcessWheelGraphics(Vector3 _RelativeVelocity)
	{		
		Animation DriveAnimation = GetComponentInChildren<Animation>();
		
		float CurrentSpeed = _RelativeVelocity.z;
		float SpeedScale = CurrentSpeed / 10.0f;
		
		DriveAnimation.animation["Drive"].speed = SpeedScale;
		
		foreach(Wheel w in m_Wheels)
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
				
				if(w.m_SteerWheel)
				{
					w.m_WheelVelo *= 0.9f;
				}
				else
				{
					w.m_WheelVelo *= 0.9f * (1.0f - m_Throttle);
				}
				
				//if(skidmarks)
				//{
				//	w.lastSkidmark = -1.0f;
				//	sound.Skid(false, 0);
				//}
			}
			// If the wheel is a steer wheel we apply two rotations:
			// *Rotation around the Steer Column (visualizes the steer direction)
			// *Rotation that visualizes the speed
			if(w.m_SteerWheel)
			{
				Vector3 Ea = w.m_WheelGraphic.parent.localEulerAngles;
				Ea.y = m_Steer * m_MaximumTurn;
				w.m_WheelGraphic.parent.localEulerAngles = Ea;
				w.m_TireGraphic.Rotate(Vector3.right * (w.m_GroundSpeed.z / m_WheelRadius) * Time.deltaTime * Mathf.Rad2Deg);
			}
			else if(!m_Handbrake && w.m_DriveWheel)
			{
				// If the wheel is a drive wheel it only gets the rotation that visualizes speed.
				// If we are hand braking we don't rotate it.
				w.m_TireGraphic.Rotate(Vector3.right * (w.m_GroundSpeed.z / m_WheelRadius) * Time.deltaTime * Mathf.Rad2Deg);
			}
		}

	}
	
	void ProcessGear(Vector3 _RelativeVelocity)
	{
		m_CurrentGear = 0;
		for(int i = 0; i < m_NumberOfGears - 1; ++i)
		{
			if(_RelativeVelocity.z > m_GearSpeeds[i])
			{
				m_CurrentGear = i + 1;
			}
		}
	}
	
	void FlipCar()
	{
		transform.rotation = Quaternion.LookRotation(transform.forward);
		transform.position += Vector3.up * 0.5f;
		rigidbody.velocity = Vector3.zero;
		rigidbody.angularVelocity = Vector3.zero;
		m_ResetTimer = 0;
		m_CurrentEnginePower = 0;
	}
	
	void ProcessInput()
	{
		m_Throttle = Input.GetAxis("Vertical");
		m_Steer = Input.GetAxis("Horizontal");
		
		/*if(throttle < 0.0
		{
			brakeLights.SetFloat("_Intensity", Mathf.Abs(throttle));
		}
		else
		{
			brakeLights.SetFloat("_Intensity", 0.0);
		}*/
		
		//CheckHandbrake();
	}
	
	/***************************************************************************************************/
	/*								Fixed Update Functions (Rigid Body)							  	   */
	/***************************************************************************************************/
	
	void FixedUpdate()
	{	
		if (!mine)
		{
			return;	
		}
		
		// Transform rigidbody velocity to local co-ordinate space
		Vector3 RelativeVelocity = transform.InverseTransformDirection(rigidbody.velocity);
		
		CalculateState();	
		
		ProcessFriction(RelativeVelocity);
		
		ProcessDrag(RelativeVelocity);
		
		CalculateEnginePower(RelativeVelocity);
		
		ApplyThrottle(m_CanDrive, RelativeVelocity);
		
		ApplySteering(m_CanSteer, RelativeVelocity);
	}
	
	void ProcessDrag(Vector3 _RelativeVelocity)
	{
		// Only apply the drag if the train is on the ground
		bool IsOnGround = false;
		foreach(Wheel w in m_Wheels)
		{
			WheelCollider Wc = w.m_Collider;
			WheelHit Wh = new WheelHit();
			
			// First we get the velocity at the point where the wheel meets the ground, if the wheel is touching the ground
			if(Wc.GetGroundHit(out Wh))
			{	
				IsOnGround = true;
				break;
			}
		}
		
		Vector3 DragMultiplier = m_GroundDragMultiplier;
		if(!IsOnGround)
		{
			DragMultiplier = m_AirDragMultiplier;
		}
		
		Vector3 RelativeDrag = new Vector3(	-_RelativeVelocity.x * Mathf.Abs(_RelativeVelocity.x), 
											-_RelativeVelocity.y * Mathf.Abs(_RelativeVelocity.y), 
											-_RelativeVelocity.z * Mathf.Abs(_RelativeVelocity.z) );
		
		Vector3 Drag = Vector3.Scale(DragMultiplier, RelativeDrag);
			
		if(m_InitialDragMultiplierX > DragMultiplier.x) // Handbrake code
		{			
			Drag.x /= (_RelativeVelocity.magnitude / (m_MaximumVelocity / ( 1 + 2 * m_HandbrakeXDragFactor ) ) );
			Drag.z *= (1.0f + Mathf.Abs(Vector3.Dot(rigidbody.velocity.normalized, transform.forward)));
			Drag += rigidbody.velocity * Mathf.Clamp01(rigidbody.velocity.magnitude / m_MaximumVelocity);
		}
		else // No handbrake
		{
			Drag.x *= m_MaximumVelocity / _RelativeVelocity.magnitude;
		}
		
		if(Mathf.Abs(_RelativeVelocity.x) < 5 && !m_Handbrake)
		{
			Drag.x = -_RelativeVelocity.x * DragMultiplier.x;
		}
			
		rigidbody.AddForce(transform.TransformDirection(Drag) * rigidbody.mass * Time.deltaTime);
	}
	
	void ProcessFriction(Vector3 _RelativeVelocity)
	{
		float SqrVel = _RelativeVelocity.x * _RelativeVelocity.x;
		
		// Add extra sideways friction based on the car's turning velocity to avoid slipping
		m_WheelFrictionCurve.extremumValue = Mathf.Clamp(300 - SqrVel, 0, 300);
		m_WheelFrictionCurve.asymptoteValue = Mathf.Clamp(150 - (SqrVel / 2), 0, 150);
			
		foreach(Wheel w in m_Wheels)
		{
			w.m_Collider.sidewaysFriction = m_WheelFrictionCurve;
			w.m_Collider.forwardFriction = m_WheelFrictionCurve;
		}
	}
	
	void CalculateEnginePower(Vector3 _RelativeVelocity)
	{
		if(m_AutomaticThrottle)
		{
			m_Throttle = 1.0f;
		}
		
		if(m_Throttle == 0)
		{
			m_CurrentEnginePower -= Time.deltaTime * 200.0f;
		}
		else if(Mathf.Sign(_RelativeVelocity.z) == Mathf.Sign(m_Throttle))
		{
			float NormPower = (m_CurrentEnginePower / m_EngineForceValues[m_EngineForceValues.Length - 1]) * 2;
			m_CurrentEnginePower += Time.deltaTime * 200.0f * EvaluateNormPower(NormPower);
		}
		else
		{
			m_CurrentEnginePower -= Time.deltaTime * 300;
		}
		
		if(m_CurrentGear == 0)
		{
			m_CurrentEnginePower = Mathf.Clamp(m_CurrentEnginePower, 0, m_EngineForceValues[0]);
		}
		else
		{
			m_CurrentEnginePower = Mathf.Clamp(m_CurrentEnginePower, m_EngineForceValues[m_CurrentGear - 1], m_EngineForceValues[m_CurrentGear]);
		}
	}
	
	void CalculateState()
	{
		m_CanDrive = false;
		m_CanSteer = false;
		
		foreach(Wheel w in m_Wheels)
		{
			if(w.m_Collider.isGrounded)
			{
				if(w.m_SteerWheel)
				{
					m_CanSteer = true;
				}
				if(w.m_DriveWheel)
				{
					m_CanDrive = true;
				}
			}
		}
	}
	
	void ApplyThrottle(bool _CanDrive, Vector3 _RelativeVelocity)
	{
		if(_CanDrive)
		{
			float ThrottleForce = 0.0f;
			float BrakeForce = 0.0f;
			
			if(Mathf.Sign(_RelativeVelocity.z) == Mathf.Sign(m_Throttle))
			{
				if(!m_Handbrake)
				{
					ThrottleForce = Mathf.Sign(m_Throttle) * m_CurrentEnginePower * rigidbody.mass;
				}
			}
			else
			{
				BrakeForce = Mathf.Sign(m_Throttle) * m_EngineForceValues[0] * rigidbody.mass;
			}
			
			rigidbody.AddForce(transform.forward * Time.deltaTime * (ThrottleForce + BrakeForce));
		}
	}
	
	void ApplySteering(bool _CanSteer, Vector3 _RelativeVelocity)
	{
		if(_CanSteer)
		{
			float TurnRadius = 3.0f / Mathf.Sin((90.0f - (m_Steer * 30.0f)) * Mathf.Deg2Rad);
			float MinMaxTurn = EvaluateSpeedToTurn(rigidbody.velocity.magnitude);
			float TurnSpeed = Mathf.Clamp(_RelativeVelocity.z / TurnRadius, -MinMaxTurn / 10.0f, MinMaxTurn / 10.0f);
			
			transform.RotateAround(	transform.position + transform.right * TurnRadius * m_Steer, 
									transform.up, 
									TurnSpeed * Mathf.Rad2Deg * Time.deltaTime * m_Steer);
			
			Vector3 DebugStartPoint = transform.position + transform.right * TurnRadius * m_Steer;
			Vector3 DebugEndPoint = DebugStartPoint + Vector3.up * 5.0f;
			
			Debug.DrawLine(DebugStartPoint, DebugEndPoint, Color.red);
			
			if(m_InitialDragMultiplierX > m_GroundDragMultiplier.x) // Handbrake
			{
				float RotationDirection = Mathf.Sign(m_Steer); // rotationDirection is -1 or 1 by default, depending on steering
				if(m_Steer == 0.0f)
				{
					if(rigidbody.angularVelocity.y < 1.0f) // If we are not steering and we are handbraking and not rotating fast, we apply a random rotationDirection
					{
						RotationDirection = Random.Range(-1.0f, 1.0f);
					}
					else
					{
						RotationDirection = rigidbody.angularVelocity.y; // If we are rotating fast we are applying that rotation to the car
					}
				}
				
				// Finally we apply this rotation around a point between the cars front wheels.
				transform.RotateAround( transform.TransformPoint( (	m_FrontWheels[0].localPosition + m_FrontWheels[1].localPosition) * 0.5f), 
																	transform.up, 
																	rigidbody.velocity.magnitude * Mathf.Clamp01(1 - rigidbody.velocity.magnitude / m_MaximumVelocity) * RotationDirection * Time.deltaTime * 2.0f);
			}
		}
	}
	
	float EvaluateSpeedToTurn(float _Speed)
	{
		if(_Speed > m_MaximumVelocity / 2.0f)
		{
			return(m_MinimumTurn);
		}
		
		float SpeedIndex = 1.0f - (_Speed / (m_MaximumVelocity / 2.0f));
		
		return(m_MinimumTurn + SpeedIndex * (m_MaximumTurn - m_MinimumTurn));
	}
	
	float EvaluateNormPower(float _NormPower)
	{
		if(_NormPower < 1)
		{
			return(10.0f - _NormPower * 9.0f);
		}
		else
		{
			return(1.9f - _NormPower * 0.9f);
		}
	}
	
	// Public
	public Color		m_PlayerColor			= Color.blue;
	public float 		m_MaximumVelocity 		= 20.0f;
	public float 		m_MaximumTurn			= 15f;
	public float 		m_MinimumTurn			= 10f;
	public int 			m_NumberOfGears 		= 2;
	public bool			m_AutomaticThrottle		= true;
	
	public Transform[] 	m_FrontWheels;
	public Transform[] 	m_RearWheels;
	
	public Transform	m_FrontSteering;
	public Transform	m_RearSteering;
	
	public Transform 	m_CenterOfMass;
	
	float 				m_ResetTime  			= 5.0f;
	
	public float	 	m_SuspensionRange 		= 0.1f;
	public float 		m_SuspensionDamper 		= 50.0f;
	public float 		m_SuspensionSpringFront = 18500.0f;
	public float 		m_SuspensionSpringRear 	= 9000.0f;
	
	public Vector3 		m_GroundDragMultiplier = new Vector3(2.0f, 5.0f, 1.0f);
	public Vector3 		m_AirDragMultiplier = new Vector3(2.0f, 5.0f, 1.0f);
	
	public float 		m_Throttle			= 0.0f;
	
	public Transform	m_TrainBoxCarTransform;
	public Transform	m_TrainLatchTransform;
		
	// Protected
		
	// Private
	WheelFrictionCurve 	m_WheelFrictionCurve;
	Wheel[] 			m_Wheels;
	float 				m_WheelRadius 		= 0.4f;
	
	float 				m_Steer 	 		= 0.0f;	

	float 				m_ResetTimer  = 0.0f;

	float[] 			m_EngineForceValues;
	float[] 			m_GearSpeeds;
	
	int 				m_CurrentGear;
	float 				m_CurrentEnginePower = 0.0f;
	
	bool 				m_Handbrake  		= false;
	float 				m_HandbrakeXDragFactor = 0.5f;
	float 				m_InitialDragMultiplierX = 10.0f;
	float 				m_HandbrakeTime = 0.0f;
	float 				m_HandbrakeTimer = 1.0f;
	
	bool 				m_CanSteer;
	bool 				m_CanDrive;
	
}
