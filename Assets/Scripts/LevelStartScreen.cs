using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelStartScreen : LevelDescription
{
	// make the destroy distance a bit further back si objects aren't destroyed too early
	private const float TUNNEL_DESTROY_DISTANCE = -100f;
	private AudioManager audioManager;
	
	public override float getTunnelDestroyDistance()
	{
		return TUNNEL_DESTROY_DISTANCE;
	}
	public override void init(GameObject level)
	{
		this.level = level;
		visibleTunnelBlocks = 22;
		setTunnelBlockRanges();
		setOutsideObjectRanges();
		setPowerUpRanges();
		maxLevelSpeed = DEFAULT_MAX_SPEED;
		minLevelSpeed = DEFAULT_MIN_SPEED;
		
		preLoadSkyBoxes();
				
		// Only use a skybox from zones that have been unlocked
		int highScore = PlayerPrefs.GetInt("HighScore", 0);
		int numberOfAvailableSkyboxes = 1;
		
		if ( highScore < ZONE_2_UNLOCK )
			numberOfAvailableSkyboxes = 1;
		else if ( highScore < ZONE_3_UNLOCK )
			numberOfAvailableSkyboxes = 2;
		else if ( highScore < ZONE_4_UNLOCK )
			numberOfAvailableSkyboxes = 3;
		else if ( highScore < ZONE_5_UNLOCK )
			numberOfAvailableSkyboxes = 4;
		else
			numberOfAvailableSkyboxes = 5;
			
		int randomSkyBoxId = Random.Range(0, numberOfAvailableSkyboxes);
		switchSkyBox(randomSkyBoxId);
		
		audioManager = GameObject.Find("Level").GetComponent<AudioManager>();
		if ( audioManager != null )
			audioManager.switchMusicTrack(AudioManager.START_SCREEN_MUSIC_ID);
	}
	public override int getLevelLength()
	{
		return 0;
	}
	public override void setTunnelBlockRanges()
	{
		tunnelBlockRanges = new LevelObjectRange[1];
		
		// set the ranges for each tunnel block that we want to use for this level
		tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 1f);
	}
	
	public override void setOutsideObjectRanges()
	{
		outsideObjectRanges = new LevelObjectRange[2];	
		outsideObjectRanges[0] = new LevelObjectRange((int)OutsideObjects.Beam, 0.75f);
		outsideObjectRanges[1] = new LevelObjectRange((int)OutsideObjects.Octagon, 1.0f);
	}
	
	public override void setPowerUpRanges()
	{
		powerUpCreateProbability = 0.0f;
		powerUpRanges = new LevelObjectRange[0];
	}

	public override int getZoneStartingSpeed ()
	{
		return 19;
	}

	public override bool isGameFinished () 
	{
		return false;
	}

//	public override void setMaxLevelSpeed()
//	{
//		maxLevelSpeed = 0.5f;
//	}
//	public override void setMinLevelSpeed()
//	{
//		minLevelSpeed = 0.4f;
//	}

}
