using UnityEngine;
using System.Collections;

public class CubeLaserPlacement : MonoBehaviour {

	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	
	// Use this for initialization
	void Start () 
	{
		// place the obstacle in a new position
		Transform cube = transform.FindChild("Cube");
		Transform horizontalLaser = transform.FindChild("HorizontalLaser");
		Transform verticalLaser = transform.FindChild("VerticalLaser");
		
		float x = Random.Range(minX, maxX);
		float y = Random.Range(minY, maxY);
		
		Vector3 cubePosition = new Vector3(x, y, 0);
		Vector3 hLaserPosition = new Vector3(0, y, 0);
		Vector3 vLaserPosition = new Vector3(x, 0, 0);
		
		cube.localPosition = cubePosition;
		horizontalLaser.localPosition = hLaserPosition;
		verticalLaser.localPosition = vLaserPosition;
	}
}
