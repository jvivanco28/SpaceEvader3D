using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LevelSurvivalAll : LevelDescription 
{
	// this level goes forever (only ends when you die)
	public const int LEVEL_LENGTH = 0;
	
	// number of tunnel blocks to be genereated before we can switch to a new "zone"
	public const int NUM_BLOCKS_BEFORE_LEVEL_SWITCH = 250;
	
	// switching out skyboxes temporarily freezes the game so we'll give the player
	// a "grace" period so that when the game freezes, they won't crash into anything 
	// (the grace period means that the tunnel blocks generated will contain no obstacle)
	public const int OUTLINE_ONLY_SECTION = NUM_BLOCKS_BEFORE_LEVEL_SWITCH - 10;
	public const int TUNNEL_ONLY_SECTION = NUM_BLOCKS_BEFORE_LEVEL_SWITCH - 25;
	
//	public const float 	SPEED_INCREMENT_EXTREME = 1.5f;
//	public const float 	SPEED_INCREMENT_HARD = 1.25f;
//	public const float	SPEED_INCREMENT_NORMAL = 1f;
//	public const float 	SPEED_INCREMENT_EASY = 0.75f;

	// these were iPad speeds
//	public const int MAX_SPEED_EXTREME = 60;
//	public const int MAX_SPEED_HARD = 57; // 1.2
//	public const int MAX_SPEED_NORMAL = 52;
//	public const int MAX_SPEED_EASY = 47;
//	public const int MIN_SPEED = 25; // 0.3

	// stand-alone PC version speeds
	public const int MAX_SPEED = 50;
	public const int MIN_SPEED = 20; // 0.3
	
	public int[] zoneStartingSpeeds = {20,20,20,20,20,20};
//	public float[] zoneSpeedIncrements = {0.8f, 1f, 1.15f, 1.3f, 1.45f, 1.6f};
	public float[] zoneSpeedIncrements = {0.8f, 1f, 1.2f, 1.4f, 1.5f, 1.6f};

	public AudioManager audioManager;
	
	private bool isIndoorOnly;
	private bool isFirstLevel;
	private int currZoneIndex;
	private bool gameFinished;

//	private int difficultly;
	
	private int[] zones;
	
	public override void init(GameObject level)
	{
		this.level = level;
				
//		difficultly = 0;
		preLoadSkyBoxes();
//		randomizeSkyBoxOrder();
		setupZones();
		setTunnelBlockRanges();
		setOutsideObjectRanges();
		setPowerUpRanges();
		switchSkyBox(zones[currZoneIndex]);
		setSpeeds();

//		// the max speed will be slower if we're using the head tracker, otherwise the game is
//		// pretty much impossible
//		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_CONTROLLER) == (int)IPlayerMove.Controller.EyeTracker )
//			maxLevelSpeed = MAX_SPEED_HEAD_TRACKER;
//		else
//			maxLevelSpeed = MAX_SPEED_EASY;
		
		isFirstLevel = true;
		isIndoorOnly = false;
		gameFinished = false;
		
		audioManager = GameObject.Find("Level").GetComponent<AudioManager>();
		switchMusicTrack();
	}
	
	private void setSpeeds()
	{
		// these won't ever change. we can probs move this to the init() method
		minLevelSpeed = MIN_SPEED;
		maxLevelSpeed = MAX_SPEED;

		speedIncrement = zoneSpeedIncrements [currZoneIndex];

//		if ( difficultly == 0 )
//		{
//			maxLevelSpeed = MAX_SPEED_EASY;
//			speedIncrement = SPEED_INCREMENT_EASY;
//		}
//		else if ( difficultly == 1 )
//		{
//			maxLevelSpeed = MAX_SPEED_NORMAL;
//			speedIncrement = SPEED_INCREMENT_NORMAL;
//		}
//		else if ( difficultly <= 6 )
//		{
//			maxLevelSpeed = MAX_SPEED_HARD;
//			speedIncrement = SPEED_INCREMENT_HARD;
//		}
//		else
//		{
//			maxLevelSpeed = MAX_SPEED_EXTREME;
//			speedIncrement = SPEED_INCREMENT_EXTREME;
//		}
	}
	
	private void switchMusicTrack()
	{
		if ( audioManager != null )
		{
			if ( currZoneIndex == 0 )
				audioManager.switchMusicTrack(AudioManager.ZONE_1_MUSIC_ID);
			else if ( currZoneIndex == 1 )
				audioManager.switchMusicTrack(AudioManager.ZONE_2_MUSIC_ID);
			else if ( currZoneIndex == 2 )
				audioManager.switchMusicTrack(AudioManager.ZONE_3_MUSIC_ID);
			else if ( currZoneIndex == 3 )
				audioManager.switchMusicTrack(AudioManager.ZONE_4_MUSIC_ID);
			else if ( currZoneIndex == 4 )
				audioManager.switchMusicTrack(AudioManager.ZONE_5_MUSIC_ID);
			else if ( currZoneIndex == 5 )
				audioManager.switchMusicTrack(AudioManager.ZONE_6_MUSIC_ID);
		}
	}
	
	private void setupZones()
	{		
		zones = new int[System.Enum.GetValues(typeof(SkyBoxes)).Length];
		zones[0] = (int)SkyBoxes.DeepSpaceBlue;
		zones[1] = (int)SkyBoxes.DeepSpaceGreenWithPlanet;
		zones[2] = (int)SkyBoxes.DeepSpaceRedWithPlanet;
//		zones[1] = (int)SkyBoxes.DeepSpaceBlueWithPlanet;
		zones[3] = (int)SkyBoxes.DeepSpaceGreen;
		zones[4] = (int)SkyBoxes.DeepSpaceRed;
		zones[5] = (int)SkyBoxes.Stars;
		currZoneIndex = PlayerPrefs.GetInt(PreferencesScript.TAG_START_ZONE, 0);
	}
	
//	private void randomizeSkyBoxOrder()
//	{
////		currZoneIndex = 6;
//
//		zones = new int[System.Enum.GetValues(typeof(SkyBoxes)).Length];
//		zones[0] = (int)SkyBoxes.DeepSpaceBlue;
//		zones[1] = (int)SkyBoxes.DeepSpaceBlueWithPlanet;
//		zones[2] = (int)SkyBoxes.DeepSpaceGreen;
//		zones[3] = (int)SkyBoxes.DeepSpaceGreenWithPlanet;
//		zones[4] = (int)SkyBoxes.DeepSpaceRed;
//		zones[5] = (int)SkyBoxes.DeepSpaceRedWithPlanet;
//		zones[6] = (int)SkyBoxes.Stars;
//				
//		int randomIndex;
//		int temp;
//		for ( int i = 0; i < zones.Length; i++ )
//		{
//			randomIndex = Random.Range(i, zones.Length);
//	
//			temp = zones[randomIndex];
//			zones[randomIndex] = zones[i];
//			zones[i] = temp;
//		}
//	}
	
	private int getBlankTunnelId()
	{
		int tunnelId;
		
		// TERRAN
		if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlue) )
			tunnelId = (int)(TunnelBlocks.Tunnel);

//		// MINE
//		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlueWithPlanet) )
//			tunnelId = (int)(TunnelBlocks.MineTunnel);
		
		// ALIEN
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreen) )
			tunnelId = (int)(TunnelBlocks.AlienTunnel);

		// WRECK
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreenWithPlanet) )
			tunnelId = (int)(TunnelBlocks.WreckTunnel);
		
		// TRICKSTER
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRed) )
			tunnelId = (int)(TunnelBlocks.TricksterTunnel);
		
		// MINE
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRedWithPlanet) )
			tunnelId = (int)(TunnelBlocks.MineTunnel);
			
		// ANJUNASPACE
		else
			tunnelId = (int)(TunnelBlocks.Outline);
		
		return tunnelId;
	}
	
	private int getNewTunnelDoor()
	{
		int tunnelId;
		
		// TERRAN
		if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlue) )
			tunnelId = (int)(TunnelBlocks.NewLevelDoor);

//		// MINE
//		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlueWithPlanet) )
//			tunnelId = (int)(TunnelBlocks.MineNewLevelDoor);
		
		// ALIEN
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreen) )
			tunnelId = (int)(TunnelBlocks.AlienNewLevelDoor);

		// WRECK
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreenWithPlanet) )
			tunnelId = (int)(TunnelBlocks.WreckNewLevelDoor);
		
		// TRICKSTER
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRed) )
			tunnelId = (int)(TunnelBlocks.TricksterNewLevelDoor);
		
		// MINE
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRedWithPlanet) )
			tunnelId = (int)(TunnelBlocks.MineNewLevelDoor);
			
		// ANJUNASPACE
		else
			tunnelId = (int)(TunnelBlocks.AnjunaNewLevelDoor);
		
		return tunnelId;
	}

	public override int getZoneStartingSpeed () {

		return zoneStartingSpeeds [currZoneIndex];
	}

	public override bool isGameFinished () 
	{
		return gameFinished;
	}

	// NOTE: this logic is soooo shitty. Might cause a headache.
	public override GameObject createRandomTunnelBlock()
	{
		GameObject newObj;

		// only blank tunnels when the game is finished
		if (gameFinished) {
			newObj = base.createTunnelBlock((int)TunnelBlocks.Blank, false);
		}
		else if ( base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH  == 0 )
		{
			newObj = base.createTunnelBlock(getNewTunnelDoor(), false);
		}
		// inside section (includes wormhole)
		else if ( isIndoorOnly )
		{
			if ( currZoneIndex != zones.Length - 1 &&  
			    (base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH == TUNNEL_ONLY_SECTION ||
					base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH == NUM_BLOCKS_BEFORE_LEVEL_SWITCH - 1) )
			{
				newObj = base.createTunnelBlock((int)TunnelBlocks.WormholeTunnel, false);	
			}
			else if ( base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH >= TUNNEL_ONLY_SECTION )
			{
				if ( currZoneIndex == zones.Length - 1 && base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH <= TUNNEL_ONLY_SECTION + 9) 
					newObj = base.createTunnelBlock((int)TunnelBlocks.Outline, true);	

				else if ( currZoneIndex == zones.Length - 1 )
					newObj = base.createTunnelBlock((int)TunnelBlocks.Blank, false);	

				else
					newObj = base.createTunnelBlock((int)TunnelBlocks.Blank, false);	
			}
			else if ( base.getNumBlocksCreated() % 2 != 0 )
			{
				newObj = base.createTunnelBlock(getBlankTunnelId(), true);
			}
			else
			{
				newObj = base.createRandomTunnelBlock();
			}
		}
		// outside section
		else
		{
			if ( base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH < DEFAULT_VISIBLE_TUNNEL_BLOCKS || 
				base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH > OUTLINE_ONLY_SECTION || 
				base.getNumBlocksCreated() % 2 != 0 )
			{
				if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRedWithPlanet) )
				{
					newObj = base.createTunnelBlock((int)TunnelBlocks.OctOutline, true);
				}
				else if (zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreenWithPlanet) )
				{
					newObj = base.createTunnelBlock((int)TunnelBlocks.WreckOutline, true);
				}
				else
				{
					newObj = base.createTunnelBlock((int)TunnelBlocks.Outline, true);
				}
			}
			else
			{
				newObj = base.createRandomTunnelBlock();
			}
		}
		
		// switch up the sky box when we need to
		if ( !isIndoorOnly && !isFirstLevel && base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH == DEFAULT_VISIBLE_TUNNEL_BLOCKS - 1 )
		{	
			switchSkyBox(zones[currZoneIndex]);
		}
		// switch up the skybox to something black at indoor sections so that we only see darkness at the end of the tunnel
		else if ( isIndoorOnly && base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH == DEFAULT_VISIBLE_TUNNEL_BLOCKS )
		{
			switchSkyBox((int)SkyBoxes.Stars);
		}
		
		// switch up the "level" once we've generated enough tunnel blocks
		if ( base.getNumBlocksCreated() % NUM_BLOCKS_BEFORE_LEVEL_SWITCH == 0)
		{
			isFirstLevel = false;

			// set only indoor tunnel blocks to hide switching the skybox out next time
			if ( !isIndoorOnly )
			{
				setIndoorTunnelsOnly();
				isIndoorOnly = true;
			}
			else
			{
				// increase difficulty
//				difficultly++;
				
//				int numberOfUnlockedZones = getNumberOfUnlockedZones();
//				currZoneIndex = (currZoneIndex + 1) % numberOfUnlockedZones;
				currZoneIndex++;
				if (currZoneIndex >= zones.Length ) 
				{
					currZoneIndex = zones.Length - 1;
					gameFinished = true;
				}
				else
				{
					// set the tunnel blocks back to default values
					setTunnelBlockRanges();
					setOutsideObjectRanges();
					setPowerUpRanges();
					isIndoorOnly = false;
					switchMusicTrack();
					setSpeeds();
				} 
			}
		}
		
		return newObj;
	}
	
	// Only use zones that have been unlocked
	private int getNumberOfUnlockedZones()
	{
		int highestScore;
		int highScore = PlayerPrefs.GetInt("HighScore", 0);
		int currScore = GameObject.Find("Level").GetComponent<LevelManagerScript>().getCurrentScore();
		
		if ( currScore > highScore )
			highestScore = currScore;
		else
			highestScore = highScore;
		
		int numberOfUnlockedZones = 1;
		
		if ( highestScore < ZONE_2_UNLOCK )
			numberOfUnlockedZones = 1;
		else if ( highestScore < ZONE_3_UNLOCK )
			numberOfUnlockedZones = 2;
		else if ( highestScore < ZONE_4_UNLOCK )
			numberOfUnlockedZones = 3;
		else if ( highestScore < ZONE_5_UNLOCK )
			numberOfUnlockedZones = 4;
		else if ( highestScore < ZONE_6_UNLOCK )
			numberOfUnlockedZones = 5;
		else
			numberOfUnlockedZones = 6;
		
		return numberOfUnlockedZones;
	}
	
	public override int getLevelLength()
	{
		return LEVEL_LENGTH;
	}
	
	public override void setTunnelBlockRanges()
	{
		if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlue) )
		{
			Debug.Log("Using DeepSpaceBlue obstacles");
			
			// START
//			tunnelBlockRanges = new LevelObjectRange[8];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.3f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.4f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.6f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.7f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.DoubleBlock, 0.8f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.9f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.99f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
			
//			// asteroids and debris
//			tunnelBlockRanges = new LevelObjectRange[6];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.4f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.6f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.8f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.DoubleBeam, 0.99f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);

			// TERRAN
			tunnelBlockRanges = new LevelObjectRange[13];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.273f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.345f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.418f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.49f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.564f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.MultiBlock, 0.636f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.71f);
			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.782f);
			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.855f);
			tunnelBlockRanges[10] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.927f);
			tunnelBlockRanges[11] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.99f);
			tunnelBlockRanges[12] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
		}
//		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlueWithPlanet) )
//		{
//			Debug.Log("Using DeepSpaceBlueWithPlanet obstacles");
//			
//			// Mine
//			tunnelBlockRanges = new LevelObjectRange[10];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.OctOutline, 0.2f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.MineDoubleBeamTunnel, 0.3f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.MineDoubleDoorTunnel, 0.4f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MineDoubleLaserTunnel, 0.5f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.MineQuadLaserTunnel, 0.6f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.MineSingleBeamTunnel, 0.7f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.MineLaserWall, 0.8f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.OctSingleAsteroid, 0.9f);
//			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.OctDoubleAsteroid, 0.99f);
//			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.MinePowerUpTunnel, 1f);
//		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreen) )
		{
			Debug.Log("Using DeepSpaceGreen obstacles");

			// ALIEN
			tunnelBlockRanges = new LevelObjectRange[10];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.AlienDoubleBeamTunnel, 0.3f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.AlienDoubleLaserTunnel, 0.4f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.AlienCubeLaserTunnel, 0.5f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.AlienLaserWallTunnel, 0.6f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.AlienMultiCubeTunnel, 0.7f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.AlientCubeFloaterTunnel, 0.8f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.9f);
			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.99f);
			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.AlienPowerUpTunnel, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreenWithPlanet) )
		{
			Debug.Log("Using DeepSpaceGreenWithPlanet obstacles");
			
			// SPACE WRECK
			tunnelBlockRanges = new LevelObjectRange[9];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.WreckOutline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.WreckBlockOutline, 0.314f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.WreckDoorOutline, 0.429f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.WreckLaserOutline, 0.543f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.WreckOctLaserOutline, 0.657f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.WreckTransporterOutline, 0.771f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.OctDoubleAsteroid, 0.886f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.OctSingleAsteroid, 0.99f);
			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.WreckPowerUpTunnel, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRed) )
		{
			Debug.Log("Using DeepSpaceRed obstacles");
			
			// TRICKSTER
			tunnelBlockRanges = new LevelObjectRange[10];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.TricksterBarrierTunnel, 0.3f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.TricksterSingleBeamTunnel, 0.4f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.TricksterDoubleBeamTunnel, 0.5f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.TricksterManHoleTunnel, 0.6f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.TricksterLaserTunnel, 0.7f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.TricksterChainLaserTunnel, 0.8f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.9f);
			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.99f);
			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRedWithPlanet) )
		{
			Debug.Log("Using DeepSpaceRedWithPlanet obstacles");
			
			// Mine
			tunnelBlockRanges = new LevelObjectRange[10];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.OctOutline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.MineDoubleBeamTunnel, 0.3f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.MineDoubleDoorTunnel, 0.4f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MineDoubleLaserTunnel, 0.5f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.MineQuadLaserTunnel, 0.6f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.MineSingleBeamTunnel, 0.7f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.MineLaserWall, 0.8f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.OctSingleAsteroid, 0.9f);
			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.OctDoubleAsteroid, 0.99f);
			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.MinePowerUpTunnel, 1f);
		}
		else // stars
		{
			Debug.Log("Using Stars obstacles");
			
			// ANJUNASPACE
			tunnelBlockRanges = new LevelObjectRange[6];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.AnjunaLaserOutline, 0.4f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.AnjunaLaserTunnel, 0.6f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.AnjunaMultiCubeOutline, 0.8f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.AnjunaMultiCubeTunnel, 0.99f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1);
		}

//		else if ( difficulty == 1 )
//		{
//			tunnelBlockRanges = new LevelObjectRange[8];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.3f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.45f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.55f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.MultiBlock, 0.7f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.8f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.99f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
//		}
//		else if ( difficulty == 2 )
//		{
//			tunnelBlockRanges = new LevelObjectRange[14];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.26f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.33f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.40f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.DoubleLaserOutline, 0.46f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.DoubleLaserTunnel, 0.52f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.58f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.64f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.MultiBlock, 0.70f);
//			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.76f);
//			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.82f);
//			tunnelBlockRanges[10] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.88f);
//			tunnelBlockRanges[11] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.93f);
//			tunnelBlockRanges[12] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.99f);
//			tunnelBlockRanges[13] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
//		}
//		else
//		{
//			tunnelBlockRanges = new LevelObjectRange[15];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.1f);
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.16f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.23f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.30f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.DoubleLaserOutline, 0.36f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.DoubleLaserTunnel, 0.42f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.46f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.52f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.MultiBlock, 0.58f);
//			tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.64f);
//			tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.70f);
//			tunnelBlockRanges[10] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.77f);
//			tunnelBlockRanges[11] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.9f);
//			tunnelBlockRanges[12] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.99f);
//			tunnelBlockRanges[13] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1f);
//		}
	}
	
	private void setIndoorTunnelsOnly()
	{		
		if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlue) )
		{
//			// EASY			
//			tunnelBlockRanges = new LevelObjectRange[8];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Tunnel, 0.3f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.417f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserTunnel, 0.533f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.DoubleBlockTunnel, 0.65f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.767f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.883f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.99f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.PowerUpTunnel, 1f);
			
			// TERRAN
			tunnelBlockRanges = new LevelObjectRange[8];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Tunnel, 0.15f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.292f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserTunnel, 0.433f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.575f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.717f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.858f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.99f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.PowerUpTunnel, 1f);
		}
//		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceBlueWithPlanet) )
//		{
//			// Mine
//			tunnelBlockRanges = new LevelObjectRange[8];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.MineTunnel, 0.2f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.MineDoubleBeamTunnel, 0.333f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.MineDoubleDoorTunnel, 0.467f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MineDoubleLaserTunnel, 0.6f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.MineQuadLaserTunnel, 0.733f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.MineSingleBeamTunnel, 0.867f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.MineLaserWall, 0.99f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.MinePowerUpTunnel, 1f);
//		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreen) )
		{
			// ALIEN
			tunnelBlockRanges = new LevelObjectRange[8];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.AlienTunnel, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.AlienDoubleBeamTunnel, 0.333f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.AlienDoubleLaserTunnel, 0.466f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.AlienCubeLaserTunnel, 0.599f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.AlienLaserWallTunnel, 0.732f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.AlienMultiCubeTunnel, 0.865f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.AlientCubeFloaterTunnel, 0.99f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.AlienPowerUpTunnel, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceGreenWithPlanet) )
		{
			// SPACE WRECK
			tunnelBlockRanges = new LevelObjectRange[7];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.WreckTunnel, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.WreckBlockTunnel, 0.36f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.WreckDoorTunnel, 0.52f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.WreckLaserTunnel, 0.68f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.WreckOctLaserTunnel, 0.84f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.WreckTransporterTunnel, 0.99f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.WreckPowerUpTunnel, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRed) )
		{
			// TRICKSTER
			tunnelBlockRanges = new LevelObjectRange[8];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.TricksterTunnel, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.TricksterBarrierTunnel, 0.333f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.TricksterSingleBeamTunnel, 0.467f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.TricksterDoubleBeamTunnel, 0.6f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.TricksterManHoleTunnel, 0.733f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.TricksterLaserTunnel, 0.867f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.TricksterChainLaserTunnel, 0.99f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.TricksterPowerUpTunnel, 1f);
		}
		else if ( zones[currZoneIndex] == (int)(SkyBoxes.DeepSpaceRedWithPlanet) )
		{
			// Mine
			tunnelBlockRanges = new LevelObjectRange[8];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.MineTunnel, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.MineDoubleBeamTunnel, 0.333f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.MineDoubleDoorTunnel, 0.467f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MineDoubleLaserTunnel, 0.6f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.MineQuadLaserTunnel, 0.733f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.MineSingleBeamTunnel, 0.867f);
			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.MineLaserWall, 0.99f);
			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.MinePowerUpTunnel, 1f);
		}
		else // stars
		{
			// ANJUNASPACE (same as outdoor level)
			tunnelBlockRanges = new LevelObjectRange[6];
			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.2f);
			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.AnjunaLaserOutline, 0.4f);
			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.AnjunaLaserTunnel, 0.6f);
			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.AnjunaMultiCubeOutline, 0.8f);
			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.AnjunaMultiCubeTunnel, 0.99f);
			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.PowerUpSection, 1);
		}
		
		// no need for outside objects since we can't see them
		outsideObjectRanges = new LevelObjectRange[0];
		
//		// set the ranges for each tunnel block that we want to use for this level
//		if ( difficulty == 0 )
//		{
//
//		}
//		else if ( difficulty == 1 )
//		{
//			tunnelBlockRanges = new LevelObjectRange[8];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Tunnel, 0.15f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.292f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.SingleLaserTunnel, 0.433f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.575f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.717f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.858f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.99f);
//			tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.PowerUpTunnel, 1f);
//		}
//		else
//		{
//			tunnelBlockRanges = new LevelObjectRange[7];
//			tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.167f);
//			tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleLaserTunnel, 0.333f);
//			tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.MultiBlockTunnel, 0.5f);
//			tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 0.667f);
//			tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.833f);
//			tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.99f);
//			tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.PowerUpTunnel, 1f);
//		}
	}
	
	public override void setOutsideObjectRanges()
	{
		if ( zones[currZoneIndex] == (int)(SkyBoxes.Stars) )
		{
			// ANJUNASPACE
			outsideObjectRanges = new LevelObjectRange[2];
			outsideObjectRanges[0] = new LevelObjectRange((int)OutsideObjects.AnjunaBeam, 0.75f);
			outsideObjectRanges[1] = new LevelObjectRange((int)OutsideObjects.AnjunaOct, 1.0f);
		}
		else 
		{
			// everything else
			outsideObjectRanges = new LevelObjectRange[2];
			outsideObjectRanges[0] = new LevelObjectRange((int)OutsideObjects.Beam, 0.75f);
			outsideObjectRanges[1] = new LevelObjectRange((int)OutsideObjects.Octagon, 1.0f);
		}
	}
	
	public override void setPowerUpRanges()
	{
		powerUpCreateProbability = 0.07f;
		if ( currZoneIndex >= 3 )
		{
			powerUpRanges = new LevelObjectRange[7];
			powerUpRanges[0] = new LevelObjectRange((int)PowerUpScript.PowerUpType.Shield, 0.1f);
			powerUpRanges[1] = new LevelObjectRange((int)PowerUpScript.PowerUpType.Death, 0.2f);
			powerUpRanges[2] = new LevelObjectRange((int)PowerUpScript.PowerUpType.SpeedUp, 0.35f);
			powerUpRanges[3] = new LevelObjectRange((int)PowerUpScript.PowerUpType.SlowDown, 0.45f);
			powerUpRanges[4] = new LevelObjectRange((int)PowerUpScript.PowerUpType.RotateLeft, 0.7f);
			powerUpRanges[5] = new LevelObjectRange((int)PowerUpScript.PowerUpType.RotateRight, 0.95f);
			powerUpRanges[6] = new LevelObjectRange((int)PowerUpScript.PowerUpType.Invulnerable, 1f);
		}
		else
		{	
			powerUpRanges = new LevelObjectRange[6];
			powerUpRanges[0] = new LevelObjectRange((int)PowerUpScript.PowerUpType.Shield, 0.15f);
			powerUpRanges[1] = new LevelObjectRange((int)PowerUpScript.PowerUpType.SpeedUp, 0.30f);
			powerUpRanges[2] = new LevelObjectRange((int)PowerUpScript.PowerUpType.SlowDown, 0.45f);
			powerUpRanges[3] = new LevelObjectRange((int)PowerUpScript.PowerUpType.RotateLeft, 0.70f);
			powerUpRanges[4] = new LevelObjectRange((int)PowerUpScript.PowerUpType.RotateRight, 0.95f);
			powerUpRanges[5] = new LevelObjectRange((int)PowerUpScript.PowerUpType.Invulnerable, 1.0f);
		}
	}
}//end class
