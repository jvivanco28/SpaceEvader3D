using UnityEngine;
using System.Collections;

public class MultiCubeObstacleScript : MonoBehaviour {
		
	// Use this for initialization
	void Start () 
	{
		Transform frontSection = transform.FindChild("FrontSection");
		Transform middleSection = transform.FindChild("MiddleSection");
		Transform backSection = transform.FindChild("BackSection");
		
		randomlyPlaceCubes(frontSection);
		randomlyPlaceCubes(middleSection);
		randomlyPlaceCubes(backSection);
	}
	
	private void randomlyPlaceCubes(Transform section) 
	{
		// place the obstacle in a new position
		Transform cube1 = section.FindChild("Cube1");
		Transform cube2 = section.FindChild("Cube2");
		
		int cubePos1 = (Random.Range(0,10000)) % 9; // there are 9 possible positions
		
		if ( cube2 != null )
		{
			int cubePos2 = (Random.Range(0,10000)) % 9;
			if ( cubePos2 == cubePos1 )
				cubePos2 = (cubePos2 + 1) % 9; // make sure the cubes aren't in the same position
			placeCube(cube2, cubePos2);
		}	
		placeCube(cube1, cubePos1);
	}
	
	private void placeCube(Transform cube, int position)
	{
		if ( position == 0 )
			cube.localPosition = new Vector3(-3.34f, 3.34f, cube.localPosition.z); // top left
		else if ( position == 1 )
			cube.localPosition = new Vector3(0, 3.34f, cube.localPosition.z); // top middle
		else if ( position == 2 )
			cube.localPosition = new Vector3(3.34f, 3.34f, cube.localPosition.z); // top right
		else if ( position == 3 )
			cube.localPosition = new Vector3(-3.34f, 0, cube.localPosition.z); // middle left
		else if ( position == 4 )
			cube.localPosition = new Vector3(0, 0, cube.localPosition.z); // middle middle
		else if ( position == 5 )
			cube.localPosition = new Vector3(3.34f, 0, cube.localPosition.z); // middle right
		else if ( position == 6 )
			cube.localPosition = new Vector3(-3.34f, -3.34f, cube.localPosition.z); // bottom left
		else if ( position == 7 )
			cube.localPosition = new Vector3(0, -3.34f, cube.localPosition.z); // bottom middle
		else
			cube.localPosition = new Vector3(3.34f, -3.34f, cube.localPosition.z); // bottom right
	}
}
