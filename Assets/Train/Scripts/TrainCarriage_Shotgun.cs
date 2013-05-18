using UnityEngine;
using System.Collections;

public class TrainCarriage : MonoBehaviour{
	/*public static int m_iMagazineCapacity = 10;
	public static float m_fReloadTime = 5.0f;
	public static float m_fOffset = 10.0f;
	
	protected int m_iMagazine = m_iMagazineCapacity;
	protected ITrainEngine m_Train = null;
	protected float m_fReloading = 0.0f;
	
	void ModifyTarget(ITrainEngine trainengine){
		m_Train = trainengine;
	}
	
	void Reposition(){
		Vector3 l_NewPosition;
		Vector3 l_NewDirection;
		
		l_NewPosition = new Vector3(m_Train.GetOrientation().position);
		transform.rotation = new Quaternion(m_Train.GetOrientation().rotation);
		l_NewDirection = transform.TransformDirection(l_NewDirection);
		l_NewPosition.x += l_NewDirection.x * m_fOffset;
		
		transform.position = l_NewPosition;
	}
	
	void Shoot(){
		Vector3 l_ShooterDirection;
		
		if (Input.GetMouseButtonDown(0)){
			if (m_Shotgun.isStopped()){
				if (0 < m_iMagazine){
					m_Shotgun.Play();	
				}else if (m_fReloading < m_fReloadTime){
					m_fReloading += Time.deltaTime;
				}else{
					m_fReloading = 0.0f;
					m_iMagazineCapacity = m_iMagazine;
				}
			}
		}
	}	*/
	
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//Reposition();
		//Shoot();
	}
}
