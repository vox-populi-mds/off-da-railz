using UnityEngine;
using System.Collections;

public class LevelModels : MonoBehaviour
{
	public Transform cube;
	
	GameObject[] Cubes = new GameObject[10];
	
	// Use this for initialization
	void Start ()
	{
		float x = -243;
		for(int i = 0; i < 1; i++)
		{
			Transform cubeTransform = (Transform) Instantiate(cube, new Vector3(x,10,263), Quaternion.identity);
			
			Vector3 Scale = new Vector3(100,100,100);
			
			cubeTransform.localScale = Scale;
			
			
			//x += 100;
			
			//Transform cubeTransform2 = (Transform) Instantiate(cube, new Vector3(x,10,263), Quaternion.identity);
						
			//cubeTransform2.localScale = Scale;
			
		}


	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}
	

}
