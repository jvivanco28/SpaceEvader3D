using UnityEngine;
using System.Collections;

public class PlayerFinishedOrbitScript : IPlayerMove {

	public const float CAM_ROTATE_SPEED = 30f;

	private Transform cam;
	private Vector3 newCamPositionVector;
	private float newAngle;

	// Use this for initialization
	void Start () 
	{
		cam = this.transform.FindChild("Near Camera");
		
		newCamPositionVector = new Vector3(-3.5f, 2.5f, -5.5f);
		
		// get look-at object
		if ( cameraLookAt == null )
			cameraLookAt = GameObject.Find("Player").transform;
		
		cam.localPosition = newCamPositionVector;
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		// NOTE: I don't really know why, but if we don't have this chunk of code
		// which moves the player (to the same spot pretty much), then the collision
		// detection won't work for crashing into anything but lasers
		// create a new vector of the desired position
		Vector3 newPositionVector;
		if ( currUpVector == PlayerUpVector.worldUp )
			newPositionVector = new Vector3(xScaled, yScaled, 0);
		else if ( currUpVector == PlayerUpVector.worldLeft)
			newPositionVector = new Vector3(-yScaled, xScaled, 0);
		else if ( currUpVector == PlayerUpVector.worldDown)
			newPositionVector = new Vector3(-xScaled, -yScaled, 0);
		else
			newPositionVector = new Vector3(yScaled, -xScaled, 0);
		
		// move to the new absolute position
		transform.localPosition = Vector3.Lerp(transform.localPosition, newPositionVector, Time.deltaTime);
		
		// orbit camera
		cam.transform.RotateAround(cameraLookAt.transform.localPosition, transform.up, Time.deltaTime * CAM_ROTATE_SPEED);

		if ( cameraLookAt != null ) 
		{
			cam.LookAt(cameraLookAt, this.transform.up);	
		}//end if
		
	}//end Update
	
	public override Controller getControllerType()
	{
		return Controller.Finished;
	}
}
