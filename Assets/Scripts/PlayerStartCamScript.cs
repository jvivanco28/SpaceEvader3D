using UnityEngine;
using System.Collections;

public class PlayerStartCamScript : IPlayerMove {
	
	public const float LERP_SPEED = 0.4f;
	public const float ORBIT_SPEED = 32;
	
	private float startTime;
	
	private Transform cam;
	private PreferencesScript prefsScript;
//	private LevelManagerScript levelMgrScript;
	private Transform spaceShipObj;
	private Transform secondCameraLookAt;
	
	void Start () 
	{
		GameObject guiObj = GameObject.Find("GUI");
		prefsScript = guiObj.GetComponent<PreferencesScript>();
		
//		GameObject level = GameObject.Find("Level");
//		levelMgrScript = level.GetComponent<LevelManagerScript>();
		
		spaceShipObj = this.transform.FindChild("SpaceShip");
		spaceShipObj.GetComponent<Renderer>().enabled = true;
		
		cam = this.transform.FindChild("Near Camera");
		startTime = Time.time;
		
		// get look-at object
		if ( cameraLookAt == null )
			cameraLookAt = GameObject.Find("Player").transform;
		
		secondCameraLookAt = GameObject.Find("CameraLookAt").transform;
		
		// camera position starts off in the distance
		cam.localPosition = new Vector3(0, 5, 15);

		// move space ship into the middle of the tunnel
		Vector3 playerPosition = new Vector3(0, 0, 0);
		transform.localPosition = Vector3.Lerp(transform.localPosition, playerPosition, Time.deltaTime);
		cam.LookAt(cameraLookAt, this.transform.up);
	}
	
	void LateUpdate () 
	{
		if ( !isPaused )
		{					
			// move camera closer to spaceship
			cam.localPosition = Vector3.Lerp(cam.transform.localPosition, cameraLookAt.localPosition, Time.deltaTime * LERP_SPEED);

			if ( (int)(cam.localEulerAngles.y) % 360 > 5 )
			{
				cam.RotateAround(cameraLookAt.transform.localPosition, transform.up, Time.deltaTime * ORBIT_SPEED);
				cam.LookAt(cameraLookAt, this.transform.up);
			}
			else if ( (int)(cam.localPosition.z ) == 0 )
			{
				turnOnPlayerControl();
			}
			else
			{
				// smooth look at
				Quaternion rotation = Quaternion.LookRotation(secondCameraLookAt.localPosition - cam.localPosition);
				cam.localRotation = Quaternion.Slerp(cam.localRotation, rotation, Time.deltaTime);
			}
			
			// FAIL SAFE
			// if the player control doesn't turn on for whatever reason, we'll have a failsafe after 8 secs
			if ( Time.time > startTime + 8 )
			{
				turnOnPlayerControl();
			}
		}//end if
	}
	
	public override Controller getControllerType()
	{
		return Controller.Start;
	}
	
	private void turnOnPlayerControl()
	{
		// hide spaceship
		spaceShipObj.GetComponent<Renderer>().enabled = false;
		
		cam.localPosition = new Vector3(0,0,0);
		
		// set up the default controller
		prefsScript.setupController();
	}
}
