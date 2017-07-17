using UnityEngine;
using System.Collections;

public class PowerUpPoint : PowerUpScript {

	public const int POINTS = 2;
	
	
//	// Update is called once per frame
//	void Update () 
//	{
//		if ( !isPaused )
//		{
//			if ( powerUpShell != null )
//				powerUpShell.Rotate(ROTATE_SPEED,ROTATE_SPEED,ROTATE_SPEED);
//		}//end if
//	}
	
	// the player will be the only one flying into this object
	void OnTriggerEnter(Collider player) 
	{
		if ( player != null && player.transform.tag.Equals("Player") )
		{			
			if ( levelMgrScript != null ) 
				levelMgrScript.incrementPointAndFuel(POINTS, new Color(RED, GREEN, BLUE), 0f);			
			
			// destroy the powerup so we don't accidentally hit it twice
			Destroy(this.gameObject);
		}//end if
	}//end OnTriggerEnter
}
