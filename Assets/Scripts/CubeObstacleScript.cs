using UnityEngine;
using System.Collections;

public class CubeObstacleScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		// place the obstacle in a new position
		Transform cube1 = transform.FindChild("Cube1");
		Transform cube2 = transform.FindChild("Cube2");
		
		if ( cube1 != null )
			randomlyPlaceCube(cube1);
		if ( cube2 != null )
			randomlyPlaceCube(cube2);
	}//end start
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	// ************ PRIVAVE METHODS ************ //
	
	private void randomlyPlaceCube(Transform cube) 
	{
		if ( cube != null ) {
			float newX, newY;
			int randomNum = (Random.Range(0,10000)) % 2;
			
			// determine X coord
			if ( randomNum == 0)
				newX = -2.5f;
			else
				newX = 2.5f;
			
			// determin Y coord
			randomNum = (Random.Range(0,10000)) % 2;
			if ( randomNum == 0 )
				newY = -2.5f;
			else
				newY = 2.5f;
			
			cube.localPosition = new Vector3(newX, newY, cube.localPosition.z);
		}// end if
	}
}
