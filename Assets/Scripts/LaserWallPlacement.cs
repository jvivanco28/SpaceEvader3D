using UnityEngine;
using System.Collections;

public class LaserWallPlacement : MonoBehaviour {
	
	// Use this for initialization
	void Start () 
	{
		int random = Random.Range(0,1000) % 3;
		if ( random == 0 )
			transform.Translate(0, 3.4f ,0);
		else if ( random == 1 )
			transform.Translate(0, -3.4f ,0);
	}
}
