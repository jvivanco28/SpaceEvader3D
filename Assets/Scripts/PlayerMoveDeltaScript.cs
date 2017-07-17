using UnityEngine;
using System.Collections;

public class PlayerMoveDeltaScript : IPlayerMove {
	
//	public const float SENSITIVITY = 0.025f;
//	public const float SENSITIVITY = 2.8f;
	public const float SENSITIVITY = 19f;

	public float lerp_speed = 4f;
	
	private Touch[] touches;
	private Touch touch;
	private Vector2 deltaPos;
	private Vector3 newPositionVector;
	private Transform cam;
	
	// Use this for initialization
	void Start () 
	{		
		PlayerPrefs.SetInt(PreferencesScript.TAG_CONTROLLER, (int)Controller.Delta);
		
		// set look-at object
		if ( cameraLookAt == null )
		{
			cameraLookAt = GameObject.Find("CameraLookAt").transform;
			cam = this.transform.FindChild("Near Camera");
		}
		give = 1.25f;
	}//end Start
	
	public override void rotateLeft()
	{
		base.rotateLeft();
		
		// so we stay in the same spot when we rotate
		float tempX = xScaled;
		xScaled = yScaled;
		yScaled = -tempX;
		
	}
	
	public override void rotateRight()
	{
		base.rotateRight();
		
		// so we stay in the same spot when we rotate
		float tempX = xScaled;
		xScaled = -yScaled;
		yScaled = tempX;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if ( !isPaused )
		{			
			touches = Input.touches;
			if ( touches != null && touches.Length >= 1 )
			{
				touch = touches[0];
	            if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
				{
					deltaPos = touch.deltaPosition;
//					xScaled += (deltaPos.x / Screen.dpi) * SENSITIVITY;
//					yScaled += (deltaPos.y / Screen.dpi) * SENSITIVITY;
					xScaled += (deltaPos.x / Screen.height) * SENSITIVITY;
					yScaled += (deltaPos.y / Screen.height) * SENSITIVITY;
					
					if ( xScaled > maxX - give )
						xScaled = maxX - give;
					else if ( xScaled < minX + give)
						xScaled = minX + give;
					
					if ( yScaled > maxY - give)
						yScaled = maxY - give;
					else if ( yScaled < minY + give)
						yScaled = minY + give;
				}//end if
	        }//end foreach
			
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
		return Controller.Delta;
	}
}//end class
