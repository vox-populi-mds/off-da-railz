using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Audio : Singleton<Audio> 
{ 
	class ClipInfo
    {
       public AudioSource 	audioSource 	{ get; set; }
       public float 		defaultVolume 	{ get; set; }
    }
	
	void Awake() 
	{
        Debug.Log("AudioManager Initialising");
        try 
		{
            transform.parent = GameObject.FindGameObjectWithTag("MainCamera").transform;
            transform.localPosition = new Vector3(0, 0, 0);
        } 
		catch 
		{
            Debug.Log("Unable to find main camera to put audiomanager");
        }
		
		m_activeAudio = new List<ClipInfo>();
    }
	
	void Update() 
	{
		ProcessActiveAudio();
	}
	
	void ProcessActiveAudio()
	{ 
	    var toRemove = new List<ClipInfo>();
	    try 
		{
	        foreach(var audioClip in m_activeAudio) 
			{
	            if(!audioClip.audioSource) 
				{
	                toRemove.Add(audioClip);
	            } 
	        }
	    } 
		catch 
		{
	        Debug.Log("Error updating active audio clips");
	        return;
	    }
	    
		// Cleanup
	    foreach(var audioClip in toRemove) 
		{
	        m_activeAudio.Remove(audioClip);
		}
    }
	
	public AudioSource Play(AudioClip _clip, Vector3 _soundOrigin, float _volume, bool _loop) 
	{
		//Create an empty game object
		GameObject soundLoc = new GameObject("Audio: " + _clip.name);
		soundLoc.transform.position = _soundOrigin;
		
		//Create the source
		AudioSource audioSource = soundLoc.AddComponent<AudioSource>();
		SetAudioSource(ref audioSource, _clip, _volume);
		audioSource.Play();
		
		// Set the audio to loop
		if(_loop) 
		{
			audioSource.loop = true;
		}
		else
		{
			Destroy(soundLoc, _clip.length);
		}
		
		//Set the source as active
		m_activeAudio.Add(new ClipInfo{audioSource = audioSource, defaultVolume = _volume});
		return(audioSource);
	}
	
	public AudioSource Play(AudioClip _clip, Transform _emitter, float _volume, bool _loop) 
	{
		//Create an empty game object
		GameObject soundLoc = new GameObject("Audio: " + _clip.name);
		soundLoc.transform.position = _emitter.position;
		soundLoc.transform.parent = _emitter;
		
		//Create the source
		AudioSource audioSource = Play(_clip, _emitter.position, _volume, _loop);
		audioSource.transform.parent = _emitter;
		return(audioSource);
	}
	
	private void SetAudioSource(ref AudioSource _source, AudioClip _clip, float _volume) 
	{
		_source.rolloffMode = AudioRolloffMode.Logarithmic;
		_source.dopplerLevel = 0.2f;
		_source.minDistance = 40.0f;
		_source.maxDistance = 800.0f;
		_source.clip = _clip;
		_source.volume = _volume;
	}
	
	public void StopSound(AudioSource _toStop) 
	{
		try 
		{
			Destroy(m_activeAudio.Find(s => s.audioSource == _toStop).audioSource.gameObject);
		} 
		catch 
		{
			Debug.Log("Error trying to stop audio source " + _toStop);
		}
	}

	// Public
	
	// Protected
	
	// Private
	List<ClipInfo> m_activeAudio;
}
