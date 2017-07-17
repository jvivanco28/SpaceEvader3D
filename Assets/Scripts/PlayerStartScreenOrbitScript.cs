using UnityEngine;
using System.Collections;

public class PlayerStartScreenOrbitScript : IPlayerMove {

	public const float ORBIT_SPEED = 10;
	private Transform cam;
	
	void Start () 
	{
		cam = this.transform.FindChild("Near Camera");
		
		// get look-at object
		if ( cameraLookAt == null )
			cameraLookAt = GameObject.Find("Player").transform;
	}
	
	void LateUpdate () 
	{
			cam.RotateAround(cameraLookAt.transform.localPosition, transform.up, Time.deltaTime * ORBIT_SPEED);
			cam.LookAt(cameraLookAt, this.transform.up);			
	}
	
	public override Controller getControllerType()
	{
		return Controller.StartScreen;
	}
}
