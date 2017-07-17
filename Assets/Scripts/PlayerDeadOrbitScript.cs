using UnityEngine;
using System.Collections;

public class PlayerDeadOrbitScript : IPlayerMove {
	
	public const float CAM_ROTATE_SPEED = 30f;
	public const float SHIP_ROTATE_SPEED = 70f;
		
	private Transform cam;
	private Vector3 newCamPositionVector;
	private float newAngle;
	
	private Transform spaceShipObj;
//	private Vector3 spaceShipRotationVector;

	// Use this for initialization
	void Start () 
	{
		spaceShipObj = this.transform.FindChild("SpaceShip");
		
		cam = this.transform.FindChild("Near Camera");
		
		newCamPositionVector = new Vector3(-3.5f, 2.5f, -7f);
		
		// get look-at object
		if ( cameraLookAt == null )
			cameraLookAt = GameObject.Find("Player").transform;
		
		cam.localPosition = newCamPositionVector;
		
//		spaceShipRotationVector = new Vector3 (spaceShipObj.eulerAngles.x, spaceShipObj.eulerAngles.y, spaceShipObj.eulerAngles.z);
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
		
		// rotate the space ship
//		float newAngle = Mathf.LerpAngle(spaceShipObj.eulerAngles.z, spaceShipObj.eulerAngles.z + 4f, Time.deltaTime * ROTATE_SPEED);
//		spaceShipRotationVector.Set(transform.eulerAngles.x, transform.eulerAngles.y, newAngle);
//		spaceShipObj.eulerAngles = spaceShipRotationVector;
		spaceShipObj.Rotate(new Vector3(Time.deltaTime * SHIP_ROTATE_SPEED, 0f, 0f));
		
		if ( cameraLookAt != null ) 
		{
			cam.LookAt(cameraLookAt, this.transform.up);	
		}//end if
		
	}//end Update
	
	public override Controller getControllerType()
	{
		return Controller.Dead;
	}
}
