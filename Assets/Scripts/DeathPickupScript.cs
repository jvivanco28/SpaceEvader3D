using UnityEngine;
using System.Collections;

public class DeathPickupScript : PowerUpScript {
	
	new protected void Start () 
	{
		base.Start();
		this.transform.localRotation = new Quaternion(0,180,0,0);
	}
	
	// the player will be the only one flying into this object
	void OnTriggerEnter(Collider player) 
	{
		if ( player != null && player.transform.tag.Equals("Player") )
		{			
			if ( levelMgrScript != null ) {

				if ( !levelMgrScript.isPlayerInvulnerable() ) 
					levelMgrScript.die();
				else {
					Color screenFlashColor = new Color(1,1,1);
					levelMgrScript.destroyDeathPickup(screenFlashColor, 0.87f);
					Destroy(this.gameObject);
				}
			}
			GameObject level = GameObject.Find("Level");
			if ( level != null )
			{
				AudioManager audioManager = level.GetComponent<AudioManager>();
				if ( audioManager != null )
				{
					audioManager.playSound(AudioManager.Sound.evilLaugh);
				}
			}
		}//end if
	}//end OnTriggerEnter
}
