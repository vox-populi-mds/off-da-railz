using UnityEngine;
using System.Collections;
using OffDaRailz;

public class ParticleCollisions : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnParticleCollision(GameObject CollideWith){
		float l_fHealth = 0.0f;
		
		transform.GetComponent<ParticleEmitter>();
		
		//l_Health = CollidedWith.GetComponent<Health>();
		//l_Health.SetDamage(l_fHealth);
		l_fHealth = 1.0f;
	}
}
