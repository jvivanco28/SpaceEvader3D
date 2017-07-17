using UnityEngine;
using System.Collections;

public class AudioManager : MonoBehaviour {
	
	public const int ZONE_1_MUSIC_ID = 0;
	public const int ZONE_2_MUSIC_ID = 1;
	public const int ZONE_3_MUSIC_ID = 2;
	public const int ZONE_4_MUSIC_ID = 3;
	public const int ZONE_5_MUSIC_ID = 4;
	public const int ZONE_6_MUSIC_ID = 5;
	public const int START_SCREEN_MUSIC_ID = 6;
	
	public const float CRASH_WAIT_TIME = 0.7f;
	public const float GIVE = 0.02f;
	public const float MUSIC_FADE_SPEED = 0.4f;
	
	public AudioClip engineClip;
	public AudioClip rotateClip;
	public AudioClip shieldRegenClip;
	public AudioClip slowDownClip;
	public AudioClip shieldLowClip;
	public AudioClip shockClip1;
	public AudioClip shockClip2;
	public AudioClip explodeClip;
	public AudioClip hitClip1;
	public AudioClip hitClip2;
	public AudioClip speedUpClip;
	public AudioClip invulnerabilityEnabledClip;
	public AudioClip invulnerabilityDisabledClip;
	public AudioClip pointClip;
	public AudioClip warpClip;
	public AudioClip arriveClip;
	public AudioClip bonusClip;
	public AudioClip applauseClip;
	public AudioClip evilLaughClip;
	public AudioClip guiSound1;
	public AudioClip guiSound2;
	public AudioClip errorSound;
	public AudioClip musicTrack1;
	public AudioClip musicTrack2;
	public AudioClip musicTrack3;
	public AudioClip musicTrack4;
	public AudioClip musicTrack5;
	public AudioClip musicTrack6;
	public AudioClip startScreenMusic;

	public enum Sound
	{
		engine,
		rotate,
		shieldRegen,
		shieldLow,
		shieldCritical,
		shock,
		explode,
		hit,
		slow,
		speedUp,
		invulnerabilityEnabled,
		invulnerabilityDisabled,
		point,
		warp,
		arrive,
		bonus,
		applause,
		evilLaugh,
		guiSound1,
		guiSound2,
		errorSound
	}
	
	private AudioSource engineAudioSource; // looped effects
	private AudioSource soundEffectAudioSource; // any other sound (powerup, crash, death, etc.)
	private AudioSource pointsAudioSource; // source the point sound only since we'll be tweaking with the pitch
	private AudioSource musicAudioSource;
	private AudioSource shieldLowAudioSource;
	private AudioSource enginePowerUpAudioSource;
	
	private float targetPitch;
	private float crashTime;
	private float currMusicVolume;
	private bool fadeMusicOut;
	private int nextMusicTrackId;
	
	void Awake () 
	{
		engineAudioSource = addAudioSource(engineClip, true, true, 7f, 1f);
		soundEffectAudioSource = addAudioSource(shieldRegenClip, false, false, 1f, 1f);
		pointsAudioSource = addAudioSource(pointClip, false, false, 1f, 1f);
		musicAudioSource = addAudioSource(musicTrack1, true, false, 0, 1);
		shieldLowAudioSource = addAudioSource(shieldLowClip, false, false, 1f, 1f);
		enginePowerUpAudioSource = addAudioSource (speedUpClip, false, false, 0.45f, 1f);
	}
	
	void Start ()
	{
		targetPitch = 1;
		crashTime = -1;
		
		// start playing engine sound if it hasn't already started
		playSound(Sound.engine);
		currMusicVolume = 0f;
		fadeMusicOut = false;
	}
	
	void LateUpdate ()
	{
		if ( Time.time > crashTime + CRASH_WAIT_TIME )
		{
			if ( engineAudioSource.pitch < targetPitch - GIVE )
				engineAudioSource.pitch += Time.deltaTime;
			else if ( engineAudioSource.pitch > targetPitch + GIVE )
				engineAudioSource.pitch -= Time.deltaTime;
		}
		else if ( engineAudioSource.pitch > GIVE )
		{
			engineAudioSource.pitch -= Time.deltaTime;
		}
		
		// for fading music in and out
		if ( fadeMusicOut )
		{
			currMusicVolume = currMusicVolume - (Time.deltaTime * MUSIC_FADE_SPEED);
			musicAudioSource.volume = currMusicVolume;
			if ( currMusicVolume < 0.1f )
			{
				fadeMusicOut = false;
				startPlayingTrack(nextMusicTrackId);
			}
		}
		else if ( currMusicVolume < 0.95f )
		{
			currMusicVolume = currMusicVolume + (Time.deltaTime * MUSIC_FADE_SPEED);
			musicAudioSource.volume = currMusicVolume;
		}
	}
	
	public void playPointSound(float pitch)
	{
		pointsAudioSource.pitch = pitch;
		playSound(Sound.point);
	}
	
	public void playSound(Sound sound)
	{
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_SOUNDS) == (int)PreferencesScript.Sounds.On )
		{
			// engine sound has its own channel
			if (sound == Sound.engine )
			{
				if ( !engineAudioSource.isPlaying )
				{
					GetComponent<AudioSource>().Play();
				}
			}
			// point sound has its own channel too
			else if ( sound == Sound.point )
			{
				if ( pointsAudioSource != null )
				{
					// TODO test this
					pointsAudioSource.PlayOneShot(pointClip, 2f);
				}
			}
			else if ( sound == Sound.shieldLow )
			{
				if ( shieldLowAudioSource != null )
				{
					shieldLowAudioSource.Stop();

					shieldLowAudioSource.pitch = 1f;
					shieldLowAudioSource.Play();
				}
			}
			else if ( sound == Sound.shieldCritical )
			{
				if ( shieldLowAudioSource != null )
				{
					shieldLowAudioSource.Stop();

					shieldLowAudioSource.pitch = 1.2f;
					shieldLowAudioSource.Play();
				}
			}
			// all other sounds are used in the same audio source
			else
			{
				if ( soundEffectAudioSource != null )
				{	
					if (sound == Sound.rotate )
						soundEffectAudioSource.PlayOneShot(rotateClip);
					else if ( sound == Sound.shieldRegen )
					{
						shieldLowAudioSource.Stop();
						soundEffectAudioSource.PlayOneShot(shieldRegenClip, 4f);
					}
					else if ( sound == Sound.shock )
					{
						if ( Random.Range(0f,1f) < 0.5f ) 
							soundEffectAudioSource.PlayOneShot(shockClip1, 1.6f);
						else
							soundEffectAudioSource.PlayOneShot(shockClip2, 1.6f);
					}
					else if ( sound == Sound.hit ) 
					{
						if ( Random.Range(0f,1f) < 0.5f ) 
							soundEffectAudioSource.PlayOneShot(hitClip1, 1f);
						else 
							soundEffectAudioSource.PlayOneShot(hitClip2, 1f);

					} 
					else if ( sound == Sound.slow )
					{
						enginePowerUpAudioSource.pitch = determineEnginePowerUpPitch();
						enginePowerUpAudioSource.PlayOneShot(slowDownClip);
					}
					else if ( sound == Sound.speedUp )
					{
						enginePowerUpAudioSource.pitch = determineEnginePowerUpPitch();
						enginePowerUpAudioSource.PlayOneShot(speedUpClip);
					}
					else if ( sound == Sound.invulnerabilityEnabled )
						soundEffectAudioSource.PlayOneShot(invulnerabilityEnabledClip);
					else if ( sound == Sound.invulnerabilityDisabled )
						soundEffectAudioSource.PlayOneShot(invulnerabilityDisabledClip);
					else if ( sound == Sound.warp )
						soundEffectAudioSource.PlayOneShot(warpClip);
					else if ( sound == Sound.arrive )
						soundEffectAudioSource.PlayOneShot(arriveClip);
					else if ( sound == Sound.bonus )
						soundEffectAudioSource.PlayOneShot(bonusClip);
					else if ( sound == Sound.applause )
						soundEffectAudioSource.PlayOneShot(applauseClip);
					else if ( sound == Sound.evilLaugh )
						soundEffectAudioSource.PlayOneShot(evilLaughClip);
					else if ( sound == Sound.guiSound1 )
						soundEffectAudioSource.PlayOneShot(guiSound1);
					else if ( sound == Sound.guiSound2 )
						soundEffectAudioSource.PlayOneShot(guiSound2);
					else if ( sound == Sound.errorSound )
						soundEffectAudioSource.PlayOneShot(errorSound);
					else if ( sound == Sound.explode ) 
						soundEffectAudioSource.PlayOneShot(explodeClip);
				}//end if
			}
		}
	}

	private float determineEnginePowerUpPitch() 
	{
		return (((targetPitch - 1f ) * 0.5f) + 1);
	}

	public void preDie()
	{
		shieldLowAudioSource.Stop();
//		shieldLowAudioSource.clip = shieldLowPreDieClip;
//		shieldLowAudioSource.loop = true;
//		shieldLowAudioSource.Play();

		shieldLowAudioSource.loop = true;
		shieldLowAudioSource.pitch = 0.55f;
		shieldLowAudioSource.Play();

//		engineAudioSource.Stop ();
//		engineAudioSource.clip = engineBrokenClip;
//		engineAudioSource.loop = true;
//		engineAudioSource.Play ();
	}
	
	public void die()
	{
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_SOUNDS) == (int)PreferencesScript.Sounds.On )
		{
			if ( engineAudioSource != null && soundEffectAudioSource != null )
			{
				killGameAudio();

				soundEffectAudioSource.Stop();
//				if ( explodeClip != null ) 
//					soundEffectAudioSource.clip = explodeClip;
				soundEffectAudioSource.PlayOneShot(explodeClip, 1.4f);
			}//end if
		}//end if
	}
	
	public void setEnginePitch(float pitch)
	{
		targetPitch = pitch;
	}
	
	public void crash()
	{
		crashTime = Time.time;
	}
	
	public void killGameAudio()
	{
		// kill any sounds playing
		if ( engineAudioSource != null )
			engineAudioSource.Stop();
		if ( soundEffectAudioSource != null )
			soundEffectAudioSource.Stop();
		if ( pointsAudioSource != null )
			pointsAudioSource.Stop();
		if ( shieldLowAudioSource != null )
			shieldLowAudioSource.Stop();
	}

	public void killEngineAudio () 
	{
		if ( engineAudioSource != null )
			engineAudioSource.Stop();
	}
	
	public void playGameMusic()
	{
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_MUSIC) == (int)PreferencesScript.Music.On )
		{
			if ( musicAudioSource != null )
			{
				musicAudioSource.Play();
			}//end if
		}//end if
	}
	
	public void pauseMusic()
	{
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_MUSIC) == (int)PreferencesScript.Music.On )
		{
			if ( musicAudioSource != null )
			{
				musicAudioSource.Pause();
			}//end if
		}//end if
	}
	
	public void stopAllMusic()
	{
		if ( musicAudioSource != null )
		{
			musicAudioSource.Stop();
		}//end if
	}
	
	public void switchMusicTrack(int trackId)
	{
		if ( musicAudioSource != null )
		{
			fadeMusicOut = true;
			nextMusicTrackId = trackId;
			
			if ( !musicAudioSource.isPlaying ) 
				startPlayingTrack(nextMusicTrackId);
		}
	}
	
	private void startPlayingTrack(int trackId)
	{

		if ( trackId == 0 )
			musicAudioSource.clip = musicTrack1;
		else if ( trackId == 1 )
			musicAudioSource.clip = musicTrack2;
		else if ( trackId == 2 )
			musicAudioSource.clip = musicTrack3;
		else if ( trackId == 3 )
			musicAudioSource.clip = musicTrack4;
		else if ( trackId == 4 )
			musicAudioSource.clip = musicTrack5;
		else if ( trackId == 5 )
			musicAudioSource.clip = musicTrack6;
		else if ( trackId == 6 )
			musicAudioSource.clip = startScreenMusic;
		
		int musicEnabled = PlayerPrefs.GetInt(PreferencesScript.TAG_MUSIC, (int)PreferencesScript.Music.On);
		if ( musicEnabled == (int)PreferencesScript.Music.On )
		{
			musicAudioSource.Play();
		}//end if
	}
	
	private AudioSource addAudioSource(AudioClip clip, bool loop, bool playOnAwake, float volume, float pitch)
	{
		AudioSource newAudioSource = this.gameObject.AddComponent<AudioSource>();
		newAudioSource.clip = clip;
		newAudioSource.loop = loop;
		newAudioSource.playOnAwake = playOnAwake;
		newAudioSource.volume = volume;
		newAudioSource.pitch = pitch;
		return newAudioSource;
	}
}
