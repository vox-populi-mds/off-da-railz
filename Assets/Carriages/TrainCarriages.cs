using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public enum ECarriageType {
	Rocket,
	Shotgun,
	Gatling
	
};

public class TrainCarriages : MonoBehaviour {
	private List<Carriages> m_listCarriages;
	 
	int GetNumberOfCarrages(){
		return (m_listCarriages.Count);
	}
	
	int AddCarriage(ECarriageType carriageType) {
		switch (carriageType) {
		case Rocket:
			break;
		}
	}
	
	// Use this for initialization
	void Start() {
		m_listCarriages = new List<Carriages>();
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
		
}
