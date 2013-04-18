using UnityEngine;
using System.Collections;

public class Game : MonoBehaviour {
	public bool showCursor;
	// Use this for initialization
	void Start () {
		// Diable cursor visibility
		Screen.showCursor = showCursor;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
