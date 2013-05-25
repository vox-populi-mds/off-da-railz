using UnityEngine;
using System.Collections;
using OffDaRailz;

public class SpeedBoost : MonoBehaviour, IUpgrade
{
	private string m_Name;
	protected static bool CoolDown;
	protected bool Enabled;
	public bool m_bEnabled = false;
	protected float CoolDownTimer = 0.0f;
	protected const float CoolDownTime = 7.0f;
	
	protected float EffectTimer = 0.0f;
	protected const float EffectTime = 3.0f;
	
	protected float OriginalSpeed;
	protected float OriginalThrottle;
	
	protected float PowerModifier = 2.0f;
	
	void Start ()
	{
		m_Name = "SpeedBoost";
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown(0) && !CoolDown && m_bEnabled)		
		{
			TrainCarriages trainCarriages = Players.Get().GetMe().Train.GetComponent<TrainCarriages>();			
			Train train = trainCarriages.GetComponent<Train>();
			
			OriginalSpeed = train.m_MaximumVelocity;
			OriginalThrottle = train.m_Throttle;
			
			train.IncreasePower(PowerModifier);
			
			CoolDown = true;
			Enabled = true;
		}
		
		if(Enabled)
		{
			EffectTimer += Time.deltaTime;
			if(EffectTimer > EffectTime)
			{
				TrainCarriages trainCarriages = Players.Get().GetMe().Train.GetComponent<TrainCarriages>();			
				Train train = trainCarriages.GetComponent<Train>();
				
				train.DecreasePower(PowerModifier);
				EffectTimer = 0.0f;
				Enabled = false;
			}
		}
		
		if(CoolDown)
		{
			CoolDownTimer += Time.deltaTime;
			if(CoolDownTimer > CoolDownTime)
			{
				CoolDown = false;
				CoolDownTimer = 0.0f;
			}
			if (Input.GetMouseButtonDown(0) && m_bEnabled)
			{
				if (Players.Get ().GetMe().Train != null)
				Players.Get().GetMe().Train.GetComponent<TrainCarriages>().Buzz();
			}
		}
	}
	
	public void SetTarget(Transform GunPort)
	{
		
	}
	
	public void EnableUpgrade()
	{
		m_bEnabled = true;
	}
	
	public void DisableUpgrade()
	{
		m_bEnabled = false;
	}
	
	public string GetName()
	{
		return(m_Name);
	}
	
	public bool IsAvailable()
	{
		return(!CoolDown);
	}
}