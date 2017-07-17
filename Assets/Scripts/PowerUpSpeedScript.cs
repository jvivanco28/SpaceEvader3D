using UnityEngine;
using System.Collections;

public class PowerUpSpeedScript : PowerUpScript 
{
	public const int POINTS = 75;
	public const float SPEED_INCREMENT = 5f; // 0.1
	
	// the player will be the only one flying into this object
	void OnTriggerEnter(Collider player) 
	{
		if ( player != null && player.transform.tag.Equals("Player") )
		{			
			if ( levelMgrScript != null )
				levelMgrScript.speedBoost(POINTS, new Color(RED, GREEN, BLUE), SCREEN_FLASH_ALPHA, SPEED_INCREMENT);
			
			// destroy the powerup so we don't accidentally hit it twice
			Destroy(this.gameObject);
		}//end if
	}//end OnTriggerEnter
}
