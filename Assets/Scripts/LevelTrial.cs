using UnityEngine;
using System.Collections;

public class LevelTrial : LevelDescription {
	
	public const int LEVEL_LENGTH = 150;
	
	public override void init(GameObject level)
	{
		this.level = level;
		setTunnelBlockRanges();
		setOutsideObjectRanges();
		setPowerUpRanges();
		
		minLevelSpeed = 0.2f;
		if ( PlayerPrefs.GetInt(PreferencesScript.TAG_CONTROLLER) == (int)IPlayerMove.Controller.EyeTracker )
			maxLevelSpeed = 0.6f;
		else
			maxLevelSpeed = 0.8f;
	}
	public override GameObject createRandomTunnelBlock()
	{
		GameObject newObj;
		
		// every second block should be an empty tunnel section
		if ( base.getNumBlocksCreated() % 2 == 0 )
			newObj = base.createTunnelBlock((int)TunnelBlocks.Outline, true);
		else
			newObj = base.createRandomTunnelBlock();
	
		return newObj;
	}
	public override int getLevelLength()
	{
		return LEVEL_LENGTH;
	}
	public override void setTunnelBlockRanges()
	{
		tunnelBlockRanges = new LevelObjectRange[14];
		tunnelBlockRanges[0] = new LevelObjectRange((int)TunnelBlocks.Outline, 0.3f);
		tunnelBlockRanges[1] = new LevelObjectRange((int)TunnelBlocks.DoubleAsteroid, 0.354f);
		tunnelBlockRanges[2] = new LevelObjectRange((int)TunnelBlocks.DoubleBeam, 0.408f);
		tunnelBlockRanges[3] = new LevelObjectRange((int)TunnelBlocks.DoubleBeamTunnel, 0.462f);
		tunnelBlockRanges[4] = new LevelObjectRange((int)TunnelBlocks.SingleLaserOutline, 0.515f);
		tunnelBlockRanges[5] = new LevelObjectRange((int)TunnelBlocks.SingleLaserTunnel, 0.569f);
		tunnelBlockRanges[6] = new LevelObjectRange((int)TunnelBlocks.LaserWallOutline, 0.623f);
		tunnelBlockRanges[7] = new LevelObjectRange((int)TunnelBlocks.LaserWallTunnel, 0.677f);
		tunnelBlockRanges[8] = new LevelObjectRange((int)TunnelBlocks.DoubleBlock, 0.731f);
		tunnelBlockRanges[9] = new LevelObjectRange((int)TunnelBlocks.DoubleBlockTunnel, 0.784f);
		tunnelBlockRanges[10] = new LevelObjectRange((int)TunnelBlocks.SingleAsteroid, 0.838f);
		tunnelBlockRanges[11] = new LevelObjectRange((int)TunnelBlocks.SingleBeam, 0.892f);
		tunnelBlockRanges[12] = new LevelObjectRange((int)TunnelBlocks.SingleBeamTunnel, 0.946f);
		tunnelBlockRanges[13] = new LevelObjectRange((int)TunnelBlocks.SingleDoorTunnel, 1f);
	}
	
	public override void setOutsideObjectRanges()
	{
		outsideObjectRanges = new LevelObjectRange[2];
		
		// set the ranges for each tunnel block that we want to use for this level
		outsideObjectRanges[0] = new LevelObjectRange((int)OutsideObjects.Beam, 0.75f);
		outsideObjectRanges[1] = new LevelObjectRange((int)OutsideObjects.Octagon, 1.0f);
	}
	
	public override void setPowerUpRanges()
	{
		powerUpCreateProbability = 0f;
		powerUpRanges = new LevelObjectRange[0];
	}
	
	public override int getNumTunnelsPerSpeedIncrease()
	{
		return 5;
	}

	// not even used
	public override int getZoneStartingSpeed ()
	{
		return 30;
	}

	// not used
	public override bool isGameFinished ()
	{
		return false;
	}
}
