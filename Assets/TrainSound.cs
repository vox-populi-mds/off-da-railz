using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TrainSound : MonoBehaviour 
{
	void Start() 
	{
		m_Train = GetComponent<Train>();
		
		DVolume *= 0.4f;
		EVolume *= 0.4f;
		FVolume *= 0.4f;
		KVolume *= 0.7f;
		LVolume *= 0.4f;
		
		DAudio = Audio.GetInstance.Play(D, transform, DVolume, true);
		EAudio = Audio.GetInstance.Play(E, transform, EVolume, true);
		FAudio = Audio.GetInstance.Play(F, transform, FVolume, true);
		KAudio = Audio.GetInstance.Play(K, transform, KVolume, true);
		LAudio = Audio.GetInstance.Play(L, transform, LVolume, true);
		DubstepAudio = Audio.GetInstance.Play(Dubstep, transform, DubstepVolume, true);
	}
	
	void Update() 
	{
		float trainSpeed  = m_Train.rigidbody.velocity.magnitude;
		float trainSpeedFactor = Mathf.Clamp01(trainSpeed / m_Train.m_MaximumVelocity);
		
		if(shiftingGear)
		{
			return;
		}
		
		float pitchFactor = Sinerp(0.0f, topGear, trainSpeedFactor);
		int newGear = (int)pitchFactor;
		
		pitchFactor -= newGear;
		float throttleFactor = pitchFactor;
		pitchFactor *= 0.3f;
		pitchFactor += throttleFactor * (0.7f) * Mathf.Clamp01(m_Train.m_Throttle * 2.0f);
		
		if(newGear != gear)
		{
			if(newGear > gear)
			{
				StartCoroutine(GearShift(prevPitchFactor, pitchFactor, gear, true));
			}
			else
			{
				StartCoroutine(GearShift(prevPitchFactor, pitchFactor, gear, false));
			}
			gear = newGear;
		}
		else
		{
			float newPitch = 0.0f;
			if(gear == 0)
			{
				newPitch = Mathf.Lerp(idlePitch, highPitchFirst, pitchFactor);
			}
			else if(gear == 1)
			{
				newPitch = Mathf.Lerp(startPitch, highPitchSecond, pitchFactor);
			}
			else if(gear == 2)
			{
				newPitch = Mathf.Lerp(lowPitch, highPitchThird, pitchFactor);
			}
			else
			{
				newPitch = Mathf.Lerp(medPitch, highPitchFourth, pitchFactor);
			}
			SetPitch(newPitch);
			SetVolume(newPitch);
		}
		prevPitchFactor = pitchFactor;
	}
	
	IEnumerator GearShift(float oldPitchFactor, float newPitchFactor, int gear, bool shiftUp)
	{
		shiftingGear = true;
		
		float timer = 0.0f;
		float pitchFactor = 0.0f;
		float newPitch = 0.0f;
		
		if(shiftUp)
		{
			while(timer < gearShiftTime)
			{
				pitchFactor = Mathf.Lerp(oldPitchFactor, 0.0f, timer / gearShiftTime);
				if(gear == 0)
				{
					newPitch = Mathf.Lerp(lowPitch, highPitchFirst, pitchFactor);
				}
				else
				{
					newPitch = Mathf.Lerp(lowPitch, highPitchSecond, pitchFactor);
				}
				SetPitch(newPitch);
				SetVolume(newPitch);
				timer += Time.deltaTime;
				yield return null;
			}
		}
		else
		{
			while(timer < gearShiftTime)
			{
				pitchFactor = Mathf.Lerp(0.0f, 1.0f, timer / gearShiftTime);
				newPitch = Mathf.Lerp(lowPitch, shiftPitch, pitchFactor);
				SetPitch(newPitch);
				SetVolume(newPitch);
				timer += Time.deltaTime;
				yield return null;
			}
		}
			
		shiftingGear = false;
	}
	
	void SetPitch(float pitch)
	{
		DAudio.pitch = pitch;
		EAudio.pitch = pitch;
		FAudio.pitch = pitch;
		LAudio.pitch = pitch;
	}
	
	void SetVolume(float pitch)
	{
		float pitchFactor = Mathf.Lerp(0.0f, 1.0f, (pitch - startPitch) / (highPitchSecond - startPitch));
		DAudio.volume = Mathf.Lerp(0.0f, DVolume, pitchFactor);
		float fVolume = Mathf.Lerp(FVolume * 0.80f, FVolume, pitchFactor);
		FAudio.volume = fVolume * 0.7f + fVolume * 0.3f * Mathf.Clamp01(m_Train.m_Throttle);
		float eVolume = Mathf.Lerp(EVolume * 0.89f, EVolume, pitchFactor);
		EAudio.volume = eVolume * 0.8f + eVolume * 0.2f * Mathf.Clamp01(m_Train.m_Throttle);
	}
	
	float Coserp(float _start, float _end, float _value)
	{
		return(Mathf.Lerp(_start, _end, 1.0f - Mathf.Cos(_value * Mathf.PI * 0.5f)));
	}
	
	float Sinerp(float _start, float _end, float _value)
	{
	    return(Mathf.Lerp(_start, _end, Mathf.Sin(_value * Mathf.PI * 0.5f)));
	}
	

	Train m_Train;
	
	public AudioClip D = null;
	public float DVolume = 1.0f;
	public AudioClip E = null;
	public float EVolume = 1.0f;
	public AudioClip F = null;
	public float FVolume = 1.0f;
	public AudioClip K  = null;
	public float KVolume = 1.0f;
	public AudioClip L  = null;
	public float LVolume = 1.0f;
	
	public AudioClip Dubstep = null;
	public float DubstepVolume = 1.0f;

	AudioSource DAudio = null;
	AudioSource EAudio = null;
	AudioSource FAudio = null;
	AudioSource KAudio = null;
	AudioSource LAudio = null;
	
	AudioSource DubstepAudio = null;

	AudioSource carAudio = null;

	float gearShiftTime = 0.1f;
	bool shiftingGear = false;

	int gear = 0;
	int topGear = 1;

	float idlePitch = 0.7f;
	float startPitch = 0.85f;
	float lowPitch = 1.17f;
	float medPitch = 1.25f;
	float highPitchFirst = 1.65f;
	float highPitchSecond = 1.76f;
	float highPitchThird = 1.80f;
	float highPitchFourth = 1.86f;
	float shiftPitch = 1.44f;

	float prevPitchFactor = 0.0f;
}
