using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainCarrages : MonoBehaviour {
	private List<Carrages> m_listCarrages;
	
	// Use this for initialization
	void Start() {
		m_listCarrages = new List<Carrages>();
	}
	
	// Update is called once per frame
	void Update() {
	
	}
	
	int GetNumberOfCarrages(){
		return (m_listCarrages.Count);
	}
}
