// Scott Emery

using UnityEngine;
using System.Collections;

public class Score : MonoBehaviour
{
	public int m_NumCarriages;
	
	// Use this for initialization
	void Start ()
	{
		// Get player objects
		GameObject[] Players;
		
		// Get Carriage interfeace
		//Carriages TrainCarragesInterface;
		
		m_NumCarriages = 0;
		
		// Find player trains
		Players = GameObject.FindGameObjectsWithTag("Train");
		
		// For loop to iterate through players
		for(int i = 0; i < 10; i++)
		{
			// Get player train carriages
			//TrainCarragesInterface = Players.GetComponent<Carriages>();
			
			// Get the total number of carriages 
			//TrainCarragesInterface.GetNumberOfCarrages();
		}
	}
	
	// Update is called once per frame
	void Update ()
	{
		// Check for carriages owned by players
		// This can be used for constant points update or final screen
	}
	
	
}

