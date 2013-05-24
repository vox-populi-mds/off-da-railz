using UnityEngine;
using System.Collections;
using OffDaRailz;

public class WeaponParticleCollider : MonoBehaviour {
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
	
	public void OnParticleCollision(GameObject CollideWith)
	{
		Transform p = CollideWith.transform.parent;
		if(p != null)
		{
			Carriage c = p.GetComponent<Carriage>();
			
			if(c != null)
			{
				c.networkView.RPC("ApplyDamage", RPCMode.All, m_fDamagePerParticle);
			}
		}
	}
}
