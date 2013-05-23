using UnityEngine;
using System.Collections;
using OffTheRailz;

public class WeaponParticleCollisions : MonoBehaviour {
	public float m_fDamagePerShot;
	private float m_fDamagePerParticle;
	
	// Use this for initialization
	void Start () {
		ParticleEmitter ParentSystem = GetComponent<ParticleEmitter>();
		
		if (ParentSystem != null){
			m_fDamagePerParticle = m_fDamagePerShot / ParentSystem.maxEmission;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	public void OnParticleCollision(GameObject CollideWith){
		Health ObjectHealth = CollideWith.GetComponent<Health>();
		
		if (ObjectHealth != null){
			ObjectHealth.SetDamage(m_fDamagePerParticle);
		}
	}
}
