using UnityEngine;
using System.Collections;


public class PlayerMoveAbsScript : IPlayerMove {
	
	public float SENSITIVITY = 1.25f;
	public float lerp_speed = 3.25f;
	
	private Vector3 mouseCoords;
	private Vector3 newPositionVector;
	private Transform cam;
	
	// Use this for initialization
	void Start () {
	
		PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)Controller.Absolute);
		
		// set look-at object
		if ( cameraLookAt == null )
		{
			cameraLookAt = GameObject.Find("CameraLookAt").transform;
			cam = this.transform.FindChild("Near Camera");
		}
		give = 1.25f;
	}//end start
	
	// Update is called once per frame
	void LateUpdate () {
		
		if ( !isPaused )
		{
			// get absolute coordinates of the mouse
			mouseCoords = Input.mousePosition;
			
			// make sure the coords are scaled down btw the min and max X vals (distance btw planes)	
			xScaled = (mouseCoords.x / Screen.width) * (Mathf.Abs(maxX - minX)) - (Mathf.Abs(maxX - minX) / 2);
			xScaled *= SENSITIVITY;
			if ( xScaled > maxX - give )
				xScaled = maxX - give;
			else if ( xScaled < minX + give)
				xScaled = minX + give;
			
			yScaled = (mouseCoords.y / Screen.height) * (Mathf.Abs(maxY - minY)) - (Mathf.Abs(maxY - minY) / 2);
			yScaled *= SENSITIVITY;
			if ( yScaled > maxY - give)
				yScaled = maxY - give;
			else if ( yScaled < minY + give)
				yScaled = minY + give;
									
			// create a new vector of the desired position
			if ( currUpVector == PlayerUpVector.worldUp )
				newPositionVector = new Vector3(xScaled, yScaled, 0);
			else if ( currUpVector == PlayerUpVector.worldLeft)
				newPositionVector = new Vector3(-yScaled, xScaled, 0);
			else if ( currUpVector == PlayerUpVector.worldDown)
				newPositionVector = new Vector3(-xScaled, -yScaled, 0);
			else
				newPositionVector = new Vector3(yScaled, -xScaled, 0);
			
			// move to the new absolute position
			transform.localPosition = Vector3.Lerp(transform.localPosition, newPositionVector, Time.deltaTime * lerp_speed);
			
			// if we've hit a rotation powerup, then rotate to the new goal angle
			if ( (int)transform.eulerAngles.z != (int)goalAngle )
			{
				float newAngle = Mathf.LerpAngle(transform.eulerAngles.z, goalAngle, Time.deltaTime * lerp_speed);
				transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, newAngle);
			}//end if
			
			// keep looking at an invisible point somwehere down the middle of the tunnel
			if ( cameraLookAt != null ) 
			{
				cam.LookAt(cameraLookAt, this.transform.up);	
			}//end if			
		}//end if
	}//end Update
	
	public override Controller getControllerType()
	{
		return Controller.Absolute;
	}	
}//enc class
