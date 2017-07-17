using UnityEngine;
using System.Collections;

public class PlayerTouchMoveScript : MonoBehaviour {
	
	public float maxX = 5f;
	public float minX = -5f;
	public float maxY = 5f;
	public float minY = -5f;
	
	// so the camera never translates all the way to the plane
	// (avoids clipping)
	public float give = 0.5f;
	
	// gives us some bit of rotation when we translate
	public Transform cameraLookAt;
	public float lerp_speed = 1f;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		// get absolute coordinates of the touch
		Touch[] touchList = Input.touches;
		
		if ( touchList != null && touchList.Length > 0 )
		{
			Touch touch = touchList[0];
			Vector3 touchCoords = touch.position;
			
			// make sure the coords are scaled down btw the min and max X vals (distance btw planes)
			float xAbsolute = touchCoords.x;	
			float xScaled = (xAbsolute / Screen.width) * (Mathf.Abs(maxX - minX) - 2 * give) - (Mathf.Abs(maxX - minX) / 2);
			if ( xScaled > maxX - give )
				xScaled = maxX - give;
			else if ( xScaled < minX + give )
				xScaled = minX + give;
			
			float yAbsolute = touchCoords.y;
			float yScaled = (yAbsolute / Screen.height) * (Mathf.Abs(maxY - minY) - 2 * give) - (Mathf.Abs(maxY - minY) / 2);
			if ( yScaled > maxY - give )
				yScaled = maxY - give;
			else if ( yScaled < minY + give)
				yScaled = minY + give;
					
			// create a new vector of the desired position
			Vector3 newPositionVector = new Vector3(xScaled, yScaled, 0);
		
			// move to the new absolute position
			transform.position = Vector3.Lerp(transform.position, newPositionVector, Time.deltaTime * lerp_speed);
			
			// keep looking at an invisible point someone down the middle of the tunnel
			if ( cameraLookAt != null ) 
				transform.LookAt(cameraLookAt);
		}//end if
	}//end Update
}
