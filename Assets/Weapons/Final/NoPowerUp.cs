using UnityEngine;
using System.Collections;
using OffDaRailz;

public class NoPowerUp : MonoBehaviour, IUpgrade
{
	private string 	m_Name;
	private bool	m_bEnabled = false;
	
	void Start ()
	{
		m_Name = "NoPowerUp";
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown(0) && m_bEnabled)
		{			
			Players.Get().GetMe().Train.GetComponent<TrainCarriages>().Buzz();
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
		return(true);
	}
}