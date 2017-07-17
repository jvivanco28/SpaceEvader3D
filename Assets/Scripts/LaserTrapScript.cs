using UnityEngine;
using System.Collections;

public class LaserTrapScript : MonoBehaviour, IPauseHandle {
	
	public const float DEFAULT_LASER_SPAN = 10;
	public float laserSpan;

	// private vars
	private float speed;	
	private bool isPaused = false;
	
	// Use this for initialization
	void Start () {
		
		if ( laserSpan == 0 )
			laserSpan = DEFAULT_LASER_SPAN;
		
		// determine the speed of the oscillating laser
		speed = Random.Range(1.0f, 4.0f);
			
		// determine the direction of the laser
		int direction = Random.Range(0,10000) % 2;
		if ( direction == 0 )
			speed = -speed;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if ( !isPaused )
		{
			// don't forget that the laser's starting position is in the MIDDLE of the tunnel block
			// so we'll subtract subtract half of the height to get it there . Also note that we're dividing
			// height by 2.25 so that the lasers won't run into the planes as the oscillate 
			float offset = ((1 + Mathf.Sin(Time.time * speed)) * laserSpan / 2.25f) - (laserSpan / 2.25f);
			transform.localPosition = new Vector3(transform.localPosition.x, offset, transform.localPosition.z);
			
//			// check to see if the player has flown into the laser
//			if ( Physics.Raycast(transform.position, transform.right, out hit, LASER_SPAN) )
//			{
//				if ( hit.collider.tag == "Player" )
//				{
//					GameObject levelMgr = GameObject.Find("Level");
//					LevelManagerScript levelMgrScript = levelMgr.GetComponent<LevelManagerScript>();
//					
//					Color screenFlashColor = new Color(1,1,1);
//					levelMgrScript.laserDamage(screenFlashColor, SCREEN_FLASH_ALPHA);
//					
//					// destroy the laser so we don't take off more damage than we need to
//					Destroy(this.gameObject);
//				}//end if
//			}//end if
		}//end if
	}//end update
	
	public void OnPauseGame()
	{
		isPaused = true;
	}
	
	public void OnResumeGame()
	{
		isPaused = false;
	}
}
