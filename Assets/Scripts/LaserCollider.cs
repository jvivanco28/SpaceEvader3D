using UnityEngine;
using System.Collections;

public class LaserCollider : MonoBehaviour {

	public const float SCREEN_FLASH_ALPHA = 0.7f;
	private bool isPaused = false;
	
	// the only thing flying into these cubes are the player
	void OnTriggerEnter(Collider player)
	{
		if ( !isPaused )
		{
			// check to see if the player has flown into the laser
			if ( player != null && player.transform.tag.Equals("Player") )
			{
				GameObject levelMgr = GameObject.Find("Level");
				LevelManagerScript levelMgrScript = levelMgr.GetComponent<LevelManagerScript>();
				
				Color screenFlashColor = new Color(1,1,1);
				levelMgrScript.laserDamage(screenFlashColor, SCREEN_FLASH_ALPHA);
				
				// destroy the laser so we don't take off more damage than we need to
				Destroy(this.gameObject);
			}//end if
		}//end if
	}//end OnTriggerEnter
}
