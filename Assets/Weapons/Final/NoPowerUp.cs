using UnityEngine;
using System.Collections;
using OffDaRailz;

public class NoPowerUp : MonoBehaviour, IUpgrade
{
	private string m_Name;
	
	void Start ()
	{
		m_Name = "NoPowerUp";
	}
	
	void Update ()
	{
		
	}
	
	public void SetTarget(Transform GunPort)
	{
		
	}
	
	public void EnableUpgrade()
	{
		
	}
	
	public void DisableUpgrade()
	{
		
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