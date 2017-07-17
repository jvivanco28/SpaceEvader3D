using UnityEngine;
using System.Collections;

public class WormholeCollider : MonoBehaviour, IPauseHandle {

	public const float SCREEN_FLASH_ALPHA = 0.8f;
	private bool isPaused = false;
	
	// the only thing flying into these cubes are the player
	void OnTriggerEnter(Collider player)
	{
		if ( !isPaused )
		{
			// if the player crashes into a cube, then send the player flying backward
			// then the levelMgr should gradually increase velocity again
			if ( player != null && player.transform.tag.Equals("Player") )
			{
				GameObject levelMgr = GameObject.Find("Level");
				LevelManagerScript levelMgrScript = levelMgr.GetComponent<LevelManagerScript>();

				Color screenFlashColor = new Color(1f, 1f, 1f);
				levelMgrScript.toggleWormhole(screenFlashColor, SCREEN_FLASH_ALPHA);
			}//end if
		}//end if
	}//end OnTriggerEnter
	
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
