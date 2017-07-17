using UnityEngine;
using System.Collections;

public class RandomXYPlacement : MonoBehaviour {
	
	public float minX;
	public float maxX;
	public float minY;
	public float maxY;
	
	// Use this for initialization
	void Start () 
	{
		float x = Random.Range(minX, maxX);
		float y = Random.Range(minY, maxY);
		
		Vector3 newPosition = new Vector3(x,y,transform.localPosition.z); // z value stays unchanged
		transform.localPosition = newPosition;
	}
}
