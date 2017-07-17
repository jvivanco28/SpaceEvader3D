using UnityEngine;
using System.Collections;

public class LevelObjectRange
{
	private int levelObjId;
	private float range;
	
	public LevelObjectRange(int levelObjId, float range)
	{
		this.levelObjId = levelObjId;
		this.range = range;
	}//end constructor
	
	public int getLevelObjId()
	{
		return levelObjId;
	}
	
	public float getRange()
	{
		return range;
	}
}//end inner class


