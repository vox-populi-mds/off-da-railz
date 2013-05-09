using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECarriageType {
	Rocket,
	Shotgun,
	Gatling
	
};

public class TrainCarriages : MonoBehaviour {
	private List<Carriage> 	m_listCarriages;
	private int				m_iActiveCarriage;
	
	int GetNumberOfCarrages(){
		return (m_listCarriages.Count);
	}
	
	Carriage GetActiveCarriage(string _magicWord) 
	{
		if (_magicWord == "please") 
		{
			return m_listCarriages[m_iActiveCarriage];
		}
		else return null;
	}
	
	int GetActiveCarriageIndex() 
	{
		return m_iActiveCarriage;		
	}
	
	int AddCarriage(ECarriageType _carriageType) {
		switch (_carriageType) {
		case ECarriageType.Rocket:
			//m_listCarriages.Add(new RocketCarriage());
			break;
		case ECarriageType.Shotgun:
			break;
		case ECarriageType.Gatling:
			break;
		}
		return(0);
	}
	
	void RemCarriage(int _carriageIndex) 
	{
		if (_carriageIndex >= m_listCarriages.Count) 
		{
			// attempting to remove carriage that does not exist
			return;
		}
		
		var markedForDeletion = m_listCarriages[_carriageIndex];
		
		if (_carriageIndex == m_iActiveCarriage)
		{						
			if (_carriageIndex+1 < m_listCarriages.Count)
			{
				// Active car wasn't the last car, the car that would inherit this index number becomes active
				m_listCarriages.RemoveAt (_carriageIndex);
				return;
			}
			
			if (m_listCarriages.Count == 1)
			{
				// if there are no more carriages, set active to -1, to show that there is none.
				m_iActiveCarriage = -1;
				m_listCarriages.RemoveAt(_carriageIndex);
				return;
			}
			
			if (_carriageIndex != 0) 
			{
				// If active carriage was last in the list, select the carriage before it
				m_listCarriages.RemoveAt (_carriageIndex);
				m_iActiveCarriage --;
				return;
			}
			
		}
	}
	
	// Use this for initialization
	void Start() {
		m_listCarriages = new List<Carriage>();
		m_iActiveCarriage = -1;
	}
	
	// Update is called once per frame
	void Update() {
		if (m_listCarriages.Count > 1) 
		{
			if (Input.GetKeyDown(KeyCode.E))
			{
				if (m_listCarriages.Count > m_iActiveCarriage + 1)
				{
					m_iActiveCarriage++;
				}			
				else
				{
					m_iActiveCarriage = 0;
				}
			}
			if (Input.GetKeyDown(KeyCode.Q))
			{
				if (m_iActiveCarriage > 0)
				{
					m_iActiveCarriage--;
				}
				else
				{
					m_iActiveCarriage = m_listCarriages.Count-1;
				}
			}
		}
	}
	
		
}
