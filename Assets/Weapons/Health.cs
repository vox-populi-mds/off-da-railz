using UnityEngine;
using System.Collections;

namespace OffTheRailz{
	public class Health : MonoBehaviour {
		public static float m_fDefaultHealth;
		protected float m_fHealth = m_fDefaultHealth;
	
		public float GetHealth(){
			return (m_fDefaultHealth);	
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
	
		}
	
		// Update is called once per frame
		void Update () {
	
		}	
	}
}
