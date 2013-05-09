using UnityEngine;
using System.Collections;
using OffTheRailz;

namespace OffTheRailz{
	public class FPSShoot : MonoBehaviour{
		public static float m_fRateOfFire = 1.0f;
		public static int m_iMagazineCapacity = 10;
		public static float m_fReloadTime = 10.0f;
		public static float m_fDamage = 10.0f;
		
		protected int m_iMagazineBullets = m_iMagazineCapacity;
		protected float m_fReloadingTime = 0.0f;
		protected GameObject m_DamagableObject;
		protected float m_fFireTime = 0.0f;
		protected Ray m_TargetDirection;
		protected RaycastHit m_Hit;
		
		// Use this for initialization
		void Start () {
				
		}
		
		// Update is called once per frame
		void Update () {
			Vector3 vecPosition = new Vector3();
			
			if (m_iMagazineBullets > 0){
				if (Input.GetMouseButtonDown(0) && (m_fFireTime > m_fRateOfFire)){
					m_TargetDirection = Camera.current.ScreenPointToRay(Camera.current.ScreenToWorldPoint(vecPosition));
					
					if (Physics.Raycast(m_TargetDirection, out m_Hit)){
						m_Hit.collider.gameObject.GetComponent<OffTheRailz.Health>().SetDamage(m_fDamage);
					}
					
					m_fFireTime = 0.0f;
				}else{
					m_fFireTime += Time.deltaTime;					
				}
			}else if (m_fReloadingTime > m_fReloadTime){
				m_iMagazineBullets = m_iMagazineCapacity;	
				m_fReloadingTime = 0.0f;
			}else{
				m_fReloadingTime += Time.deltaTime;
			}
		}
	}
}
