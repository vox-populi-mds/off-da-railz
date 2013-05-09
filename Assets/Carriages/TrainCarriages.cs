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
		return m_listCarriages[m_ActiveCarriage];
	}
	
	int GetActiveCarriageIndex() 
	{
		return m_ActiveCarriage;		
	}
	
	int AddCarriage(Carriage _carriage) {
		
		m_listCarriages.Add(_carriage);
		
		return(0);
	}
	
	void RemCarriage(Carriage _carriage) 
	{
		if (!m_listCarriages.Find(_carriage)) 
		{
			// attempting to remove carriage that does not exist
			return;
		}
		
		var markedForDeletion = _carriage;
				
		if (_carriage == m_ActiveCarriage)
		{
			m_listCarriages.FindIndex(_carriage);
			
			Destroy(m_ActiveCarriage);
			
			m_ActiveCarriage = null;
			m_listCarriages.RemoveAt(_carriageIndex);
					
			if (_carriageIndex != 0) 
			{
				// If active carriage was last in the list, select the carriage before it
				m_listCarriages.RemoveAt (_carriageIndex);
				m_ActiveCarriage --;
				return;
			}
			
		}
	}
	
	// Use this for initialization
	void Start() {
		m_listCarriages = new List<Carriage>();
		m_ActiveCarriage = -1;
	}
	
	// Update is called once per frame
	void Update() {
		if (m_listCarriages.Count > 1) 
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
		}
	}
	
		
}
