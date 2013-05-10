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
	
		
}
