using UnityEngine;
using System.Collections;

public class Health : MonoBehaviour {
	public static float m_fDefaultHealth = 100.0f;
	protected float m_fHealth = m_fDefaultHealth;

	public float GetHealth(){
		return (m_fHealth);	
	}

	public void SetDamage(float fDamage){		
		if (0 >= m_fHealth){
			OnDeath();
		}else{
			m_fHealth -= fDamage;				
		}
	}	
	
	public void Reset()
	{
		m_fHealth = 100.0f;
	}	
	
	public void OnParticleCollision(GameObject CollidedWith){
		float fValue = 0.0f;
		
		
		fValue += 1.0f;
	}				
	
	public void OnDeath(){
		m_fHealth = 0.0f;		
	}

	public void OnBirth(){
	
	}	

	// Use this for initialization
	void Start () {
		Reset ();
	}

	// Update is called once per frame
	void Update () {

	}	
}