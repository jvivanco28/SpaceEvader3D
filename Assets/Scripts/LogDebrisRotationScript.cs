using UnityEngine;
using System.Collections;

public class LogDebrisRotationScript : MonoBehaviour {

	// Use this for initialization
	void Start () 
	{
		int startingAngle = Random.Range(0,10000) % 360;
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, startingAngle);
	}
}
