using UnityEngine;
using System.Collections;

public class PowerUpRotateLeftScript : PowerUpScript {

	public const int POINTS = 100;
	
	// the player will be the only one flying into this object
	void OnTriggerEnter(Collider player) 
	{
		if ( player != null && player.transform.tag.Equals("Player") )
		{			
			if ( levelMgrScript != null ) 
				levelMgrScript.playerRotateLeft(POINTS, new Color(RED, GREEN, BLUE), SCREEN_FLASH_ALPHA);
			
			// destroy the powerup so we don't accidentally hit it twice
			Destroy(this.gameObject);
		}//end if
	}//end OnTriggerEnter
}
