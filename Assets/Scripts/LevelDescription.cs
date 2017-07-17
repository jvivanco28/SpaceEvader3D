using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class LevelDescription : ScriptableObject
{
	// constants
	public const int 	BLOCK_SIZE = 10;
	public const float 	POWER_UP_POS = 3f;
	public const float 	DEFAULT_MIN_SPEED = 19f; // 19
	public const float 	DEFAULT_MAX_SPEED = 19f;
	public const float  DEFAUlT_SPEED_INCREMENT = 1.5f;
	
	public const int ZONE_2_UNLOCK = 1500;
	public const int ZONE_3_UNLOCK = 3500;
	public const int ZONE_4_UNLOCK = 6000;
	public const int ZONE_5_UNLOCK = 9000;
	public const int ZONE_6_UNLOCK = 12500;

	// new version doesn't have zone unlocks
//	public const int ZONE_2_UNLOCK = 0;
//	public const int ZONE_3_UNLOCK = 0;
//	public const int ZONE_4_UNLOCK = 0;
//	public const int ZONE_5_UNLOCK = 0;
//	public const int ZONE_6_UNLOCK = 0;
	
	// tunnel blocks are deleted once they are behind this many units
	// (need to call method to access this member to allow subclasses to override)
	protected const int DEFAULT_TUNNEL_DESTROY_DISTANCE = -20;
	protected const int DEFAULT_VISIBLE_TUNNEL_BLOCKS = 16;
	
	// can only create an outside object after this many tunnel blocks have been created
	public const int OUTSIDE_OBJ_WAIT_PERIOD = 4;
	
	// speed increases after this many tunnels have passed
	public const int NUM_TUNNELS_PER_SPEED_INCREASE = 10;
	
	// the start of the tunnel will be this many units in front of the player when the game first starts
	public const int START_TUNNEL_DEPTH = 80;
	
	// tunnel block model paths
	public const string OUTLINE_OBJ_PATH = 				"TunnelBlocks/Misc/TunnelOutline";
	public const string OCT_OUTLINE_OBJ_PATH = 			"TunnelBlocks/Misc/OctTunnelOutline";
	public const string BLANK_OBJ_PATH = 				"TunnelBlocks/Misc/BlankTunnel";
	public const string WORMHOLE_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Misc/WormholeTunnel";	

	// TERRAN
	public const string NEW_LEVEL_DOOR_OBJ_PATH = 		"TunnelBlocks/Terran/NewLevelDoor";
	public const string POWER_UP_SECTION_OBJ_PATH = 	"TunnelBlocks/Terran/PowerUpTunnelOutline";
	public const string POWER_UP_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Terran/PowerUpTunnel";
	public const string DOUBLE_BEAM_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/DoubleBeamTunnel";
	public const string SINGLE_BEAM_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/SingleBeamTunnel";
	public const string LASER_WALL_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/LaserWallTunnel";
	public const string LASER_WALL_OBJ_PATH = 			"TunnelBlocks/Terran/LaserWallOutline";
	public const string SINGLE_DOOR_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/SingleDoorTunnel";
	public const string SINGLE_CUBE_OBJ_PATH = 			"TunnelBlocks/Terran/SingleBlockTunnelOutline";
	public const string SINGLE_CUBE_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/SingleBlockTunnel";
	public const string DOUBLE_CUBE_OBJ_PATH = 			"TunnelBlocks/Terran/DoubleBlockTunnelOutline";
	public const string DOUBLE_CUBE_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/DoubleBlockTunnel";
	public const string MULTI_CUBE_OBJ_PATH = 			"TunnelBlocks/Terran/MultiCubeTunnleOutline";
	public const string MULTI_CUBE_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/MultiCubeTunnel";
	public const string LASER_OBJ_PATH = 				"TunnelBlocks/Terran/LaserTunnelOutline";
	public const string DOUBLE_LASER_OBJ_PATH = 		"TunnelBlocks/Terran/DoubleLaserTunnelOutline";
	public const string LASER_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Terran/LaserTunnel";
	public const string DOUBLE_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Terran/DoubleLaserTunnel";
	public const string TUNNEL_OBJ_PATH = 				"TunnelBlocks/Terran/Tunnel";
	
	// ASTEROID
	public const string SINGLE_ASTEROID_OBJ_PATH = 		"TunnelBlocks/Asteroid/SingleAsteroidTunnelOutline";
	public const string DOUBLE_ASTEROID_OBJ_PATH = 		"TunnelBlocks/Asteroid/DoubleAsteroidTunnelOutline";
	public const string OCT_SINGLE_ASTEROID_OBJ_PATH = 		"TunnelBlocks/Asteroid/OctSingleAsteroidTunnelOutline";
	public const string OCT_DOUBLE_ASTEROID_OBJ_PATH = 		"TunnelBlocks/Asteroid/OctDoubleAsteroidTunnelOutline";
	public const string DOUBLE_BEAM_OBJ_PATH = 			"TunnelBlocks/Asteroid/DoubleBeamTunnelOutline";
	public const string SINGLE_BEAM_OBJ_PATH = 			"TunnelBlocks/Asteroid/SingleBeamTunnelOutline";
	
	// MINE
	public const string MINE_NEW_LEVEL_DOOR_OBJ_PATH = 		"TunnelBlocks/Mine/NewLevelDoor";
	public const string MINE_TUNNEL_OBJ_PATH = 				"TunnelBlocks/Mine/Tunnel";
	public const string MINE_DOUBLE_BEAM_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Mine/DoubleBeamTunnel";
	public const string MINE_DOUBLE_LASER_TUNNEL_OBJ_PATH =	"TunnelBlocks/Mine/DoubleLaserTunnel";
	public const string MINE_QUAD_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Mine/QuadLaserTunnel";
	public const string MINE_SINGLE_BEAM_TUNNEL_OBJ_PATH =	"TunnelBlocks/Mine/SingleBeamTunnel";
	public const string MINE_DOUBLE_DOOR_TUNNEL_OBJ_PATH =	"TunnelBlocks/Mine/DoubleDoorTunnel";
	public const string MINE_LASER_WALL_TUNNEL_OBJ_PATH =	"TunnelBlocks/Mine/LaserWallTunnel";
	public const string MINE_POWER_UP_TUNNEL_OBJ_PATH =		"TunnelBlocks/Mine/PowerUpTunnel";

	// ALIEN
	public const string ALIEN_NEW_LEVEL_DOOR_OBJ_PATH = 		"TunnelBlocks/Alien/NewLevelDoor";
	public const string ALIEN_TUNNEL_OBJ_PATH = 				"TunnelBlocks/Alien/Tunnel";
	public const string ALIEN_DOUBLE_BEAM_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Alien/DoubleBeamTunnel";
	public const string ALIEN_DOUBLE_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Alien/DoubleLaserTunnel";
	public const string ALIEN_CUBE_LASER_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Alien/CubeLaserTunnel";
	public const string ALIEN_LASER_WALL_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Alien/LaserWallTunnel";
	public const string ALIEN_MULTI_CUBE_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Alien/MultiCubeTunnel";
	public const string ALIEN_CUBE_FLOATER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Alien/CubeFloaterTunnel";
	public const string ALIEN_POWER_UP_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Alien/PowerUpTunnel";
	
	// TRICKSTER
	public const string TRICKSTER_NEW_LEVEL_DOOR_OBJ_PATH = 	"TunnelBlocks/Trickster/NewLevelDoor";
	public const string TRICKSTER_TUNNEL_OBJ_PATH = 			"TunnelBlocks/Trickster/Tunnel";
	public const string TRICKSTER_POWER_UP_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Trickster/PowerUpTunnel";
	public const string TRICKSTER_BARRIER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Trickster/BarrierTunnel";
	public const string TRICKSTER_SINGLE_BEAM_TUNNEL_OBJ_PATH = "TunnelBlocks/Trickster/SingleBeamTunnel";
	public const string TRICKSTER_DOUBLE_BEAM_TUNNEL_OBJ_PATH = "TunnelBlocks/Trickster/DoubleBeamTunnel";
	public const string TRICKSTER_MAN_HOLE_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Trickster/ManHoleTunnel";
	public const string TRICKSTER_LASER_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Trickster/LaserTunnel";
	public const string TRICKSTER_CHAIN_LASER_TUNNEL_OBJ_PATH = "TunnelBlocks/Trickster/ChainLaserTunnel";
	
	// WRECK
	public const string WRECK_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Wreck/Tunnel";
	public const string WRECK_OUTLINE_OBJ_PATH = 		"TunnelBlocks/Wreck/Outline";
	public const string WRECK_OCT_LASER_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Wreck/OctLaserOutline";
	public const string WRECK_OCT_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/OctLaserTunnel";
	public const string WRECK_LASER_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Wreck/LaserOutline";
	public const string WRECK_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/LaserTunnel";
	public const string WRECK_DOOR_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/DoorTunnel";
	public const string WRECK_DOOR_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Wreck/DoorOutline";
	public const string WRECK_BLOCK_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/BlockTunnel";
	public const string WRECK_BLOCK_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Wreck/BlockOutline";
	public const string WRECK_TRANSPORTER_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Wreck/TransporterOutline";
	public const string WRECK_TRANSPORTER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/TransporterTunnel";
	public const string WRECK_POWER_UP_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Wreck/PowerUpTunnel";
	public const string WRECK_NEW_LEVEL_DOOR_OBJ_PATH = 	"TunnelBlocks/Wreck/NewLevelDoor";
	
	// ANJUNASPACE
	public const string ANJUNA_NEW_LEVEL_DOOR_OBJ_PATH = "TunnelBlocks/Anjunaspace/NewLevelDoor";
	public const string ANJUNA_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Anjunaspace/Tunnel";
	public const string ANJUNA_BEAM_OUTLINE_OBJ_PATH = 	"TunnelBlocks/Anjunaspace/AnjunaBeamOutline";
	public const string ANJUNA_LASER_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Anjunaspace/LaserTunnel";
	public const string ANJUNA_LASER_OUTLINE_OBJ_PATH = "TunnelBlocks/Anjunaspace/LaserTunnelOutline";
	public const string ANJUNA_MULTI_CUBE_TUNNEL_OBJ_PATH = 	"TunnelBlocks/Anjunaspace/MultiCubeTunnel";
	public const string ANJUNA_MULTI_CUBE_OUTLINE_OBJ_PATH =	"TunnelBlocks/Anjunaspace/MultiCubeTunnelOutline";
	public const string ANJUNA_POWER_UP_TUNNEL_OBJ_PATH = 		"TunnelBlocks/Anjunaspace/PowerUpTunnel";

	// outside obj model paths
	public const string OCTAGON_OBJ_PATH = "OutsideObjects/Octagon";
	public const string BEAM_OBJ_PATH = "OutsideObjects/LogDebris";
	public const string ANJUNA_OCT_OBJ_PATH = "OutsideObjects/AnjunaOctagon";
	public const string ANJUNA_BEAM_OBJ_PATH = "OutsideObjects/AnjunaBeam";
	
	// powerup obj model paths
	public const string ROTATE_RIGHT_OBJ_PATH = "Pickups/RotateRight";
	public const string ROTATE_LEFT_OBJ_PATH = "Pickups/RotateLeft";
	public const string SLOW_DOWN_OBJ_PATH = "Pickups/SlowDown";
	public const string SPEED_UP_OBJ_PATH = "Pickups/SpeedUp";
	public const string SHIELD_OBJ_PATH = "Pickups/Shield";
	public const string INVULNERABILITY_OBJ_PATH = "Pickups/Invulnerability";
	public const string POINT_OBJ_PATH = "Pickups/Point";
	public const string DEATH_OBJ_PATH = "Pickups/DeathPickup";
	
	// skyboxes
	public const string DEEP_SPACE_BLUE_SKYBOX = "SkyBoxVolume2/DeepSpaceBlue/DSB";
//	public const string DEEP_SPACE_BLUE_WITH_PLANET_SKYBOX = "SkyBoxVolume2/DeepSpaceBlueWithPlanet/DSBWP";
	public const string DEEP_SPACE_GREEN_SKYBOX = "SkyBoxVolume2/DeepSpaceGreen/DSG";
	public const string DEEP_SPACE_GREEN_SKYBOX_WITH_PLANET = "SkyBoxVolume2/DeepSpaceGreenWithPlanet/DSGWP";
	public const string DEEP_SPACE_RED_SKYBOX = "SkyBoxVolume2/DeepSpaceRed/DSR";
	public const string DEEP_SPACE_RED_WITH_PLANET_SKYBOX = "SkyBoxVolume2/DeepsSpaceRedWithPlanet/DSRWP";
	public const string STARS_SKYBOX = "SkyBoxVolume2/Stars01/StarSkyBox";
	
	// misc paths
	public const string SPEED_3D_TEXT = "Misc/Speed3DText";
	public const string SLOW_3D_TEXT = "Misc/Slow3DText";
	public const string SHIELD_3D_TEXT = "Misc/Shield3DText";

	
	// vars
	protected Material[] skyboxes;
	protected int visibleTunnelBlocks;
	protected LevelObjectRange[] tunnelBlockRanges;
	protected LevelObjectRange[] outsideObjectRanges;
	protected LevelObjectRange[] powerUpRanges;

	protected float powerUpCreateProbability;
	protected int powerUpStreamPosition; // where the stream of points/powerups will land
	protected int numPowerUpsInStream; // how many points/powerups will go in a stream
	protected int currPowerUpInStream;
	
	protected GameObject level;
	protected GuiScript guiScript;
	protected int numBlocksCreated;
	protected float maxLevelSpeed; // fastest speed you can go according to the level
	protected float minLevelSpeed;
	protected float speedIncrement;
	
	// enums
	public enum LevelDescriptionId
	{
		StartScreen,
		SpaceWreak,
		HumanTrial
	}
//	

	
	public enum TunnelBlocks 
	{
		AlienTunnel,
		AlienNewLevelDoor,
		AlienDoubleBeamTunnel,
		AlienDoubleLaserTunnel,
		AlienCubeLaserTunnel,
		AlienLaserWallTunnel,
		AlienMultiCubeTunnel,
		AlienPowerUpTunnel,
		AlientCubeFloaterTunnel,
		
		MineTunnel,
		MineLaserWall,
		MineNewLevelDoor,
		MineDoubleBeamTunnel,
		MineDoubleLaserTunnel,
		MineQuadLaserTunnel,
		MineDoubleDoorTunnel,
		MineSingleBeamTunnel,
		MinePowerUpTunnel,
		
		TricksterTunnel,
		TricksterNewLevelDoor,
		TricksterPowerUpTunnel,
		TricksterBarrierTunnel,
		TricksterSingleBeamTunnel,
		TricksterDoubleBeamTunnel,
		TricksterManHoleTunnel,
		TricksterLaserTunnel,
		TricksterChainLaserTunnel,
		
		WreckOutline,
		WreckTunnel,
		WreckOctLaserOutline,
		WreckOctLaserTunnel,
		WreckLaserOutline,
		WreckLaserTunnel,
		WreckDoorTunnel,
		WreckDoorOutline,
		WreckBlockTunnel,
		WreckBlockOutline,
		WreckTransporterOutline,
		WreckTransporterTunnel,
		WreckPowerUpTunnel,
		WreckNewLevelDoor,

		AnjunaTunnel,
		AnjunaNewLevelDoor,
		AnjunaBeam,
		AnjunaLaserTunnel,
		AnjunaLaserOutline,
		AnjunaMultiCubeTunnel,
		AnjunaMultiCubeOutline,
		AnjunaPowerUpTunnel,
		
		SingleAsteroid,
		DoubleAsteroid,
		OctSingleAsteroid,
		OctDoubleAsteroid,
		SingleBeam,
		DoubleBeam,

		WormholeTunnel,
		PowerUpSection,
		PowerUpTunnel,
		NewLevelDoor,
		SingleBeamTunnel,
		DoubleBeamTunnel,
		LaserWallTunnel,
		LaserWallOutline,
		SingleDoorTunnel,
		SingleBlock,
		SingleBlockTunnel,
		DoubleBlock,
		DoubleBlockTunnel,
		MultiBlock,
		MultiBlockTunnel,
		SingleLaserOutline,
		DoubleLaserOutline,
		SingleLaserTunnel,
		DoubleLaserTunnel,
		
		Outline,
		OctOutline,
		Tunnel,
		Blank
	}
	
	public enum OutsideObjects 
	{
		Octagon,
		Beam,
		AnjunaBeam,
		AnjunaOct
	}
	
	public enum SkyBoxes
	{
		DeepSpaceBlue,
//		DeepSpaceBlueWithPlanet,
		DeepSpaceGreen,
		DeepSpaceGreenWithPlanet,
		DeepSpaceRed,
		DeepSpaceRedWithPlanet,
		Stars
	}
	
	// methods
	public abstract int getLevelLength();
	public abstract void setTunnelBlockRanges();
	public abstract void setOutsideObjectRanges();
	public abstract void setPowerUpRanges();
	public abstract int getZoneStartingSpeed ();
	public abstract bool isGameFinished ();
//	public abstract void setMaxLevelSpeed();
//	public abstract void setMinLevelSpeed();
	
	public virtual float getTunnelDestroyDistance()
	{
		return DEFAULT_TUNNEL_DESTROY_DISTANCE;
	}

	// NOTE: you can't call a specific constructor with ScriptableObjects so
	// we need an init() method
	public virtual void init(GameObject level)
	{
		this.level = level;
		
		visibleTunnelBlocks = DEFAULT_VISIBLE_TUNNEL_BLOCKS;
		preLoadSkyBoxes();

		// default level description (in case the subclass forgets to create it's own)
		tunnelBlockRanges = new LevelObjectRange[1];
		tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 1f);
		
		// TODO: make sure this is fail proof
		outsideObjectRanges = new LevelObjectRange[0];
		powerUpRanges = new LevelObjectRange[0];
		powerUpCreateProbability = 0f;
		maxLevelSpeed = DEFAULT_MAX_SPEED;
		minLevelSpeed = DEFAULT_MIN_SPEED;
		speedIncrement = DEFAUlT_SPEED_INCREMENT;
		
		resetPowerUpStream();
	}
	
	private void resetPowerUpStream() 
	{
		powerUpStreamPosition = Random.Range(0,10000) % 8;
		numPowerUpsInStream = Random.Range (5,20);
		currPowerUpInStream = 0;
	}
	
	public virtual LinkedList<GameObject> initLevel()
	{
		// create initial tunnel blocks
		LinkedList<GameObject> currTunnels = new LinkedList<GameObject>();
		GameObject tunnelBlock;
		float currZ;
		
		if ( visibleTunnelBlocks == 0 )
			visibleTunnelBlocks = DEFAULT_VISIBLE_TUNNEL_BLOCKS;
		
		// start BEHIND the player and create tunnel blocks in the forward direction
		for ( int i = 0; i < visibleTunnelBlocks; i++ ) 
		{
			// always start with the tunnel outlines as we fly into the map
			tunnelBlock = createTunnelBlock((int)TunnelBlocks.Outline, false);	
			currTunnels.AddLast(tunnelBlock);
			
			// push the tunnel back in the Z direction
			currZ = (i * BLOCK_SIZE) + START_TUNNEL_DEPTH;
			tunnelBlock.transform.localPosition = new Vector3(0, 0, currZ);
		}//end for
		
		numBlocksCreated = 0;
		
		return currTunnels;
	}
	
	public virtual GameObject createRandomTunnelBlock()
	{
		int tunnelId = -1;
		float random = Random.Range(0f, 1f);
		
		// go thru each probability to see which tunnel block we should create
		for ( int i = 0; i < tunnelBlockRanges.Length && tunnelId == -1; i++ )
		{
			if ( random <= tunnelBlockRanges[i].getRange() )
				tunnelId = tunnelBlockRanges[i].getLevelObjId();
		}
		// if we didn't get anything (this shouldn't happen if the ranges were typed in correctly)
		// then just grab a default tunnelblock
		if ( tunnelId < 0 )
		{
			Debug.Log("TunnelBlock Ranges not set correctly! Using default tunnel block.");
			tunnelId = (int)TunnelBlocks.Outline;
		}
		
		GameObject newTunnelBlock = createTunnelBlock(tunnelId, true);

		return newTunnelBlock;
	}
	
	public GameObject createRandomOutsideObject()
	{
		GameObject newOutsideObject = null;

		if ( numBlocksCreated % OUTSIDE_OBJ_WAIT_PERIOD == 0 )
		{
			int outsideObjId = -1;
			float random = Random.Range(0f, 1f);
			
			// go thru each probability to see which tunnel block we should create
			for ( int i = 0; i < outsideObjectRanges.Length && outsideObjId == -1; i++ )
			{
				if ( random <= outsideObjectRanges[i].getRange() )
					outsideObjId = outsideObjectRanges[i].getLevelObjId();
			}
			
			// create the object if we've got a valid ID
			if ( outsideObjId >= 0 )
				newOutsideObject = createOutsideObject(outsideObjId);
		}
		return newOutsideObject;
	}
	
	public float getMaxLevelSpeed()
	{
		return maxLevelSpeed;
	}
	
	public float getMinLevelSpeed()
	{
		return minLevelSpeed;
	}
	
	public int getNumBlocksCreated()
	{
		return numBlocksCreated;
	}
	
	public virtual int getNumTunnelsPerSpeedIncrease()
	{
		return NUM_TUNNELS_PER_SPEED_INCREASE;
	}
	
	public float getSpeedIncrement() 
	{
		return speedIncrement;
	}

	
	// *************************** PRIVATE METHODS *************************** //
	
	
	// create the tunnelBlock specified, and place it at the back of the tunnel
	protected GameObject createTunnelBlock(int tunnelBlock, bool possiblePickup) 
	{
		numBlocksCreated++;
		
		// figure out the name of the prefab that we want to initialize
		string tunnelIdPath;
		TunnelBlocks newTunnelBlockId = ((TunnelBlocks)tunnelBlock);
		
		if ( newTunnelBlockId == TunnelBlocks.PowerUpSection )
			tunnelIdPath = POWER_UP_SECTION_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.PowerUpTunnel )
			tunnelIdPath = POWER_UP_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.NewLevelDoor )
			tunnelIdPath = NEW_LEVEL_DOOR_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleBeamTunnel )
			tunnelIdPath = SINGLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleBeamTunnel )
			tunnelIdPath = DOUBLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.LaserWallTunnel )
			tunnelIdPath = LASER_WALL_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.LaserWallOutline )
			tunnelIdPath = LASER_WALL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleDoorTunnel )
			tunnelIdPath = SINGLE_DOOR_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleBlock )
			tunnelIdPath = SINGLE_CUBE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleBlockTunnel )
			tunnelIdPath = SINGLE_CUBE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleBlock )
			tunnelIdPath = DOUBLE_CUBE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleBlockTunnel )
			tunnelIdPath = DOUBLE_CUBE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MultiBlock )
			tunnelIdPath = MULTI_CUBE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MultiBlockTunnel )
			tunnelIdPath = MULTI_CUBE_TUNNEL_OBJ_PATH;	
		else if ( newTunnelBlockId == TunnelBlocks.SingleLaserOutline )
			tunnelIdPath = LASER_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleLaserTunnel )
			tunnelIdPath = LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleLaserOutline )
			tunnelIdPath = DOUBLE_LASER_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleLaserTunnel )
			tunnelIdPath = DOUBLE_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.Tunnel )
			tunnelIdPath = TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WormholeTunnel )
			tunnelIdPath = WORMHOLE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.Blank )
			tunnelIdPath = BLANK_OBJ_PATH;
		
		else if ( newTunnelBlockId == TunnelBlocks.SingleBeam )
			tunnelIdPath = SINGLE_BEAM_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleBeam )
			tunnelIdPath = DOUBLE_BEAM_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.SingleAsteroid )
			tunnelIdPath = SINGLE_ASTEROID_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.DoubleAsteroid )
			tunnelIdPath = DOUBLE_ASTEROID_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.OctSingleAsteroid )
			tunnelIdPath = OCT_SINGLE_ASTEROID_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.OctDoubleAsteroid )
			tunnelIdPath = OCT_DOUBLE_ASTEROID_OBJ_PATH;

		
		else if ( newTunnelBlockId == TunnelBlocks.MineTunnel )
			tunnelIdPath = MINE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineNewLevelDoor )
			tunnelIdPath = MINE_NEW_LEVEL_DOOR_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineDoubleBeamTunnel )
			tunnelIdPath = MINE_DOUBLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineLaserWall )
			tunnelIdPath = MINE_LASER_WALL_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineDoubleLaserTunnel )
			tunnelIdPath = MINE_DOUBLE_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineDoubleDoorTunnel )
			tunnelIdPath = MINE_DOUBLE_DOOR_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineQuadLaserTunnel )
			tunnelIdPath = MINE_QUAD_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MineSingleBeamTunnel )
			tunnelIdPath = MINE_SINGLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.MinePowerUpTunnel )
			tunnelIdPath = MINE_POWER_UP_TUNNEL_OBJ_PATH;
		
		else if ( newTunnelBlockId == TunnelBlocks.TricksterTunnel )
			tunnelIdPath = TRICKSTER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterNewLevelDoor )
			tunnelIdPath = TRICKSTER_NEW_LEVEL_DOOR_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterPowerUpTunnel )
			tunnelIdPath = TRICKSTER_POWER_UP_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterBarrierTunnel )
			tunnelIdPath = TRICKSTER_BARRIER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterSingleBeamTunnel )
			tunnelIdPath = TRICKSTER_SINGLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterDoubleBeamTunnel )
			tunnelIdPath = TRICKSTER_DOUBLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterManHoleTunnel )
			tunnelIdPath = TRICKSTER_MAN_HOLE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterLaserTunnel )
			tunnelIdPath = TRICKSTER_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.TricksterChainLaserTunnel )
			tunnelIdPath = TRICKSTER_CHAIN_LASER_TUNNEL_OBJ_PATH;
		
		else if ( newTunnelBlockId == TunnelBlocks.AlienNewLevelDoor )
			tunnelIdPath = ALIEN_NEW_LEVEL_DOOR_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienDoubleBeamTunnel )
			tunnelIdPath = ALIEN_DOUBLE_BEAM_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienDoubleLaserTunnel )
			tunnelIdPath = ALIEN_DOUBLE_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienCubeLaserTunnel )
			tunnelIdPath = ALIEN_CUBE_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienLaserWallTunnel )
			tunnelIdPath = ALIEN_LASER_WALL_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienMultiCubeTunnel )
			tunnelIdPath = ALIEN_MULTI_CUBE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienPowerUpTunnel )
			tunnelIdPath = ALIEN_POWER_UP_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlientCubeFloaterTunnel )
			tunnelIdPath = ALIEN_CUBE_FLOATER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AlienTunnel )
			tunnelIdPath = ALIEN_TUNNEL_OBJ_PATH;
		
		else if ( newTunnelBlockId == TunnelBlocks.WreckOutline )
			tunnelIdPath = WRECK_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckTunnel )
			tunnelIdPath = WRECK_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckOctLaserOutline )
			tunnelIdPath = WRECK_OCT_LASER_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckOctLaserTunnel )
			tunnelIdPath = WRECK_OCT_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckLaserOutline )
			tunnelIdPath = WRECK_LASER_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckLaserTunnel )
			tunnelIdPath = WRECK_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckDoorTunnel )
			tunnelIdPath = WRECK_DOOR_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckDoorOutline )
			tunnelIdPath = WRECK_DOOR_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckBlockTunnel )
			tunnelIdPath = WRECK_BLOCK_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckBlockOutline )
			tunnelIdPath = WRECK_BLOCK_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckTransporterOutline )
			tunnelIdPath = WRECK_TRANSPORTER_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckTransporterTunnel )
			tunnelIdPath = WRECK_TRANSPORTER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckPowerUpTunnel )
			tunnelIdPath = WRECK_POWER_UP_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.WreckNewLevelDoor )
			tunnelIdPath = WRECK_NEW_LEVEL_DOOR_OBJ_PATH;
		
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaBeam )
			tunnelIdPath = ANJUNA_BEAM_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaNewLevelDoor )
			tunnelIdPath = ANJUNA_NEW_LEVEL_DOOR_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaLaserOutline )
			tunnelIdPath = ANJUNA_LASER_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaLaserTunnel )
			tunnelIdPath = ANJUNA_LASER_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaMultiCubeOutline )
			tunnelIdPath = ANJUNA_MULTI_CUBE_OUTLINE_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaMultiCubeTunnel )
			tunnelIdPath = ANJUNA_MULTI_CUBE_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaTunnel )
			tunnelIdPath = ANJUNA_TUNNEL_OBJ_PATH;
		else if ( newTunnelBlockId == TunnelBlocks.AnjunaPowerUpTunnel )
			tunnelIdPath = ANJUNA_POWER_UP_TUNNEL_OBJ_PATH;
				
		else if ( newTunnelBlockId == TunnelBlocks.OctOutline )
			tunnelIdPath = OCT_OUTLINE_OBJ_PATH;
		else
			tunnelIdPath = OUTLINE_OBJ_PATH;
				
		// create the game object and place it at the end of the tunnel by default
		GameObject newTunnelBlock = (GameObject)Instantiate(Resources.Load(tunnelIdPath));
		newTunnelBlock.transform.parent = level.transform.FindChild("Tunnels");
		newTunnelBlock.transform.localRotation = newTunnelBlock.transform.parent.transform.localRotation;
		
		// "maybe" create a new power up
		if ( possiblePickup && 
			tunnelBlock == (int)TunnelBlocks.Outline || 
			tunnelBlock == (int)TunnelBlocks.OctOutline || 
			tunnelBlock == (int)TunnelBlocks.Tunnel ||
			tunnelBlock == (int)TunnelBlocks.AlienTunnel ||
			tunnelBlock == (int)TunnelBlocks.TricksterTunnel ||
			tunnelBlock == (int)TunnelBlocks.MineTunnel ||
			tunnelBlock == (int)TunnelBlocks.AnjunaTunnel || 
			tunnelBlock == (int)TunnelBlocks.WreckTunnel ||
			tunnelBlock == (int)TunnelBlocks.WreckOutline ) 
			
			createRandomPowerUp(newTunnelBlock);
		
		return newTunnelBlock;
	}//end getRandomTunnelBlock
	
	// place power up in the tunnel block passed as a param
	protected GameObject createRandomPowerUp(GameObject tunnelBlock)
	{
		GameObject newPowerUp = null;
		
		int powerUpId = -1;
		float random = Random.Range(0f, 1.0f);
		
		// see if we should generate a powerup at all
		if ( random <= powerUpCreateProbability )
		{
			//... if so, which one?
			random = Random.Range(0f, 1.0f);
			for ( int i = 0; i < powerUpRanges.Length && powerUpId == -1; i++ )
			{
				if ( random <= powerUpRanges[i].getRange() )
					powerUpId = powerUpRanges[i].getLevelObjId();
			}
		}
		// we'll place a point "power up" only for every second tunnel block
		else if ( numBlocksCreated % 2 == 0 )
		{
			powerUpId = (int)PowerUpScript.PowerUpType.Point;			
		}

		if ( powerUpId >= 0 )
		{
			if ( currPowerUpInStream > numPowerUpsInStream )
				resetPowerUpStream();
			
			newPowerUp = createPowerUp(powerUpId);
			if ( newPowerUp != null )
			{
				newPowerUp.transform.parent = tunnelBlock.transform;
				newPowerUp.transform.localEulerAngles = new Vector3(0,0,0);
	
				// Figure out which quadrant to put the power up in
				if ( powerUpStreamPosition == 0 )
					newPowerUp.transform.localPosition = new Vector3(-POWER_UP_POS, POWER_UP_POS, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 1 )
					newPowerUp.transform.localPosition = new Vector3(0, POWER_UP_POS, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 2 )
					newPowerUp.transform.localPosition = new Vector3(POWER_UP_POS, POWER_UP_POS, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 3 )
					newPowerUp.transform.localPosition = new Vector3(-POWER_UP_POS, 0, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 4 )
					newPowerUp.transform.localPosition = new Vector3(0, 0, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 5 )
					newPowerUp.transform.localPosition = new Vector3(POWER_UP_POS, 0, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 6 )
					newPowerUp.transform.localPosition = new Vector3(-POWER_UP_POS, -POWER_UP_POS, -POWER_UP_POS);
				else if ( powerUpStreamPosition == 7 )
					newPowerUp.transform.localPosition = new Vector3(0, -POWER_UP_POS, -POWER_UP_POS);
				else
					newPowerUp.transform.localPosition = new Vector3(POWER_UP_POS, -POWER_UP_POS, -POWER_UP_POS);
				
				// we have to do some extra stuff for the rotate power ups so that they are presented in a way
				// that isn't confusing (the player needs to be able to tell which way they'll rotate)
				if ( powerUpId == (int)PowerUpScript.PowerUpType.RotateRight || powerUpId == (int)PowerUpScript.PowerUpType.RotateLeft)
				{
					if ( powerUpStreamPosition == 1 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, -45);
					else if ( powerUpStreamPosition == 2 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, -90);
					else if ( powerUpStreamPosition == 3 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, 45);
					else if ( powerUpStreamPosition == 5 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, -135);
					else if ( powerUpStreamPosition == 6)
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, 90);
					else if ( powerUpStreamPosition == 7 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, 135);
					else if ( powerUpStreamPosition == 8 )
						newPowerUp.transform.localEulerAngles = new Vector3(0, 0, 180);
				}
			}//end if
			
			currPowerUpInStream++;
		}//end if
		return newPowerUp;
	}
	
	protected GameObject createPowerUp(int powerUpId)
	{
		PowerUpScript.PowerUpType powerUp = ((PowerUpScript.PowerUpType)powerUpId);
		GameObject newPowerUp = null;
		if ( powerUp == PowerUpScript.PowerUpType.RotateLeft )
			newPowerUp = (GameObject)Instantiate(Resources.Load(ROTATE_LEFT_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.RotateRight )
			newPowerUp = (GameObject)Instantiate(Resources.Load(ROTATE_RIGHT_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.SlowDown )
			newPowerUp = (GameObject)Instantiate(Resources.Load(SLOW_DOWN_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.Shield )
			newPowerUp = (GameObject)Instantiate(Resources.Load(SHIELD_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.Invulnerable )
			newPowerUp = (GameObject)Instantiate(Resources.Load(INVULNERABILITY_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.SpeedUp )
			newPowerUp = (GameObject)Instantiate(Resources.Load(SPEED_UP_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.Point )
			newPowerUp = (GameObject)Instantiate(Resources.Load(POINT_OBJ_PATH));
		else if ( powerUp == PowerUpScript.PowerUpType.Death )
			newPowerUp = (GameObject)Instantiate(Resources.Load(DEATH_OBJ_PATH));		
		return newPowerUp;
	}
	
	protected GameObject createOutsideObject(int outsideObjId)
	{
		// figure out the name of the prefab that we want to initialize
		string objectPath = null;
		
		if ( outsideObjId == (int)OutsideObjects.Octagon )
			objectPath = OCTAGON_OBJ_PATH;
		else  if ( outsideObjId == (int)OutsideObjects.Beam )
			objectPath = BEAM_OBJ_PATH;
		else  if ( outsideObjId == (int)OutsideObjects.AnjunaBeam )
			objectPath = ANJUNA_BEAM_OBJ_PATH;
		else  if ( outsideObjId == (int)OutsideObjects.AnjunaOct )
			objectPath = ANJUNA_OCT_OBJ_PATH;
		
		GameObject newOutsideObject = null;
		if ( objectPath != null )
		{
			// create the game object and place it at the end of the tunnel by default
			newOutsideObject = (GameObject)Instantiate(Resources.Load(objectPath));
			newOutsideObject.transform.parent = level.transform.FindChild("NonTunnels");
			newOutsideObject.transform.localRotation = newOutsideObject.transform.parent.transform.localRotation;
		}
		
		return newOutsideObject;
	}//end createOutsideObject
	
	protected Material createSkyBox(int skyboxId)
	{
		Material skybox;
		
		if ( skyboxId == (int)SkyBoxes.DeepSpaceBlue )
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_BLUE_SKYBOX));
//		else if ( skyboxId == (int)SkyBoxes.DeepSpaceBlueWithPlanet )
//			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_BLUE_WITH_PLANET_SKYBOX));
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceGreen )
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_GREEN_SKYBOX));
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceGreenWithPlanet )
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_GREEN_SKYBOX_WITH_PLANET));
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceRed )
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_RED_SKYBOX));
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceRedWithPlanet )
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.DEEP_SPACE_RED_WITH_PLANET_SKYBOX));
		else 
			skybox = (Material)Instantiate(Resources.Load(LevelDescription.STARS_SKYBOX));
		
		return skybox;
	}
	
	protected void preLoadSkyBoxes()
	{
		skyboxes = new Material[System.Enum.GetValues(typeof(SkyBoxes)).Length];
		
		// this will cache the skyboxes (I think/I hope) so that when we load them, they won't take up much time.
		// Unity doesn't support multi threading, so that's why we have to hack this together =\
		for (int i = 0; i < skyboxes.Length; i++ ) 
		{
			// create new skybox
			skyboxes[i] = createSkyBox(i);
		}
	}
	
//	protected void switchSkyBox(int skyboxId)
//	{		
//		// briefly hold up the game while we switch sky boxes
//		float currTimeScale = Time.timeScale;
//		Time.timeScale = 0;
//				
//		// swap out skybox
//		RenderSettings.skybox = skyboxes[skyboxId];
//		
//		Time.timeScale = currTimeScale;
//	}
	
	protected void switchSkyBox(int skyboxId)
	{
//		base.switchSkyBox(skyboxId);
		// swap out skybox
		RenderSettings.skybox = skyboxes[skyboxId];
		
		// rotate the world a bit so we can always view different areas of the skybox
		Transform world = level.transform.parent;
//		int rotate = Random.Range(0,10000) % 360;
		int rotate = 0;
		
		if ( skyboxId == (int)SkyBoxes.DeepSpaceBlue )
			rotate = 250;
//		else if ( skyboxId == (int)SkyBoxes.DeepSpaceBlueWithPlanet )
//			rotate = 229;
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceGreen )
			rotate = 210;
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceGreenWithPlanet )
			rotate = 65;
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceRed )
			rotate = 315;
		else if ( skyboxId == (int)SkyBoxes.DeepSpaceRedWithPlanet )
			rotate = 330;
		else 
			rotate = 0;
		
		Vector3 rotationVector = new Vector3(0, rotate, 0);
		world.eulerAngles = rotationVector;
	}
}
