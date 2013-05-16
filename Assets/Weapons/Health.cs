using UnityEngine;
using System.Collections;

namespace OffTheRailz{
	public class Health : MonoBehaviour {
		public static float m_fDefaultHealth = 100.0f;
		protected float m_fHealth = m_fDefaultHealth;
	
		public float GetHealth(){
			return (m_fHealth);	
		}

		public void SetDamage(float fDamage){		
			if (0 >= m_fHealth){
				m_fHealth = 0.0f;
				OnDeath();
			}else{
				m_fHealth -= fDamage;				
			}
		}
		
		void OnParticleCollision(){
		
		}		
	
		public void OnDeath(){
		
		}
	
		public void OnBirth(){
		
		}	
	
		// Use this for initialization
		void Start () {
			m_fHealth = 100.0f;	
		}
	
		// Update is called once per frame
		void Update () {
	
		}	
	}
}