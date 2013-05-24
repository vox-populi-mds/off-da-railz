using UnityEngine;
using System.Collections;
using OffDaRailz;

public class WeaponParticleCollider : MonoBehaviour {
	public float m_fDamagePerShot;
	public static AudioClip m_BulletMetalSound;
	private float m_fDamagePerParticle;
	
	// Use this for initialization
	void Start () {
		ParticleEmitter ParentSystem = GetComponent<ParticleEmitter>();
		
		if (ParentSystem != null){
			m_fDamagePerParticle = m_fDamagePerShot / ParentSystem.maxEmission;
		}
		
		if (m_BulletMetalSound == null){
			m_BulletMetalSound = Resources.LoadAssetAtPath("Assets/Weapons/Final/bulletmetal.mp3", typeof(AudioClip)) as AudioClip;
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
				var thing = Audio.GetInstance.Play(m_BulletMetalSound, transform, 1.0f, false);	
				c.networkView.RPC("ApplyDamage", RPCMode.All, m_fDamagePerParticle);
			}
		}
	}
}
