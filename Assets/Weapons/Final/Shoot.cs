using UnityEngine;
using System.Collections;

//http://soundbible.com/1706-Shot-Gun.html
// http://soundfxcenter.com/download-sound/halo-4-unsc-shotgun-reload-sound-effect/

public class Shoot : MonoBehaviour{
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
	
	// Use this for initialization
	void Start (){
		m_RenderBullets = GetComponent<ParticleEmitter>();
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
}