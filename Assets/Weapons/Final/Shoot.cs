using UnityEngine;
using System.Collections;
using OffDaRailz;

//http://soundbible.com/1706-Shot-Gun.html
// http://soundfxcenter.com/download-sound/halo-4-unsc-shotgun-reload-sound-effect/

public class Shoot : MonoBehaviour, IUpgrade{
	public static int m_iMagazineCapacity = 10;
	public static float m_fDamage = 10.0f;
	public static AudioClip m_ShootSound;
	public static AudioClip m_ReloadSound;
	public static float m_fReloadTime = 5.0f;
	protected static ParticleEmitter m_RenderBullets;
	
	protected int m_iMagazineBullets = m_iMagazineCapacity;
	protected AudioSource m_ShootSource;
	protected AudioSource m_ReloadSource;
	
	protected float m_fReloadingTime = 0.0f;
	protected bool m_bPlayingSound = false;
	protected bool m_bEnabled = false;
	
	protected Transform m_GunPort;

	// Use this for initialization
	void Start (){
		m_fReloadingTime = 0.0f;
		
		if (m_ShootSound == null){
			m_ShootSound = Resources.LoadAssetAtPath("Assets/Weapons/Final/FireShotgun.mp3", typeof(AudioClip)) as AudioClip;
		}
		
		if (m_ReloadSound == null){
			m_ReloadSound = Resources.LoadAssetAtPath("Assets/Weapons/Final/ReloadShotgun.mp3", typeof(AudioClip)) as AudioClip;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (m_bEnabled && (GetComponent<Animation>().IsPlaying("Take 001") == false)){
			if (m_iMagazineBullets > 0){
				if (Input.GetMouseButtonDown(0) && (m_RenderBullets.particleCount == 0)){
					m_ShootSource = Audio.GetInstance.Play(m_ShootSound, m_RenderBullets.transform, 1.0f, false);	
					--m_iMagazineBullets;
					m_RenderBullets.Emit();
				}
			}else if (m_fReloadingTime > m_fReloadTime){
				m_iMagazineBullets = m_iMagazineCapacity;	
				m_fReloadingTime = 0.0f;
				m_bPlayingSound = true;
			}else{
				if (m_bPlayingSound == false){
					m_ReloadSource = Audio.GetInstance.Play(m_ReloadSound, m_RenderBullets.transform, 1.0f, false);
					m_bPlayingSound = true;
				}
				
				m_fReloadingTime += Time.deltaTime;
			}
		}
		
		if (m_GunPort != null){
			transform.position = m_GunPort.position;
			transform.rotation = m_GunPort.rotation; 
		}
	}
	
	public void SetTarget(Transform GunPort){
		m_GunPort = GunPort;
	}
	
	public void EnableUpgrade(){
		Transform g;
		
		if (m_RenderBullets == null){
			g = transform.FindChild("ShotgunPellets");	
			m_RenderBullets = g.GetComponent<ParticleEmitter>(); 
		}
		
		if (m_bEnabled == false){
			GetComponent<Animation>().animation["Take 001"].speed = 1.0f;
			GetComponent<Animation>().Play("Take 001");
			m_bEnabled = true;
		}
	}
	
	public void DisableUpgrade(){
		if (m_bEnabled == true){
			GetComponent<Animation>().animation["Take 001"].speed = -1.0f;
			GetComponent<Animation>().Play("Take 001");			
		}
	}
}