using UnityEngine;
using System.Collections;

public class DisplayScoreBoard : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		// Background Box
		GUI.Box(new Rect(10,10,Screen.width-20,Screen.height-20), "Scoreboard");
	}
}
