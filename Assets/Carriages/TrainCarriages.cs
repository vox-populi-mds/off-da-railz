using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECarriageType {
	Rocket,
	Shotgun,
	Gatling
	
};

public class TrainCarriages : MonoBehaviour {
	private List<Carriage> m_listCarriages;
	 
	int GetNumberOfCarrages(){
		return (m_listCarriages.Count);
	}
	
	int AddCarriage(ECarriageType carriageType) {
		switch (carriageType) {
		case ECarriageType.Rocket:
			break;
			
		}
		return(0);
	}
	
	// Use this for initialization
	void Start() {
		m_listCarriages = new List<Carriage>();
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
		
}
